using System;

namespace Todo.DAL
{
    public class Todo : IEquatable<Todo>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public int ColumnId { get; set; }
        public DateTime Deadline { get; set; }
        public Column Column { get; set; }

        public bool Equals(Todo other)
        {
            return other != null &&
                   Id == other.Id &&
                   Title == other.Title &&
                   Description == other.Description &&
                   Priority == other.Priority &&
                   ColumnId == other.ColumnId &&
                   Deadline == other.Deadline;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Todo);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Title, Description, Priority, ColumnId, Deadline);
        }
    }
}
