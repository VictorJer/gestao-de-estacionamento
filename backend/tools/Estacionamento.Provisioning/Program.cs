using Estacionamento.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
if (!builder.Environment.IsDevelopment())
{
    throw new InvalidOperationException("Provisionamento de teste permitido somente em Development.");
}

var connection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection é obrigatório.");
var username = Environment.GetEnvironmentVariable("ESTACIONAMENTO_TEST_USER");
var password = Environment.GetEnvironmentVariable("ESTACIONAMENTO_TEST_PASSWORD");
var role = Environment.GetEnvironmentVariable("ESTACIONAMENTO_TEST_ROLE");
if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)
    || role is not ("manobrista" or "caixa" or "administrador"))
{
    throw new InvalidOperationException("Informe usuário, senha e perfil de teste válidos por variáveis de ambiente.");
}

builder.Services.AddDbContext<EmployeeDbContext>(options => options.UseSqlServer(connection));
builder.Services.AddIdentityCore<Employee>(options => options.Password.RequiredLength = 12)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<EmployeeDbContext>();
using var app = builder.Build();
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
await db.Database.MigrateAsync();
await using var transaction = await db.Database.BeginTransactionAsync();
var users = scope.ServiceProvider.GetRequiredService<UserManager<Employee>>();
var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
foreach (var name in new[] { "manobrista", "caixa", "administrador" })
{
    if (!await roles.RoleExistsAsync(name))
    {
        var result = await roles.CreateAsync(new IdentityRole(name));
        if (!result.Succeeded) throw new InvalidOperationException("Não foi possível preparar os perfis de teste.");
    }
}

if (await users.FindByNameAsync(username) is not null)
{
    throw new InvalidOperationException("Usuário de teste já existe. Não altere credenciais automaticamente.");
}

var created = await users.CreateAsync(new Employee { UserName = username, IsActive = true }, password);
if (!created.Succeeded) throw new InvalidOperationException("Não foi possível criar o usuário de teste.");
var assigned = await users.AddToRoleAsync((await users.FindByNameAsync(username))!, role);
if (!assigned.Succeeded) throw new InvalidOperationException("Não foi possível atribuir o perfil de teste.");
await transaction.CommitAsync();
Console.WriteLine("Usuário de teste criado sem exibir credenciais.");
