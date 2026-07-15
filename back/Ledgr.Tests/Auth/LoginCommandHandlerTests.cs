using FluentAssertions;
using Ledgr.Application.Auth.Commands.Login;
using Ledgr.Application.Common.Exceptions;
using Ledgr.Application.Common.Interfaces;
using Ledgr.Domain.Entities;
using Ledgr.Domain.Enums;
using Ledgr.Tests.Helpers;
using Moq;

namespace Ledgr.Tests.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IJwtService> _jwt = new();
    private readonly Mock<IPasswordHasher> _hasher = new();

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsLoginResult()
    {
        using var db = DbContextFactory.Create();
        db.Organizations.Add(new Organization { Id = 1, Name = "Org" });
        db.Users.Add(new User { Id = 1, OrgId = 1, Email = "user@test.com", PasswordHash = "hashed", Name = "User", Role = UserRole.Admin });
        await db.SaveChangesAsync();

        _hasher.Setup(h => h.Verify("secret", "hashed")).Returns(true);
        _jwt.Setup(j => j.Generate(It.IsAny<User>())).Returns("token123");

        var handler = new LoginCommandHandler(db, _jwt.Object, _hasher.Object);
        var result = await handler.Handle(new LoginCommand("user@test.com", "secret"), CancellationToken.None);

        result.Token.Should().Be("token123");
        result.UserId.Should().Be(1);
        result.Name.Should().Be("User");
        result.Role.Should().Be(UserRole.Admin);
        result.OrgId.Should().Be(1);
    }

    [Fact]
    public async Task Handle_EmailNotFound_ThrowsUnauthorized()
    {
        using var db = DbContextFactory.Create();
        var handler = new LoginCommandHandler(db, _jwt.Object, _hasher.Object);

        await handler.Invoking(h => h.Handle(new LoginCommand("nobody@test.com", "pass"), CancellationToken.None))
            .Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsUnauthorized()
    {
        using var db = DbContextFactory.Create();
        db.Organizations.Add(new Organization { Id = 1, Name = "Org" });
        db.Users.Add(new User { Id = 1, OrgId = 1, Email = "user@test.com", PasswordHash = "hashed", Name = "User", Role = UserRole.Analyst });
        await db.SaveChangesAsync();

        _hasher.Setup(h => h.Verify("wrongpass", "hashed")).Returns(false);

        var handler = new LoginCommandHandler(db, _jwt.Object, _hasher.Object);
        await handler.Invoking(h => h.Handle(new LoginCommand("user@test.com", "wrongpass"), CancellationToken.None))
            .Should().ThrowAsync<UnauthorizedException>();
    }
}
