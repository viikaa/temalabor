using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Todo.BLL.Services;
using Todo.DAL;

namespace Todo.BLL
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddTodoDbContext(this IServiceCollection services, string connectionString) =>
            services.AddDbContext<TodoDbContext>(options => options.UseSqlServer(connectionString));

        public static IServiceCollection AddColumnService(this IServiceCollection services) =>
            services.AddScoped<IColumnService, ColumnService>();

        public static IServiceCollection AddTodoService(this IServiceCollection services) =>
            services.AddScoped<ITodoService, TodoService>();
    }
}
