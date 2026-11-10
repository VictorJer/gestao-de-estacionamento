using System.Net;
using System.Net.Http.Json;
using Estacionamento.Infrastructure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Estacionamento.IntegrationTests;

[TestClass]
public sealed class SessionHttpTests
{
    [TestMethod]
    public async Task CookieExpiradoNaoMantemSessao()
    {
        var server = Environment.GetEnvironmentVariable("ESTACIONAMENTO_TEST_SQL")
            ?? (OperatingSystem.IsWindows()
                ? "Server=.\\SQLEXPRESS;Integrated Security=True;Encrypt=True;TrustServerCertificate=True"
                : throw new InvalidOperationException("Defina ESTACIONAMENTO_TEST_SQL para um SQL Server real."));
        var connection = new SqlConnectionStringBuilder(server)
        {
            InitialCatalog = "Estacionamento_Fase1_ExpiryTest_" + Guid.NewGuid().ToString("N")
        };
        using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(web =>
        {
            web.UseSetting("ConnectionStrings:DefaultConnection", connection.ConnectionString);
            web.UseSetting("Security:AllowedOrigin", "https://localhost");
            web.UseSetting("Session:LifetimeSeconds", "1");
            web.UseEnvironment("Development");
        });
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
        try
        {
            await db.Database.MigrateAsync();
            var users = scope.ServiceProvider.GetRequiredService<UserManager<Employee>>();
            Assert.IsTrue((await users.CreateAsync(new Employee { UserName = "expira" }, "SenhaDeTeste!12345")).Succeeded);
            var token = (await (await client.GetAsync("/api/v1/session/antiforgery"))
                .Content.ReadFromJsonAsync<AntiforgeryToken>())!.RequestToken;
            using var login = AuthorizedPost("/api/v1/session/login", token,
                new { Username = "expira", Password = "SenhaDeTeste!12345" });
            Assert.AreEqual(HttpStatusCode.OK, (await client.SendAsync(login)).StatusCode);
            await Task.Delay(TimeSpan.FromSeconds(2));
            Assert.AreEqual(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/v1/session")).StatusCode);
        }
        finally
        {
            await db.Database.EnsureDeletedAsync();
        }
    }

    [TestMethod]
    public async Task SessaoUsaCookieSeguroELogoutRevogaAcesso()
    {
        var server = Environment.GetEnvironmentVariable("ESTACIONAMENTO_TEST_SQL")
            ?? (OperatingSystem.IsWindows()
                ? "Server=.\\SQLEXPRESS;Integrated Security=True;Encrypt=True;TrustServerCertificate=True"
                : throw new InvalidOperationException("Defina ESTACIONAMENTO_TEST_SQL para um SQL Server real."));
        var database = "Estacionamento_Fase1_HttpTest_" + Guid.NewGuid().ToString("N");
        var connection = new SqlConnectionStringBuilder(server) { InitialCatalog = database };
        using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(web =>
        {
            web.UseSetting("ConnectionStrings:DefaultConnection", connection.ConnectionString);
            web.UseSetting("Security:AllowedOrigin", "https://localhost");
            web.UseEnvironment("Development");
        });
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false,
        });
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
        try
        {
            await db.Database.MigrateAsync();
            var manager = scope.ServiceProvider.GetRequiredService<UserManager<Employee>>();
            var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            Assert.IsTrue((await roles.CreateAsync(new IdentityRole("caixa"))).Succeeded);
            var employee = new Employee { UserName = "caixa.http", IsActive = true };
            Assert.IsTrue((await manager.CreateAsync(employee, "SenhaDeTeste!12345")).Succeeded);
            Assert.IsTrue((await manager.AddToRoleAsync(employee, "caixa")).Succeeded);

            var bootstrap = await client.GetAsync("/api/v1/session/antiforgery");
            Assert.AreEqual(HttpStatusCode.OK, bootstrap.StatusCode);
            Assert.IsTrue(bootstrap.Headers.CacheControl?.NoStore);
            var token = (await bootstrap.Content.ReadFromJsonAsync<AntiforgeryToken>())?.RequestToken;
            Assert.IsFalse(string.IsNullOrWhiteSpace(token));
            Assert.AreEqual(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/v1/session")).StatusCode);
            using var forged = new HttpRequestMessage(HttpMethod.Get, "/api/v1/test/admin-only");
            forged.Headers.Add("X-User-Role", "administrador");
            Assert.AreEqual(HttpStatusCode.Unauthorized, (await client.SendAsync(forged)).StatusCode);
            Assert.AreEqual(HttpStatusCode.Forbidden,
                (await client.PostAsJsonAsync("/api/v1/session/login", new { Username = "caixa.http", Password = "errada" })).StatusCode);
            using var wrongOrigin = AuthorizedPost("/api/v1/session/login", token!,
                new { Username = "caixa.http", Password = "SenhaDeTeste!12345" });
            wrongOrigin.Headers.Remove("Origin");
            wrongOrigin.Headers.Add("Origin", "https://atacante.example");
            Assert.AreEqual(HttpStatusCode.Forbidden, (await client.SendAsync(wrongOrigin)).StatusCode);
            using var wrongToken = AuthorizedPost("/api/v1/session/login", "token-invalido",
                new { Username = "caixa.http", Password = "SenhaDeTeste!12345" });
            Assert.AreEqual(HttpStatusCode.Forbidden, (await client.SendAsync(wrongToken)).StatusCode);
            using var invalidPassword = AuthorizedPost("/api/v1/session/login", token!,
                new { Username = "caixa.http", Password = "errada" });
            Assert.AreEqual(HttpStatusCode.Unauthorized, (await client.SendAsync(invalidPassword)).StatusCode);

            using var validPassword = AuthorizedPost("/api/v1/session/login", token!,
                new { Username = "caixa.http", Password = "SenhaDeTeste!12345" });
            var login = await client.SendAsync(validPassword);
            Assert.AreEqual(HttpStatusCode.OK, login.StatusCode);
            var cookie = login.Headers.GetValues("Set-Cookie").Single(value => value.StartsWith("Estacionamento.Session="));
            StringAssert.Contains(cookie, "httponly", StringComparison.OrdinalIgnoreCase);
            StringAssert.Contains(cookie, "secure", StringComparison.OrdinalIgnoreCase);
            StringAssert.Contains(cookie, "path=/api", StringComparison.OrdinalIgnoreCase);
            Assert.IsFalse(cookie.Contains("domain=", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(login.Headers.CacheControl?.NoStore);

            var session = await client.GetAsync("/api/v1/session");
            Assert.AreEqual(HttpStatusCode.OK, session.StatusCode);
            Assert.IsTrue(session.Headers.CacheControl?.NoStore);
            var body = await session.Content.ReadAsStringAsync();
            StringAssert.Contains(body, "caixa.http");
            StringAssert.Contains(body, "caixa");
            Assert.AreEqual(HttpStatusCode.Forbidden, (await client.GetAsync("/api/v1/test/admin-only")).StatusCode);

            var admin = new Employee { UserName = "admin.http" };
            Assert.IsTrue((await roles.CreateAsync(new IdentityRole("administrador"))).Succeeded);
            Assert.IsTrue((await manager.CreateAsync(admin, "SenhaDeTeste!12345")).Succeeded);
            Assert.IsTrue((await manager.AddToRoleAsync(admin, "administrador")).Succeeded);

            var authenticatedToken = (await (await client.GetAsync("/api/v1/session/antiforgery"))
                .Content.ReadFromJsonAsync<AntiforgeryToken>())!.RequestToken;
            Assert.AreEqual(HttpStatusCode.Forbidden, (await client.PostAsync("/api/v1/session/logout", null)).StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, (await client.GetAsync("/api/v1/session")).StatusCode);
            using var logout = AuthorizedPost("/api/v1/session/logout", authenticatedToken);
            Assert.AreEqual(HttpStatusCode.NoContent, (await client.SendAsync(logout)).StatusCode);
            Assert.AreEqual(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/v1/session")).StatusCode);
            using var noRegistration = AuthorizedPost("/api/v1/session/register", token!);
            Assert.AreEqual(HttpStatusCode.Unauthorized, (await client.SendAsync(noRegistration)).StatusCode);

            var adminToken = (await (await client.GetAsync("/api/v1/session/antiforgery"))
                .Content.ReadFromJsonAsync<AntiforgeryToken>())!.RequestToken;
            using var adminLogin = AuthorizedPost("/api/v1/session/login", adminToken,
                new { Username = "admin.http", Password = "SenhaDeTeste!12345" });
            Assert.AreEqual(HttpStatusCode.OK, (await client.SendAsync(adminLogin)).StatusCode);
            Assert.AreEqual(HttpStatusCode.NoContent, (await client.GetAsync("/api/v1/test/admin-only")).StatusCode);
        }
        finally
        {
            await db.Database.EnsureDeletedAsync();
        }
    }

    private sealed record AntiforgeryToken(string RequestToken);

    private static HttpRequestMessage AuthorizedPost(string path, string token, object? payload = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, path);
        request.Headers.Add("Origin", "https://localhost");
        request.Headers.Add("X-CSRF-TOKEN", token);
        if (payload is not null) request.Content = JsonContent.Create(payload);
        return request;
    }
}
