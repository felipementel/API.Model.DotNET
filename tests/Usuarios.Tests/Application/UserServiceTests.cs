using Usuarios.Adapters.Repositories;
using Usuarios.Application.Commands;
using Usuarios.Application.Services;
using Usuarios.Domain.Errors;

namespace Usuarios.Tests.Application;

public sealed class UserServiceTests
{
    [Fact]
    public void CreateUserAndListUsers_ShouldPersistUserInMemory()
    {
        var service = new UserService(new InMemoryUserRepository());

        var createdUser = service.CreateUser(MakeCommand());
        var users = service.ListUsers();

        Assert.Equal(1, createdUser.Id);
        Assert.Equal("Maria", createdUser.Nome);
        Assert.Single(users);
        Assert.Equal(["11999990000", "1133334444"], users[0].Telefones);
    }

    [Fact]
    public void CreateUser_WithExistingId_ShouldThrowException()
    {
        var service = new UserService(new InMemoryUserRepository());
        var command = MakeCommand();

        service.CreateUser(command);

        Assert.Throws<UserAlreadyExistsException>(() => service.CreateUser(command));
    }

    [Fact]
    public void UpdateUser_ShouldKeepRouteIdAndReplaceOtherFields()
    {
        var service = new UserService(new InMemoryUserRepository());
        service.CreateUser(MakeCommand());

        var updatedUser = service.UpdateUser(
            1,
            new SaveUserCommand(
                99,
                "Ana",
                new DateOnly(1988, 5, 20),
                false,
                ["11888887777"]));

        Assert.Equal(1, updatedUser.Id);
        Assert.Equal("Ana", updatedUser.Nome);
        Assert.False(updatedUser.Status);
        Assert.Equal(["11888887777"], updatedUser.Telefones);
    }

    [Fact]
    public void DeleteUnknownUser_ShouldThrowException()
    {
        var service = new UserService(new InMemoryUserRepository());

        Assert.Throws<UserNotFoundException>(() => service.DeleteUser(999));
    }

    private static SaveUserCommand MakeCommand(int userId = 1, string nome = "Maria") =>
        new(
            userId,
            nome,
            new DateOnly(1990, 1, 10),
            true,
            ["11999990000", "1133334444"]);
}
