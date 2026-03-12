using Usuarios.Domain.Entities;
using Usuarios.Domain.Ports;

namespace Usuarios.Adapters.Repositories;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly Dictionary<int, User> _storage = [];
    private readonly Lock _lock = new();

    public User Save(User user)
    {
        var storedUser = Clone(user);

        lock (_lock)
        {
            _storage[user.Id] = storedUser;
        }

        return Clone(storedUser);
    }

    public IReadOnlyList<User> ListAll()
    {
        lock (_lock)
        {
            return _storage.Values.Select(Clone).ToArray();
        }
    }

    public User? GetById(int userId)
    {
        lock (_lock)
        {
            return _storage.TryGetValue(userId, out var user) ? Clone(user) : null;
        }
    }

    public void Delete(int userId)
    {
        lock (_lock)
        {
            _storage.Remove(userId);
        }
    }

    private static User Clone(User user) =>
        user with
        {
            Telefones = user.Telefones.ToArray()
        };
}
