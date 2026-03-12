using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Usuarios.Application.Commands;

namespace Usuarios.Api.Contracts;

public sealed class UserRequest
{
    [Range(1, int.MaxValue)]
    public int Id { get; init; }

    [Required]
    [StringLength(120, MinimumLength = 1)]
    public string Nome { get; init; } = string.Empty;

    [JsonPropertyName("dtNascimento")]
    public DateOnly DtNascimento { get; init; }

    public bool Status { get; init; }

    public IReadOnlyList<string> Telefones { get; init; } = [];

    public SaveUserCommand ToCommand() =>
        new(Id, Nome, DtNascimento, Status, Telefones.ToArray());
}
