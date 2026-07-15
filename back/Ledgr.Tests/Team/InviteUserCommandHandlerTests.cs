using FluentAssertions;
using Ledgr.Application.Common.Exceptions;
using Ledgr.Application.Common.Interfaces;
using Ledgr.Application.Team.Commands.InviteUser;
using Ledgr.Domain.Entities;
using Ledgr.Domain.Enums;
using Ledgr.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Ledgr.Tests.Team;

public class InviteUserCommandHandlerTests
{
    private readonly Mock<IPasswordHasher> _hasher = new();

    public InviteUserCommandHandlerTests()
    {
        _hasher.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed");
    }

    private static InviteUserCommand Cmd(UserRole inviterRole, UserRole role, string email = "new@test.com") =>
        new(OrgId: 1, InviterRole: inviterRole, Name: "New User", Email: email, Password: "pass", Role: role);

    [Theory]
    [InlineData(UserRole.Admin,   UserRole.Admin)]
    [InlineData(UserRole.Admin,   UserRole.Manager)]
    [InlineData(UserRole.Admin,   UserRole.Analyst)]
    [InlineData(UserRole.Manager, UserRole.Analyst)]
    public async Task Handle_AllowedCombinations_CreatesUser(UserRole inviter, UserRole target)
    {
        using var db = DbContextFactory.Create();
        db.Organizations.Add(new Organization { Id = 1, Name = "Org" });
        await db.SaveChangesAsync();

        var handler = new InviteUserCommandHandler(db, _hasher.Object);
        var id = await handler.Handle(Cmd(inviter, target), CancellationToken.None);

        id.Should().BeGreaterThan(0);
        (await db.Users.CountAsync()).Should().Be(1);
    }

    [Theory]
    [InlineData(UserRole.Manager, UserRole.Manager)]
    [InlineData(UserRole.Manager, UserRole.Admin)]
    public async Task Handle_ManagerInvitingNonAnalyst_ThrowsForbidden(UserRole inviter, UserRole target)
    {
        using var db = DbContextFactory.Create();
        var handler = new InviteUserCommandHandler(db, _hasher.Object);

        await handler.Invoking(h => h.Handle(Cmd(inviter, target), CancellationToken.None))
            .Should().ThrowAsync<ForbiddenException>();
    }

    [Theory]
    [InlineData(UserRole.Analyst, UserRole.Analyst)]
    [InlineData(UserRole.Analyst, UserRole.Manager)]
    [InlineData(UserRole.Analyst, UserRole.Admin)]
    public async Task Handle_AnalystInviting_ThrowsForbidden(UserRole inviter, UserRole target)
    {
        using var db = DbContextFactory.Create();
        var handler = new InviteUserCommandHandler(db, _hasher.Object);

        await handler.Invoking(h => h.Handle(Cmd(inviter, target), CancellationToken.None))
            .Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsConflict()
    {
        using var db = DbContextFactory.Create();
        db.Organizations.Add(new Organization { Id = 1, Name = "Org" });
        db.Users.Add(new User { Id = 1, OrgId = 1, Email = "existing@test.com", Name = "Existing", Role = UserRole.Analyst, PasswordHash = "h" });
        await db.SaveChangesAsync();

        var handler = new InviteUserCommandHandler(db, _hasher.Object);

        await handler.Invoking(h => h.Handle(Cmd(UserRole.Admin, UserRole.Analyst, "existing@test.com"), CancellationToken.None))
            .Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Handle_ValidInvite_PasswordIsHashed()
    {
        using var db = DbContextFactory.Create();
        db.Organizations.Add(new Organization { Id = 1, Name = "Org" });
        await db.SaveChangesAsync();

        var handler = new InviteUserCommandHandler(db, _hasher.Object);
        await handler.Handle(Cmd(UserRole.Admin, UserRole.Analyst), CancellationToken.None);

        var user = await db.Users.FirstAsync();
        user.PasswordHash.Should().Be("hashed");
        _hasher.Verify(h => h.Hash("pass"), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidInvite_SetsCorrectOrgAndRole()
    {
        using var db = DbContextFactory.Create();
        db.Organizations.Add(new Organization { Id = 1, Name = "Org" });
        await db.SaveChangesAsync();

        var handler = new InviteUserCommandHandler(db, _hasher.Object);
        await handler.Handle(
            new InviteUserCommand(OrgId: 1, InviterRole: UserRole.Admin, Name: "Alice", Email: "alice@test.com", Password: "pass", Role: UserRole.Manager),
            CancellationToken.None);

        var user = await db.Users.FirstAsync();
        user.OrgId.Should().Be(1);
        user.Role.Should().Be(UserRole.Manager);
        user.Name.Should().Be("Alice");
    }
}
