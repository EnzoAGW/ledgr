using Ledgr.Application.Dashboard.Queries.GetDashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledgr.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var orgId = int.Parse(User.FindFirst("orgId")!.Value);
        return Ok(await mediator.Send(new GetDashboardQuery(orgId), ct));
    }
}
