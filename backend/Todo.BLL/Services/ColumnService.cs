using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using Todo.DAL;

namespace Todo.BLL.Services
{
    public interface IColumnService
    {
        Task<List<Column>> GetColumnsAsync();
        Task<Column> GetSingleColumnAsync(int Id);
        Task<Column> InsertColumnAsync(Column column);
        Task UpdateColumnAsync(int Id, Column column);
        Task DeleteColumnAsync(int Id);
        Task<bool> IsValidRequestBody(Column column, int? Id = null);
    }
    public class ColumnService : IColumnService
    {
        readonly TodoDbContext _context;
        public ColumnService(TodoDbContext context)
        {
            _context = context;
        }


        public async Task<List<Column>> GetColumnsAsync()
        {
            return await _context.Columns
                .Select(c => new Column(c.Id, c.Title))
                .ToListAsync();
        }
        public async Task<Column> GetSingleColumnAsync(int Id)
        {
            return await _context.Columns
                .Where(c => c.Id == Id)
                .Select(c => new Column(c.Id, c.Title))
                .SingleAsync();
        }

        public async Task<Column> InsertColumnAsync(Column column)
        {
            DAL.Column newColumn = new() { Title = column.Title };
            _context.Columns.Add(newColumn);
            await _context.SaveChangesAsync();
            return await GetSingleColumnAsync(newColumn.Id);
        }

        public async Task UpdateColumnAsync(int Id, Column column)
        {
            var updatedColumn = new DAL.Column { Id = Id, Title = column.Title };
            var entry = _context.Columns.Attach(updatedColumn);
            entry.State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteColumnAsync(int Id)
        {
            _context.Columns.Remove(new DAL.Column { Id = Id });
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsValidRequestBody(Column column, int? Id = null)
        {
            var isIdCorrect = column.Id == Id;
            var doesIdExist = column.Id == null || await _context.Columns.AnyAsync(c => c.Id == column.Id);
            var isColumnUnique = !(await _context.Columns.AnyAsync(c => c.Title == column.Title && c.Id != column.Id));

            return isIdCorrect && doesIdExist && isColumnUnique;

        }
    }
}
