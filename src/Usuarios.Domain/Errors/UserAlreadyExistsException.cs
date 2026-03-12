namespace Usuarios.Domain.Errors;

public sealed class UserAlreadyExistsException(int userId)
    : Exception($"Usuario com id {userId} ja existe.");
