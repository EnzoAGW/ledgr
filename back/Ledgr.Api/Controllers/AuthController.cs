using Ledgr.Application.Auth.Commands.Login;
using Ledgr.Application.Auth.Commands.RegisterOrg;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ledgr.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterOrgCommand cmd, CancellationToken ct)
    {
        var orgId = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(Login), new { orgId });
    }
}
