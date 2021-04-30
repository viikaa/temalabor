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
        public async Task GetTodosWithData()
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

        [TestMethod]
        public async Task PostTodoWithId()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                var client = testScope.CreateClient();

                var response = await client.PostAsJsonAsync("/api/todos", 
                    new BLL.Todo(
                        4,
                        "TestPost",
                        "Desc",
                        "2021-04-30T10:54",
                        0, 
                        1)
                    );

                Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task PostTodoWithNegativePriority()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                var client = testScope.CreateClient();

                var response = await client.PostAsJsonAsync("/api/todos",
                    new BLL.Todo(
                        null,
                        "TestPost",
                        "Desc",
                        "2021-04-30T10:54",
                        -1,
                        1)
                    );

                Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task PostTodoWithIncorrectDateFormat()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                var client = testScope.CreateClient();

                var response = await client.PostAsJsonAsync("/api/todos",
                    new BLL.Todo(
                        null,
                        "TestPost",
                        "Desc",
                        "2021-04-30T10:54:00",
                        0,
                        1)
                    );

                Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task PostTodoToNotExistingColumn()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                var client = testScope.CreateClient();

                var response = await client.PostAsJsonAsync("/api/todos",
                    new BLL.Todo(
                        null,
                        "TestPost",
                        "Desc",
                        "2021-04-30T10:54",
                        0,
                        3)
                    );

                Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task PostTodoWithNotUniqueTitle()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                testScope.AddSeedEntities(TestColumns);
                testScope.AddSeedEntities(TestTodos);
                var client = testScope.CreateClient();

                var response = await client.PostAsJsonAsync("/api/todos",
                    new BLL.Todo(
                        null,
                        "Todo1",
                        "salala",
                        "2021-04-30T10:54",
                        0,
                        1)
                    );

                Assert.AreEqual(System.Net.HttpStatusCode.Conflict, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task PostTodoSuccess()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                testScope.AddSeedEntities(TestColumns);
                var client = testScope.CreateClient();

                var response = await client.PostAsJsonAsync("/api/todos",
                    new BLL.Todo(
                        null,
                        "Todo3",
                        "salala",
                        "2021-04-30T10:54",
                        0,
                        1)
                    );

                var responseBody = await response.Content.ReadFromJsonAsync<BLL.Todo>();
                var insertedTodo = testScope.GetDbTableContent<DAL.Todo>().Single(t => t.Id == responseBody.Id);

                Assert.IsNotNull(response);
                Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
                Assert.IsNotNull(responseBody);
                Assert.AreEqual(insertedTodo, MapDtoToEntity(responseBody));
            }
        }

        [TestMethod]
        public async Task PutTodoWithoutId()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                var client = testScope.CreateClient();
                int queryId = 1;

                var response = await client.PutAsJsonAsync($"/api/todos/{queryId}",
                    new BLL.Todo(
                        null,
                        "TestPost",
                        "Desc",
                        "2021-04-30T10:54",
                        0,
                        1)
                    );

                Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task PutTodoWithNotExistingId()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                var client = testScope.CreateClient();
                int queryId = 4;
                int todoId = queryId;

                var response = await client.PutAsJsonAsync($"/api/todos/{queryId}",
                    new BLL.Todo(
                        todoId,
                        "UpdatedTodo1",
                        null,
                        "2021-04-30T10:54",
                        0,
                        1)
                    );

                Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task PutTodoWithNotMatchingIds()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                var client = testScope.CreateClient();
                int queryId = 1;
                int todoId = 2;

                var response = await client.PutAsJsonAsync($"/api/todos/{queryId}",
                    new BLL.Todo(
                        todoId,
                        "UpdatedTodo1",
                        null,
                        "2021-04-30T10:54",
                        0,
                        1)
                    );

                Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            }
        }


        [TestMethod]
        public async Task PutTodoWithNegativePriority()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                var client = testScope.CreateClient();
                int queryId = 1;
                int todoId = queryId;

                var response = await client.PutAsJsonAsync($"/api/todos/{queryId}",
                    new BLL.Todo(
                        todoId,
                        "UpdatedTodo1",
                        null,
                        "2021-04-30T10:54",
                        -1,
                        1)
                    );

                Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task PutTodoWithIncorrectDateFormat()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                var client = testScope.CreateClient();
                int queryId = 1;
                int todoId = queryId;

                var response = await client.PutAsJsonAsync($"/api/todos/{queryId}",
                    new BLL.Todo(
                        todoId,
                        "UpdatedTodo1",
                        null,
                        "2021-04-30T10:54:00",
                        0,
                        1)
                    );

                Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task PutTodoToNotExistingColumn()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                var client = testScope.CreateClient();
                int queryId = 1;
                int todoId = queryId;

                var response = await client.PutAsJsonAsync($"/api/todos/{queryId}",
                    new BLL.Todo(
                        todoId,
                        "UpdatedTodo1",
                        null,
                        "2021-04-30T10:54",
                        0,
                        3)
                    );

                Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task PutTodoWithNotUniqueTitle()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                var client = testScope.CreateClient();
                testScope.AddSeedEntities(TestColumns);
                testScope.AddSeedEntities(TestTodos);
                int queryId = 1;
                int todoId = queryId;

                var response = await client.PutAsJsonAsync($"/api/todos/{queryId}",
                    new BLL.Todo(
                        todoId,
                        "Todo2",
                        null,
                        "2021-04-30T10:54",
                        0,
                        1)
                    );

                Assert.AreEqual(System.Net.HttpStatusCode.Conflict, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task PutTodoSuccess()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                var client = testScope.CreateClient();
                testScope.AddSeedEntities(TestColumns);
                testScope.AddSeedEntities(TestTodos);
                int queryId = 1;
                int todoId = queryId;
                string uniqueTitle = "UniqueTitle";

                var response = await client.PutAsJsonAsync($"/api/todos/{queryId}",
                    new BLL.Todo(
                        todoId,
                        uniqueTitle,
                        null,
                        "2021-04-30T10:54",
                        0,
                        1)
                    );

                var updatedTodo = testScope.GetDbTableContent<DAL.Todo>().Single(t => t.Id == todoId);

                Assert.IsNotNull(response);
                Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response.StatusCode);
                Assert.AreEqual(updatedTodo.Title, uniqueTitle);
            }
        }

        [TestMethod]
        public async Task DeleteNotExistingTodo()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                testScope.AddSeedEntities(TestColumns);
                testScope.AddSeedEntities(TestTodos);
                var client = testScope.CreateClient();

                var response = await client.DeleteAsync("/api/todos/4");

                Assert.IsNotNull(response);
                Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task DeleteTodoSuccess()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                testScope.AddSeedEntities(TestColumns);
                testScope.AddSeedEntities(TestTodos);
                var client = testScope.CreateClient();

                int id = 1;
                var response = await client.DeleteAsync($"/api/todos/{id}");
                var deletedColumn = testScope.GetDbTableContent<DAL.Todo>().SingleOrDefault(t => t.Id == id);

                Assert.IsNotNull(response);
                Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response.StatusCode);
                Assert.IsNull(deletedColumn);
            }
        }

    }
}
