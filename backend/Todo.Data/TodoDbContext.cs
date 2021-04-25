using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.DAL
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Column>().HasData(
            //    new Column { Id = 1, Title = "Első" },
            //    new Column { Id = 2, Title = "Második" }
            //);

            //modelBuilder.Entity<Todo>().HasData(
            //    new Todo { 
            //        Id = 1,
            //        Title = "egyeske",
            //        Description = "lol",
            //        ColumnId = 1 ,
            //        Deadline = DateTime.Now },
            //    new Todo {
            //        Id = 2,
            //        Title = "ketteske",
            //        Description = "xd",
            //        ColumnId = 2 ,
            //        Deadline = DateTime.Now }
            //);
        }
        
        public DbSet<Todo> Todos { get; set; }
        public DbSet<Column> Columns { get; set; }
    }
}
