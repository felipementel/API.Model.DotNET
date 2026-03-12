namespace Usuarios.Domain.Errors;

public sealed class UserNotFoundException(int userId)
    : Exception($"Usuario com id {userId} nao foi encontrado.");
