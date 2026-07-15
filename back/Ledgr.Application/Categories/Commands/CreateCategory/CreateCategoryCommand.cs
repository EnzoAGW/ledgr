using Ledgr.Domain.Enums;
using MediatR;

namespace Ledgr.Application.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(int OrgId, string Name, CategoryType Type, string Color) : IRequest<int>;
