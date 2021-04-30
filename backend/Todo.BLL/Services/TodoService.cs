using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Todo.DAL;
using System.Globalization;

namespace Todo.BLL.Services
{
    public interface ITodoService
    {
        Task<List<Todo>> GetTodosAsync();
        Task<Todo> GetSingleTodoAsync(int Id);
        Task<Todo> InsertTodoAsync(Todo todo);
        Task UpdateTodoAsync(Todo todo, int Id);
        Task<bool> DeleteTodoAsync(int Id);
        Task<bool> IsValidRequestBody(Todo todo, int? Id = null);
        Task<bool> IsTodoUniqueInColumn(Todo todo);
    }
    public class TodoService : ITodoService
    {
        readonly TodoDbContext _context;
        public TodoService(TodoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Todo>> GetTodosAsync()
        {
            var todos = _context.Todos.AsQueryable();
            return await todos
                .OrderBy(t => t.Priority)
                .Select(t => new Todo(
                    t.Id,
                    t.Title,
                    t.Description,
                    t.Deadline.ToString("yyyy-MM-ddTHH:mm", null),
                    t.Priority,
                    t.ColumnId))
                .ToListAsync();
        }

        public async Task<Todo> GetSingleTodoAsync(int Id)
        {
            return await _context.Todos
                .Where(t => t.Id == Id)
                .Select(t => new Todo(
                    t.Id,
                    t.Title,
                    t.Description,
                    t.Deadline.ToString("yyyy-MM-ddTHH:mm", null),
                    t.Priority,
                    t.ColumnId))
                .SingleOrDefaultAsync();
        }

        public async Task<Todo> InsertTodoAsync(Todo todo)
        {
            DAL.Todo newTodo = new()
            {
                Title = todo.Title,
                Description = todo.Description,
                Deadline = DateTime.ParseExact(todo.Deadline, "yyyy-MM-ddTHH:mm", null),
                Priority = todo.Priority,
                ColumnId = todo.ColumnId
            };
            _context.Todos.Add(newTodo);
            await _context.SaveChangesAsync();
            return await GetSingleTodoAsync(newTodo.Id);
        }

        public async Task UpdateTodoAsync(Todo todo, int Id)
        {
            DAL.Todo newTodo = new()
            {
                Id = Id,
                Title = todo.Title,
                Description = todo.Description,
                Deadline = DateTime.ParseExact(todo.Deadline, "yyyy-MM-ddTHH:mm", null),
                Priority = todo.Priority,
                ColumnId = todo.ColumnId
            };
            var entry = _context.Todos.Attach(newTodo);
            entry.State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteTodoAsync(int Id)
        {
            _context.Todos.Remove(new DAL.Todo { Id = Id });
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e) when (
                e is DbUpdateConcurrencyException ||
                e is DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool> IsValidRequestBody(Todo todo, int? Id = null)
        {
            var isIdCorrect = todo.Id == Id;
            var doesIdExistIfNotNull = todo.Id == null || await _context.Todos.AnyAsync(t => t.Id == todo.Id);
            var isPriorityPositive = todo.Priority >= 0;
            var isDeadlineInCorrectFormat = DateTime.TryParseExact(todo.Deadline, "yyyy-MM-ddTHH:mm", null, DateTimeStyles.None, out _);
            var doesColumnExist = await _context.Columns.AnyAsync(c => c.Id == todo.ColumnId);

            return isIdCorrect &&
                   doesIdExistIfNotNull &&
                   isPriorityPositive &&
                   isDeadlineInCorrectFormat &&
                   doesColumnExist;
        }

        public async Task<bool> IsTodoUniqueInColumn(Todo todo)
            => !await _context.Todos.AnyAsync(t => t.Title == todo.Title &&
                                              t.ColumnId == todo.ColumnId &&
                                              t.Id != todo.Id);
    }
}
