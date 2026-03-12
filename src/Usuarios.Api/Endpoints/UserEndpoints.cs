using Microsoft.AspNetCore.Mvc;
using Usuarios.Api.Contracts;
using Usuarios.Application.Services;
using Usuarios.Domain.Errors;

namespace Usuarios.Api.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/usuarios").WithTags("Usuarios");

        group.MapPost(string.Empty, ([FromBody] UserRequest request, [FromServices] UserService userService) =>
        {
            try
            {
                var user = userService.CreateUser(request.ToCommand());
                return Results.Json(
                    UserResponse.FromDomain(user),
                    statusCode: StatusCodes.Status201Created);
            }
            catch (UserAlreadyExistsException error)
            {
                return Results.Conflict(new { detail = error.Message });
            }
        })
        .Produces<UserResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict);

        group.MapGet(string.Empty, ([FromServices] UserService userService) =>
        {
            var users = userService.ListUsers().Select(UserResponse.FromDomain).ToArray();
            return Results.Ok(users);
        })
        .Produces<IReadOnlyList<UserResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{usuarioId:int}", (int usuarioId, [FromServices] UserService userService) =>
        {
            try
            {
                var user = userService.GetUser(usuarioId);
                return Results.Ok(UserResponse.FromDomain(user));
            }
            catch (UserNotFoundException error)
            {
                return Results.NotFound(new { detail = error.Message });
            }
        })
        .Produces<UserResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{usuarioId:int}", (int usuarioId, [FromBody] UserRequest request, [FromServices] UserService userService) =>
        {
            try
            {
                var user = userService.UpdateUser(usuarioId, request.ToCommand());
                return Results.Ok(UserResponse.FromDomain(user));
            }
            catch (UserNotFoundException error)
            {
                return Results.NotFound(new { detail = error.Message });
            }
        })
        .Produces<UserResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{usuarioId:int}", (int usuarioId, [FromServices] UserService userService) =>
        {
            try
            {
                userService.DeleteUser(usuarioId);
                return Results.NoContent();
            }
            catch (UserNotFoundException error)
            {
                return Results.NotFound(new { detail = error.Message });
            }
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
