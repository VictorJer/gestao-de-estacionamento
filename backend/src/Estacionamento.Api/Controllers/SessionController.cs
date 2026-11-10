using System.ComponentModel.DataAnnotations;
using Estacionamento.Infrastructure.Identity;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Estacionamento.Api.Controllers;

[ApiController]
[Route("api/v1/session")]
public sealed class SessionController(
    IAntiforgery antiforgery,
    EmployeeCredentials credentials,
    SignInManager<Employee> signIn,
    UserManager<Employee> users) : ControllerBase
{
    [HttpGet("antiforgery")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AntiforgeryResponse), StatusCodes.Status200OK)]
    public ActionResult<AntiforgeryResponse> GetAntiforgery()
    {
        Response.Headers.CacheControl = "no-store";
        var token = antiforgery.GetAndStoreTokens(HttpContext).RequestToken!;
        return new AntiforgeryResponse(token);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SessionResponse>> Login(LoginRequest request)
    {
        Response.Headers.CacheControl = "no-store";
        var employee = await credentials.AuthenticateAsync(request.Username, request.Password);
        if (employee is null) return Unauthorized();

        await signIn.SignInAsync(employee, isPersistent: false);
        return new SessionResponse(employee.UserName!, (await users.GetRolesAsync(employee)).ToArray());
    }

    [HttpGet]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SessionResponse>> GetSession()
    {
        Response.Headers.CacheControl = "no-store";
        var employee = await users.GetUserAsync(User);
        if (employee is null || !employee.IsActive) return Unauthorized();
        return new SessionResponse(employee.UserName!, (await users.GetRolesAsync(employee)).ToArray());
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout()
    {
        Response.Headers.CacheControl = "no-store";
        await signIn.SignOutAsync();
        return NoContent();
    }
}

public sealed record LoginRequest([Required] string Username, [Required] string Password);
public sealed record SessionResponse(string Username, string[] Roles);
public sealed record AntiforgeryResponse(string RequestToken);
