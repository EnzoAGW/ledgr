using Ledgr.Application.Team.Commands.InviteUser;
using Ledgr.Application.Team.Commands.RemoveUser;
using Ledgr.Application.Team.Commands.UpdateUserRole;
using Ledgr.Application.Team.Queries.GetTeam;
using Ledgr.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ledgr.Api.Controllers;

[ApiController]
[Route("api/team")]
[Authorize(Roles = "Admin")]
public class TeamController(IMediator mediator) : ControllerBase
{
    private int OrgId    => int.Parse(User.FindFirst("orgId")!.Value);
    private int UserId   => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    private UserRole Role => Enum.Parse<UserRole>(User.FindFirst(ClaimTypes.Role)!.Value);

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(await mediator.Send(new GetTeamQuery(OrgId), ct));

    [HttpPost]
    public async Task<IActionResult> Invite([FromBody] InviteUserRequest req, CancellationToken ct)
    {
        var id = await mediator.Send(
            new InviteUserCommand(OrgId, Role, req.Name, req.Email, req.Password, req.Role), ct);
        return CreatedAtAction(nameof(List), new { id });
    }

    [HttpPatch("{id:int}/role")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleRequest req, CancellationToken ct)
    {
        await mediator.Send(new UpdateUserRoleCommand(id, OrgId, UserId, req.Role), ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remove(int id, CancellationToken ct)
    {
        await mediator.Send(new RemoveUserCommand(id, OrgId, UserId), ct);
        return NoContent();
    }
}

public record InviteUserRequest(string Name, string Email, string Password, UserRole Role);
public record UpdateRoleRequest(UserRole Role);
