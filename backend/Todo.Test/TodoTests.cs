using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Test
{
    [TestClass]
    public class TodoTests
    {
        private static readonly DAL.Column[] TestColumns = new[]
        {
            new DAL.Column { Id = 1, Title = "Column1" },
            new DAL.Column { Id = 2, Title = "Column2" }
        };

        private static readonly DAL.Todo[] TestTodos = new[]
        {
            new DAL.Todo
            {
                Id = 1,
                Title = "Todo1",
                Description = "Desc1",
                Priority = 0,
                ColumnId = 1,
                Deadline = DateTime.ParseExact("2021-01-01T12:00", "yyyy-MM-ddTHH:mm", null)
            },
            new DAL.Todo
            {
                Id = 2,
                Title = "Todo2",
                Description = "Desc2",
                Priority = 0,
                ColumnId = 1,
                Deadline = DateTime.ParseExact("2021-01-01T12:00", "yyyy-MM-ddTHH:mm", null)
            },
            new DAL.Todo
            {
                Id = 3,
                Title = "Todo3",
                Description = "Desc3",
                Priority = 0,
                ColumnId = 2,
                Deadline = DateTime.ParseExact("2021-01-01T12:00", "yyyy-MM-ddTHH:mm", null)
            }
        };

        private static BLL.Todo MapEntityToDto(DAL.Todo entity)
            => new BLL.Todo(
                entity.Id,
                entity.Title,
                entity.Description,
                entity.Deadline.ToString("yyyy-MM-ddTHH:mm", null),
                entity.Priority,
                entity.ColumnId);

        private static DAL.Todo MapDtoToEntity(BLL.Todo dto)
            => new DAL.Todo 
            {
                Id = dto.Id ?? 0,
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                ColumnId = dto.ColumnId,
                Deadline = DateTime.ParseExact(dto.Deadline, "yyyy-MM-ddTHH:mm", null)
            };

        [TestMethod]
        public async Task GetTodosWhenEmpty()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                var client = testScope.CreateClient();
                var response = await client.GetAsync("/api/todos");

                response.EnsureSuccessStatusCode();
                var actual = await response.Content.ReadFromJsonAsync<BLL.Todo[]>();

                Assert.IsNotNull(actual);
                Assert.AreEqual(0, actual.Length);
            }
        }

        [TestMethod]
        public async Task GetColumnsWithData()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                testScope.AddSeedEntities(TestColumns);
                testScope.AddSeedEntities(TestTodos);
                var client = testScope.CreateClient();

                var response = await client.GetAsync("/api/todos");
                response.EnsureSuccessStatusCode();
                var actual = await response.Content.ReadFromJsonAsync<BLL.Todo[]>();

                DAL.Todo[] actualEntities = new DAL.Todo[TestTodos.Length];
                for (int i = 0; i < TestTodos.Length; i++)
                {
                    actualEntities[i] = MapDtoToEntity(actual[i]);
                }

                Assert.IsNotNull(actual);
                CollectionAssert.AreEquivalent(TestTodos, actualEntities);
            }
        }

        [TestMethod]
        public async Task GetSingleTodoWhenDoesNotExist()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                testScope.AddSeedEntities(TestColumns);
                testScope.AddSeedEntities(TestTodos);
                var client = testScope.CreateClient();

                int id = 4;
                var response = await client.GetAsync($"/api/todos/{id}");

                Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task GetSingleTodoWhenExists()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                testScope.AddSeedEntities(TestColumns);
                testScope.AddSeedEntities(TestTodos);
                int id = 1;
                var client = testScope.CreateClient();
                var response = await client.GetAsync($"/api/todos/{id}");

                response.EnsureSuccessStatusCode();
                var actual = await response.Content.ReadFromJsonAsync<BLL.Todo>();
                var actualEntity = MapDtoToEntity(actual);

                Assert.IsNotNull(actual);
                Assert.AreEqual(TestTodos.Single(t => t.Id == id), actualEntity);
            }
        }


    }
}
