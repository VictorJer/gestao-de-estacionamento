using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Estacionamento.Infrastructure.Identity;

public sealed class EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
    : IdentityDbContext<Employee, IdentityRole, string>(options)
{
}
