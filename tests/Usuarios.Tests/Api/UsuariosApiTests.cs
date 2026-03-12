using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Usuarios.Application.Services;

namespace Usuarios.Tests.Api;

public sealed class UsuariosApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public UsuariosApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HealthEndpoints_ShouldReportApplicationStatus()
    {
        using var factory = CreateIsolatedFactory();
        using var client = factory.CreateClient();

        var liveResponse = await client.GetFromJsonAsync<Dictionary<string, string>>("/health/live");
        var readyResponse = await client.GetFromJsonAsync<Dictionary<string, string>>("/health/ready");

        Assert.NotNull(liveResponse);
        Assert.NotNull(readyResponse);
        Assert.Equal("alive", liveResponse["status"]);
        Assert.Equal("ready", readyResponse["status"]);
        Assert.Equal("in-memory", readyResponse["persistence"]);
    }

    [Fact]
    public async Task Readiness_ShouldReturn503_WhenUserServiceIsUnavailable()
    {
        using var factory = CreateIsolatedFactory().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<UserService>();
            });
        });
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health/ready");
        var payload = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal("User service unavailable.", payload["detail"]);
    }

    [Fact]
    public async Task UsuariosCrudFlow_ShouldBehaveLikePythonExample()
    {
        using var factory = CreateIsolatedFactory();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/usuarios", new
        {
            id = 1,
            nome = "Carlos",
            dtNascimento = "1992-03-14",
            status = true,
            telefones = new[] { "11911112222", "1122223333" }
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createdUser = await createResponse.Content.ReadFromJsonAsync<UserResponseDto>();
        Assert.NotNull(createdUser);
        Assert.Equal(1, createdUser.Id);
        Assert.Equal("Carlos", createdUser.Nome);

        var listResponse = await client.GetFromJsonAsync<List<UserResponseDto>>("/usuarios");
        Assert.NotNull(listResponse);
        Assert.Single(listResponse);

        var getResponse = await client.GetFromJsonAsync<UserResponseDto>("/usuarios/1");
        Assert.NotNull(getResponse);
        Assert.Equal("Carlos", getResponse.Nome);

        var updateResponse = await client.PutAsJsonAsync("/usuarios/1", new
        {
            id = 200,
            nome = "Carlos Silva",
            dtNascimento = "1992-03-14",
            status = false,
            telefones = new[] { "11900001111" }
        });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updatedUser = await updateResponse.Content.ReadFromJsonAsync<UserResponseDto>();
        Assert.NotNull(updatedUser);
        Assert.Equal(1, updatedUser.Id);
        Assert.Equal("Carlos Silva", updatedUser.Nome);
        Assert.False(updatedUser.Status);
        Assert.Equal(["11900001111"], updatedUser.Telefones);

        var deleteResponse = await client.DeleteAsync("/usuarios/1");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var missingResponse = await client.GetAsync("/usuarios/1");
        Assert.Equal(HttpStatusCode.NotFound, missingResponse.StatusCode);
    }

    [Fact]
    public async Task DuplicateId_ShouldReturn409()
    {
        using var factory = CreateIsolatedFactory();
        using var client = factory.CreateClient();

        var payload = new
        {
            id = 1,
            nome = "Patricia",
            dtNascimento = "1995-08-10",
            status = true,
            telefones = new[] { "11977776666" }
        };

        var firstResponse = await client.PostAsJsonAsync("/usuarios", payload);
        var duplicateResponse = await client.PostAsJsonAsync("/usuarios", payload);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
    }

    [Fact]
    public async Task UnknownUser_ShouldReturn404_ForGetPutAndDelete()
    {
        using var factory = CreateIsolatedFactory();
        using var client = factory.CreateClient();

        var getResponse = await client.GetAsync("/usuarios/999");
        var putResponse = await client.PutAsJsonAsync("/usuarios/999", new
        {
            id = 999,
            nome = "Inexistente",
            dtNascimento = "2000-01-01",
            status = false,
            telefones = Array.Empty<string>()
        });
        var deleteResponse = await client.DeleteAsync("/usuarios/999");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }

    private WebApplicationFactory<Program> CreateIsolatedFactory() =>
        _factory.WithWebHostBuilder(_ => { });

    private sealed class UserResponseDto
    {
        public int Id { get; init; }

        public string Nome { get; init; } = string.Empty;

        public string DtNascimento { get; init; } = string.Empty;

        public bool Status { get; init; }

        public string[] Telefones { get; init; } = [];
    }
}
