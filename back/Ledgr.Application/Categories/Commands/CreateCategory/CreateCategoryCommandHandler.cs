using Ledgr.Application.Common.Interfaces;
using Ledgr.Domain.Entities;
using MediatR;

namespace Ledgr.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler(ILedgrDbContext db) : IRequestHandler<CreateCategoryCommand, int>
{
    public async Task<int> Handle(CreateCategoryCommand req, CancellationToken ct)
    {
        var cat = new Category { OrgId = req.OrgId, Name = req.Name, Type = req.Type, Color = req.Color };
        db.Categories.Add(cat);
        await db.SaveChangesAsync(ct);
        return cat.Id;
    }
}
