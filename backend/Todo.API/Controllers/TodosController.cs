using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        public async Task<ActionResult<List<Todo.BLL.Todo>>> GetTodos()
            => await TodoService.GetTodosAsync();

        // GET api/<TodosController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Todo.BLL.Todo>> GetSingleTodo(int id)
        {
            try
            {
                var todo = await TodoService.GetSingleTodoAsync(id);
                return todo != null ? todo : NotFound();
            }
            catch (Exception e)
            {
                if (e is ArgumentNullException)
                    return Problem("Database not found", null, 500);
                if (e is InvalidOperationException)
                    return Problem("There is more than one todo with the given id.", null, 500);
                else
                    return Problem("Database error", null, 500);
            }

        }

        // POST api/<TodosController>
        [HttpPost]
        public async Task<ActionResult<Column>> InsertTodo([FromBody] Todo.BLL.Todo todo)
        {
            if (!(await TodoService.IsValidRequestBody(todo)))
                return BadRequest();

            if (!(await TodoService.IsTodoUniqueInColumn(todo)))
                return Conflict();

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

            if (!(await TodoService.IsTodoUniqueInColumn(todo)))
                return Conflict();

            await TodoService.UpdateTodoAsync(todo, id);
            return NoContent();
        }

        // DELETE api/<TodosController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await TodoService.DeleteTodoAsync(id))
                return NoContent();
            else
                return Problem("Todo with the given id does not exist, or could not be deleted",
                    null, 404);
        }
    }
}
