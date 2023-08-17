using Microsoft.AspNetCore.Mvc;
using TodoLibrary.DataAccess;

namespace MinimalApi.Endpoints;

public static class TodoEndpoints
{
    // Extension method (this WebAppliction app)
    public static void AddTodoEndpoints(this WebApplication app)
    {
        app.MapGet("/api/Todos", async (ITodoData data) =>
        {
            var output = await data.GetAllAssigned(1);
            return Results.Ok(output);
        });

        app.MapPost("/api/Todos", async (ITodoData data, [FromBody] string task) =>
        {
            var output = await data.Create(1, task);
            return Results.Ok(output);
        });

        app.MapDelete("/api/Todos/{id}", async (ITodoData data, int id) =>
        {
            await data.DeleteTodo(1, id);
            return Results.Ok();
        });
    }
}
