using System.Text.Json.Serialization;
using Usuarios.Domain.Entities;

namespace Usuarios.Api.Contracts;

public sealed class UserResponse
{
    public int Id { get; init; }

    public string Nome { get; init; } = string.Empty;

    [JsonPropertyName("dtNascimento")]
    public DateOnly DtNascimento { get; init; }

    public bool Status { get; init; }

    public IReadOnlyList<string> Telefones { get; init; } = [];

    public static UserResponse FromDomain(User user) =>
        new()
        {
            Id = user.Id,
            Nome = user.Nome,
            DtNascimento = user.DtNascimento,
            Status = user.Status,
            Telefones = user.Telefones.ToArray()
        };
}
