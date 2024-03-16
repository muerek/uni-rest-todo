using Microsoft.EntityFrameworkCore;

static class TodosEndpoints
{
    public static WebApplication MapTodosEndpoints(this WebApplication app)
    {
        var group = app
            .MapGroup("/todos")
            .WithOpenApi();

        group.MapGet("/", GetAllTodos);
        group.MapGet("/completed", GetCompletedTodos);
        group.MapGet("/open", GetOpenTodos);
        group.MapGet("/{id:int}", GetTodo);
        group.MapPost("/", CreateTodo);
        group.MapPut("/{id:int}", UpdateTodo);
        group.MapDelete("/{id:int}", DeleteTodo);

        return app;
    }

    static async Task<IResult> GetAllTodos(TodoDb db)
    {
        var todos = await db.Todos
            .Select(t => new TodoSummaryDto(t.Id, t.Name, t.Completed))
            .ToListAsync();

        return TypedResults.Ok(todos);
    }

    static async Task<IResult> GetCompletedTodos(TodoDb db)
    {
        var todos = await db.Todos
            .Where(t => t.Completed)
            .Select(t => new TodoSummaryDto(t.Id, t.Name, t.Completed))
            .ToListAsync();

        return TypedResults.Ok(todos);
    }

    static async Task<IResult> GetOpenTodos(TodoDb db)
    {
        var todos = await db.Todos
            .Where(t => !t.Completed)
            .Select(t => new TodoSummaryDto(t.Id, t.Name, t.Completed))
            .ToListAsync();

        return TypedResults.Ok(todos);
    }

    static async Task<IResult> GetTodo(int id, TodoDb db) =>
        await db.Todos.FindAsync(id) is Todo todo
            ? TypedResults.Ok(todo)
            : TypedResults.NotFound();

    static async Task<IResult> CreateTodo(TodoCreateDto input, TodoDb db)
    {
        var todo = new Todo()
        {
            Name = input.Name,
            Description = input.Description
        };

        db.Todos.Add(todo);
        await db.SaveChangesAsync();

        return TypedResults.Created($"/todos/{todo.Id}", todo);
    }

    static async Task<IResult> UpdateTodo(int id, TodoUpdateDto update, TodoDb db)
    {
        var todo = await db.Todos.FindAsync(id);

        if (todo == null) return TypedResults.NotFound();

        todo.Name = update.Name;
        todo.Description = update.Description;
        todo.Completed = update.Completed;

        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    static async Task<IResult> DeleteTodo(int id, TodoDb db)
    {
        var todo = await db.Todos.FindAsync(id);

        if (todo == null) return TypedResults.NotFound();

        db.Todos.Remove(todo);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}