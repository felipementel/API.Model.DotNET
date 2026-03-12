using Usuarios.Adapters.Repositories;
using Usuarios.Api.Endpoints;
using Usuarios.Application.Services;
using Usuarios.Domain.Ports;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<UserService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthEndpoints();
app.MapUserEndpoints();

await app.RunAsync();

public partial class Program
{
    protected Program()
    {
    }
}
