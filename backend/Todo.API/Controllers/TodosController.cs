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
    public class TodosController : ControllerBase
    {
        readonly ITodoService TodoService;

        public TodosController(ITodoService todoService)
        {
            TodoService = todoService;
        }

        // GET: api/<TodosController>
        [HttpGet]
        public async Task<ActionResult<List<Todo.BLL.Todo>>> GetTodos([FromQuery] int? columnId)
            => await TodoService.GetTodosAsync(columnId);

        // GET api/<TodosController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Todo.BLL.Todo>> GetSingleTodo(int id)
        {
            try
            {
                return await TodoService.GetSingleTodoAsync(id);
            }
            catch (Exception)
            {
                return NotFound();
            }

        }

        // POST api/<TodosController>
        [HttpPost]
        public async Task<ActionResult<Column>> InsertTodo([FromBody] Todo.BLL.Todo todo)
        {
            if (!(await TodoService.IsValidRequestBody(todo)))
                return BadRequest();
            var createdTodo = await TodoService.InsertTodoAsync(todo);
            return CreatedAtAction(
                nameof(GetSingleTodo),
                new { id = createdTodo.Id },
                createdTodo);
        }

        // PUT api/<TodosController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Todo.BLL.Todo todo)
        {
            if (!(await TodoService.IsValidRequestBody(todo, id)))
                return BadRequest();
            await TodoService.UpdateTodoAsync(todo, id);
            return NoContent();
        }

        // DELETE api/<TodosController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await TodoService.DeleteTodoAsync(id);
                return NoContent();
            }
            catch (Exception)
            {
                return NoContent();
            }
        }
    }
}
