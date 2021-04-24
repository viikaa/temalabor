using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.BLL;
using Todo.BLL.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Todo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColumnsController : ControllerBase
    {
        readonly IColumnService ColumnService;
        public ColumnsController(IColumnService columnService)
        {
            ColumnService = columnService;
        }

        // GET: api/<ColumnsController>
        [HttpGet]
        public async Task<ActionResult<List<Column>>> GetColumns() 
            => await ColumnService.GetColumnsAsync();

        // GET api/<ColumnsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Column>> GetSingleColumn(int id)
        {
            try
            {
                return await ColumnService.GetSingleColumnAsync(id);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST api/<ColumnsController>
        [HttpPost]
        public async Task<ActionResult<Column>> Post([FromBody] Column column)
        {
            if (!(await ColumnService.IsValidRequestBody(column)))
                return BadRequest();
            if (!await ColumnService.IsColumnUnique(column))
                return Conflict();

            var createdColumn = await ColumnService.InsertColumnAsync(column);
            return CreatedAtAction(
                nameof(GetSingleColumn),
                new { id = createdColumn.Id },
                createdColumn);
        }

        // PUT api/<ColumnsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Column column)
        {
            if (!(await ColumnService.IsValidRequestBody(column, id)))
                return BadRequest();
            if (!await ColumnService.IsColumnUnique(column))
                return Conflict();

            await ColumnService.UpdateColumnAsync(id, column);
            return NoContent();
        }

        // DELETE api/<ColumnsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await ColumnService.DeleteColumnAsync(id);
                return NoContent();
            }
            catch (Exception)
            {
                return NoContent();
            }
        }
    }
}
