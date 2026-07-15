using Ledgr.Domain.Enums;
using MediatR;

namespace Ledgr.Application.Categories.Queries.GetCategories;

public record GetCategoriesQuery(int OrgId) : IRequest<IList<CategoryDto>>;

public record CategoryDto(int Id, string Name, CategoryType Type, string Color);
