using Microsoft.AspNetCore.Identity;

namespace Estacionamento.Infrastructure.Identity;

public sealed class Employee : IdentityUser
{
    public bool IsActive { get; set; } = true;
}
