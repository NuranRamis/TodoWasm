using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Models;

namespace FileData.DAOs;

public class TodoFileDao : ITodoDao
{
    private readonly FileContext context;

    public TodoFileDao(FileContext context)
    {
        this.context = context;
    }

    public Task<Todo> CreateAsync(Todo todo)
    {
        int id = 1;
        if (context.Todos.Any())
        {
            id = context.Todos.Max(t => t.Id);
            id++;
        }

        todo.Id = id;
        
        context.Todos.Add(todo);
        context.SaveChanges();

        return Task.FromResult(todo);
    }

    public Task<IEnumerable<Todo>> GetAsync(SearchTodoParametersDto searchParameters)
    {
        IEnumerable<Todo> result = context.Todos.AsEnumerable();
        if (!string.IsNullOrEmpty(searchParameters.Username))
        {
            //we know username is unique, so fetch the first
            result = context.Todos.Where(todo =>
                todo.Owner.UserName.Equals(searchParameters.Username, StringComparison.OrdinalIgnoreCase));
        }

        if (searchParameters.UserId != null)
        {
            result = result.Where(t => t.Owner.Id == searchParameters.UserId);
        }

        if (searchParameters.CompletedStatus !=null)
        {
            result = result.Where(t => t.IsCompleted == searchParameters.CompletedStatus);
        }

        if (!string.IsNullOrEmpty(searchParameters.TitleContains))
        {
            result = result.Where(t =>
                t.Title.Equals(searchParameters.TitleContains, StringComparison.OrdinalIgnoreCase));
        }
        return Task.FromResult(result);
    }
}