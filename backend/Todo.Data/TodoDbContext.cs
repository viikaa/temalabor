using Microsoft.EntityFrameworkCore;

namespace Todo.DAL
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }  
        public DbSet<Todo> Todos { get; set; }
        public DbSet<Column> Columns { get; set; }
    }
}
