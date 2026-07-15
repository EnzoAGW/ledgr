using MediatR;

namespace Ledgr.Application.Auth.Commands.RegisterOrg;

public record RegisterOrgCommand(string OrgName, string AdminName, string Email, string Password) : IRequest<int>;
