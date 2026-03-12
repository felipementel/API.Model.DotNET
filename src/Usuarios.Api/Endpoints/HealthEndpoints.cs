using Microsoft.AspNetCore.Mvc;
using Usuarios.Application.Services;

namespace Usuarios.Api.Endpoints;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/health").WithTags("Health");

        group.MapGet("/live", () => Results.Ok(new { status = "alive" }))
            .Produces(StatusCodes.Status200OK);

        group.MapGet("/ready", ([FromServices] IServiceProvider serviceProvider) =>
        {
            var userService = serviceProvider.GetService<UserService>();
            if (userService is null)
            {
                return Results.Json(
                    new { detail = "User service unavailable." },
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }

            return Results.Ok(new { status = "ready", persistence = "in-memory" });
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status503ServiceUnavailable);

        return app;
    }
}
