using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.DAL
{
    public class Column
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<Todo> Todos { get; } = new List<Todo>();
    }
}
