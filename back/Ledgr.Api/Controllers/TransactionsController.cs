using Ledgr.Application.Transactions.Commands.CreateTransaction;
using Ledgr.Application.Transactions.Commands.UpdateTransactionStatus;
using Ledgr.Application.Transactions.Queries.GetTransactions;
using Ledgr.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledgr.Api.Controllers;

[ApiController]
[Route("api/transactions")]
[Authorize]
public class TransactionsController(IMediator mediator) : ControllerBase
{
    private int OrgId => int.Parse(User.FindFirst("orgId")!.Value);
    private int UserId => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] string? search,
        [FromQuery] int? accountId,
        [FromQuery] int? categoryId,
        [FromQuery] TransactionType? type,
        [FromQuery] TransactionStatus? status,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(
            new GetTransactionsQuery(OrgId, search, accountId, categoryId, type, status, from, to, page, pageSize), ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateTransactionCommand cmd, CancellationToken ct)
    {
        var id = await mediator.Send(cmd with { OrgId = OrgId }, ct);
        return CreatedAtAction(nameof(List), new { id });
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest req, CancellationToken ct)
    {
        await mediator.Send(new UpdateTransactionStatusCommand(id, OrgId, req.Status), ct);
        return NoContent();
    }
}

public record UpdateStatusRequest(TransactionStatus Status);
