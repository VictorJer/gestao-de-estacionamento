using Microsoft.AspNetCore.Identity;

namespace Estacionamento.Infrastructure.Identity;

public sealed class EmployeeCredentials(UserManager<Employee> users)
{
    public async Task<Employee?> AuthenticateAsync(string username, string password)
    {
        var employee = await users.FindByNameAsync(username);
        if (employee is null || !employee.IsActive || await users.IsLockedOutAsync(employee))
        {
            return null;
        }

        return await users.CheckPasswordAsync(employee, password) ? employee : null;
    }
}
