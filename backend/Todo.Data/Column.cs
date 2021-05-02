using System;
using System.Collections.Generic;

namespace Todo.DAL
{
    public class Column : IEquatable<Column>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<Todo> Todos { get; } = new List<Todo>();
        public bool Equals(Column other)
        {
            return other != null && Id == other.Id && Title == other.Title;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Column);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Title);
        }
    }
}
