using Estacionamento.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Configure ConnectionStrings:DefaultConnection antes de iniciar a API.");
var sessionLifetime = builder.Configuration.GetValue<int?>("Session:LifetimeSeconds") ?? 8 * 60 * 60;
if (sessionLifetime < 1) throw new InvalidOperationException("Session:LifetimeSeconds deve ser positivo.");
var allowedOrigin = builder.Configuration["Security:AllowedOrigin"]
    ?? throw new InvalidOperationException("Configure Security:AllowedOrigin para a origem HTTPS da SPA.");
if (!Uri.TryCreate(allowedOrigin, UriKind.Absolute, out var allowedOriginUri)
    || allowedOriginUri.Scheme != Uri.UriSchemeHttps || allowedOriginUri.AbsolutePath != "/")
{
    throw new InvalidOperationException("Security:AllowedOrigin deve ser uma origem HTTPS sem caminho.");
}

builder.Services.AddDbContext<EmployeeDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddIdentityCore<Employee>()
    .AddRoles<IdentityRole>()
    .AddSignInManager()
    .AddEntityFrameworkStores<EmployeeDbContext>();
builder.Services.AddScoped<EmployeeCredentials>();
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies(options => options.ApplicationCookie!.Configure(cookie =>
    {
        cookie.Cookie.Name = "Estacionamento.Session";
        cookie.Cookie.Path = "/api";
        cookie.Cookie.HttpOnly = true;
        cookie.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        cookie.Cookie.SameSite = SameSiteMode.Lax;
        cookie.ExpireTimeSpan = TimeSpan.FromSeconds(sessionLifetime);
        cookie.SlidingExpiration = false;
        cookie.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        cookie.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
        cookie.Events.OnValidatePrincipal = async context =>
        {
            var manager = context.HttpContext.RequestServices.GetRequiredService<UserManager<Employee>>();
            var user = context.Principal is null ? null : await manager.GetUserAsync(context.Principal);
            if (user is null || !user.IsActive)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            }
        };
    }));
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
    .AddPolicy("Manobrista", policy => policy.RequireRole("manobrista"))
    .AddPolicy("Caixa", policy => policy.RequireRole("caixa"))
    .AddPolicy("Administrador", policy => policy.RequireRole("administrador"));
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "Estacionamento.Antiforgery";
    options.Cookie.Path = "/api";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();
var keyRingDirectory = builder.Configuration["Security:KeyRingDirectory"];
if (!string.IsNullOrWhiteSpace(keyRingDirectory))
{
    builder.Services.AddDataProtection()
        .SetApplicationName("Estacionamento.Fase1")
        .PersistKeysToFileSystem(new DirectoryInfo(keyRingDirectory));
}
var proxyAddress = builder.Configuration["Security:KnownProxy"];
if (!string.IsNullOrWhiteSpace(proxyAddress))
{
    if (!IPAddress.TryParse(proxyAddress, out var address))
        throw new InvalidOperationException("Security:KnownProxy deve ser um endereço IP válido.");
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
        options.ForwardLimit = 1;
        options.KnownProxies.Add(address);
        options.AllowedHosts.Add(allowedOriginUri.Host);
    });
}

var app = builder.Build();

if (app.Environment.IsDevelopment() && app.Configuration.GetValue<bool>("Database:ApplyMigrations"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
    for (var attempt = 0; ; attempt++)
    {
        try
        {
            await db.Database.MigrateAsync();
            break;
        }
        catch (Microsoft.Data.SqlClient.SqlException) when (attempt < 30)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapGet("/api/v1/test/admin-only", () => Results.NoContent())
        .RequireAuthorization("Administrador")
        .ExcludeFromDescription();
}

if (!string.IsNullOrWhiteSpace(proxyAddress)) app.UseForwardedHeaders();
app.UseAuthentication();
app.Use(async (context, next) =>
{
    if (!HttpMethods.IsGet(context.Request.Method)
        && !HttpMethods.IsHead(context.Request.Method)
        && !HttpMethods.IsOptions(context.Request.Method))
    {
        // Clientes sem Origin não têm exceção implícita: ações de navegador exigem origem e token.
        if (!string.Equals(context.Request.Headers.Origin.ToString(), allowedOrigin, StringComparison.Ordinal)
            || !context.Request.IsHttps)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        try
        {
            await context.RequestServices.GetRequiredService<IAntiforgery>().ValidateRequestAsync(context);
        }
        catch (AntiforgeryValidationException)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }
    }

    await next();
});
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;
