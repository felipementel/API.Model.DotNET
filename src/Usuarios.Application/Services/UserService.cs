using Usuarios.Application.Commands;
using Usuarios.Domain.Entities;
using Usuarios.Domain.Errors;
using Usuarios.Domain.Ports;

namespace Usuarios.Application.Services;

public sealed class UserService(IUserRepository repository)
{
    public User CreateUser(SaveUserCommand command)
    {
        if (repository.GetById(command.Id) is not null)
        {
            throw new UserAlreadyExistsException(command.Id);
        }

        var user = ToUser(command);
        return repository.Save(user);
    }

    public IReadOnlyList<User> ListUsers() => repository.ListAll();

    public User GetUser(int userId) =>
        repository.GetById(userId) ?? throw new UserNotFoundException(userId);

    public User UpdateUser(int userId, SaveUserCommand command)
    {
        _ = GetUser(userId);
        var user = ToUser(command, userId);
        return repository.Save(user);
    }

    public void DeleteUser(int userId)
    {
        _ = GetUser(userId);
        repository.Delete(userId);
    }

    private static User ToUser(SaveUserCommand command, int? userId = null) =>
        new(
            userId ?? command.Id,
            command.Nome,
            command.DtNascimento,
            command.Status,
            command.Telefones.ToArray());
}
