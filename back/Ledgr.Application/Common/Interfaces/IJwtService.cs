using Ledgr.Domain.Entities;

namespace Ledgr.Application.Common.Interfaces;

public interface IJwtService
{
    string Generate(User user);
}
