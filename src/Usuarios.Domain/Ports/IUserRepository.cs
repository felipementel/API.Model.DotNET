using Usuarios.Domain.Entities;

namespace Usuarios.Domain.Ports;

public interface IUserRepository
{
    User Save(User user);

    IReadOnlyList<User> ListAll();

    User? GetById(int userId);

    void Delete(int userId);
}
