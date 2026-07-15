using Ledgr.Application.Accounts.Commands.CreateAccount;
using Ledgr.Application.Accounts.Queries.GetAccounts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledgr.Api.Controllers;

[ApiController]
[Route("api/accounts")]
[Authorize]
public class AccountsController(IMediator mediator) : ControllerBase
{
    private int OrgId => int.Parse(User.FindFirst("orgId")!.Value);

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(await mediator.Send(new GetAccountsQuery(OrgId), ct));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateAccountCommand cmd, CancellationToken ct)
    {
        var id = await mediator.Send(cmd with { OrgId = OrgId }, ct);
        return CreatedAtAction(nameof(List), new { id });
    }
}
