using System;

namespace Todo.DAL
{
    public class Todo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public int ColumnId { get; set; }
        public DateTime Deadline { get; set; }
        public Column Column { get; set; }
    }
}
