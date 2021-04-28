using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.BLL;
using Todo.BLL.Services;

namespace Todo.Test
{
    public class MockColumnService : IColumnService
    {
        private static readonly List<Column> MockDb = new();
        public Task<List<Column>> GetColumnsAsync()
        {
            return Task.FromResult(MockDb);
        }

        public Task<Column> GetSingleColumnAsync(int Id)
        {
            return Id switch
            {
                1 => Task.FromResult<Column>(null),
                2 => throw new ArgumentNullException(),
                3 => throw new InvalidOperationException(),
                4 => throw new Exception(),
                _ => Task.FromResult(new Column(Id, $"Column{Id}"))
            };
        }

        public Task<Column> InsertColumnAsync(Column column)
        {
            return Task.FromResult(column);
        }
        public Task UpdateColumnAsync(int Id, Column column)
        {
            return Task.CompletedTask;
        }

        public Task<bool> DeleteColumnAsync(int Id) 
            => Id == 1 ? Task.FromResult(true) : Task.FromResult(false);

        public Task<bool> IsColumnUnique(Column column)
        {
            if (column.Id == 1) return Task.FromResult(true);
            else return Task.FromResult(false);
        }

        public Task<bool> IsValidRequestBody(Column column, int? Id = null)
        {
            if (column.Id == 1) return Task.FromResult(true);
            if (column.Id == 3) return Task.FromResult(true);
            else return Task.FromResult(false);
        }

        public static void AddColumnsToMockDb(List<Column> columns)
        {
            MockDb.AddRange(columns);
        }

        public static List<Column> GetMockDb()
        {
            return MockDb;
        }
    }
}
