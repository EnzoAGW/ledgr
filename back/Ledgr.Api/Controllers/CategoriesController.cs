using Ledgr.Application.Categories.Commands.CreateCategory;
using Ledgr.Application.Categories.Queries.GetCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledgr.Api.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoriesController(IMediator mediator) : ControllerBase
{
    private int OrgId => int.Parse(User.FindFirst("orgId")!.Value);

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(await mediator.Send(new GetCategoriesQuery(OrgId), ct));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand cmd, CancellationToken ct)
    {
        var id = await mediator.Send(cmd with { OrgId = OrgId }, ct);
        return CreatedAtAction(nameof(List), new { id });
    }
}
