namespace Usuarios.Application.Commands;

public sealed record SaveUserCommand(
    int Id,
    string Nome,
    DateOnly DtNascimento,
    bool Status,
    IReadOnlyList<string> Telefones);
