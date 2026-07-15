using Ledgr.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ledgr.Application.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler(ILedgrDbContext db) : IRequestHandler<GetCategoriesQuery, IList<CategoryDto>>
{
    public async Task<IList<CategoryDto>> Handle(GetCategoriesQuery req, CancellationToken ct)
    {
        return await db.Categories
            .Where(c => c.OrgId == req.OrgId)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Type, c.Color))
            .ToListAsync(ct);
    }
}
