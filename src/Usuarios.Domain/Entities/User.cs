namespace Usuarios.Domain.Entities;

public sealed record User(
    int Id,
    string Nome,
    DateOnly DtNascimento,
    bool Status,
    IReadOnlyList<string> Telefones);
