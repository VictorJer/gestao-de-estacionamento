using Estacionamento.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Estacionamento.IntegrationTests;

[TestClass]
public sealed class EmployeePersistenceTests
{
    [TestMethod]
    public async Task MigracaoAutenticaSomenteFuncionarioAtivoEmSqlServerReal()
    {
        var serverConnection = Environment.GetEnvironmentVariable("ESTACIONAMENTO_TEST_SQL")
            ?? (OperatingSystem.IsWindows()
                ? "Server=.\\SQLEXPRESS;Integrated Security=True;Encrypt=True;TrustServerCertificate=True"
                : throw new InvalidOperationException("Defina ESTACIONAMENTO_TEST_SQL para um SQL Server real."));
        var uniqueDatabase = "Estacionamento_Fase1_Test_" + Guid.NewGuid().ToString("N");
        var connection = new SqlConnectionStringBuilder(serverConnection) { InitialCatalog = uniqueDatabase };

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<EmployeeDbContext>(options => options.UseSqlServer(connection.ConnectionString));
        services.AddIdentityCore<Employee>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<EmployeeDbContext>();
        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
        try
        {
            await db.Database.MigrateAsync();
            var manager = scope.ServiceProvider.GetRequiredService<UserManager<Employee>>();
            var auth = new EmployeeCredentials(manager);
            var employee = new Employee { UserName = "caixa.teste", IsActive = true };
            Assert.IsTrue((await manager.CreateAsync(employee, "SenhaDeTeste!12345")).Succeeded);

            Assert.IsNotNull(await auth.AuthenticateAsync("caixa.teste", "SenhaDeTeste!12345"));
            Assert.IsNull(await auth.AuthenticateAsync("inexistente", "SenhaDeTeste!12345"));
            Assert.IsNull(await auth.AuthenticateAsync("caixa.teste", "SenhaInvalida!12345"));

            employee.IsActive = false;
            Assert.IsTrue((await manager.UpdateAsync(employee)).Succeeded);
            Assert.IsNull(await auth.AuthenticateAsync("caixa.teste", "SenhaDeTeste!12345"));
        }
        finally
        {
            // Descarta apenas o banco exclusivo criado por este teste.
            await db.Database.EnsureDeletedAsync();
        }
    }
}
