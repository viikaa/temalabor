using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Todo.DAL;
using Todo.BLL;
using System.Linq;

namespace Todo.Test
{
    [TestClass]
    public class ColumnTests
    {
        private static readonly DAL.Column[] TestColumns = new []
        {
            new DAL.Column { Id = 1, Title = "Column1" },
            new DAL.Column { Id = 2, Title = "Column2" },
            new DAL.Column { Id = 3, Title = "Column3" }
        };

        private static BLL.Column MapEntityToDto(DAL.Column entity) 
            => new BLL.Column(entity.Id, entity.Title);

        private static DAL.Column MapDtoToEntity(BLL.Column dto)
            => new DAL.Column { Id = dto.Id ?? 0, Title = dto.Title };

        [TestMethod]
        public async Task GetColumnsWithData()
        {
            using(var testScope = TestWebAppFactory.Create())
            {
                testScope.AddSeedEntities(TestColumns);

                var client = testScope.CreateClient();
                var response = await client.GetAsync("/api/columns");

                response.EnsureSuccessStatusCode();
                var actual = await response.Content.ReadFromJsonAsync<BLL.Column[]>();

                DAL.Column[] actualEntities = new DAL.Column[TestColumns.Length];
                for (int i = 0; i < TestColumns.Length; i++)
                {
                    actualEntities[i] = MapDtoToEntity(actual[i]);
                }

                Assert.IsNotNull(actual);
                CollectionAssert.AreEquivalent(TestColumns, actualEntities);
            }
        }

        [TestMethod]
        public async Task GetColumnsWhenEmpty()
        {
            using(var testScope = TestWebAppFactory.Create())
            {
                var client = testScope.CreateClient();
                var response = await client.GetAsync("/api/columns");

                response.EnsureSuccessStatusCode();
                var actual = await response.Content.ReadFromJsonAsync<BLL.Column[]>();

                Assert.IsNotNull(actual);
                Assert.AreEqual(0, actual.Length);
            }
        }

        

        [TestMethod]
        public async Task GetSingleColumnWhenExists()
        {
            using(var testScope = TestWebAppFactory.Create())
            {
                testScope.AddSeedEntities(TestColumns);
                int id = 1;
                var client = testScope.CreateClient();
                var response = await client.GetAsync($"/api/columns/{id}");

                response.EnsureSuccessStatusCode();
                var actual = await response.Content.ReadFromJsonAsync<BLL.Column>();
                var actualEntity = MapDtoToEntity(actual);

                Assert.IsNotNull(actual);
                Assert.AreEqual(TestColumns.Single(tc => tc.Id == id), actualEntity);
            }
        }

        [TestMethod]
        public async Task GetSingleColumnWhenDoesNotExist()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                testScope.AddSeedEntities(TestColumns);
                var client = testScope.CreateClient();

                int id = 4;
                var response = await client.GetAsync($"/api/columns/{id}");

                Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task PostColumnWithId()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                var client = testScope.CreateClient();

                var response = await client.PostAsJsonAsync("/api/columns", new BLL.Column(4, "TestPost"));

                Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task PostColumnWithNotUniqueTitle()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                testScope.AddSeedEntities(TestColumns);
                var client = testScope.CreateClient();

                var response = await client.PostAsJsonAsync("/api/columns", new BLL.Column(null, "Column1"));

                Assert.AreEqual(System.Net.HttpStatusCode.Conflict, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task PostColumnSuccess()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                testScope.AddSeedEntities(TestColumns);
                var client = testScope.CreateClient();

                var response = await client.PostAsJsonAsync("/api/columns", new BLL.Column(null, "Column4"));
                var responseBody = await response.Content.ReadFromJsonAsync<BLL.Column>();
                var insertedColumn = testScope.GetDbTableContent<DAL.Column>().Single(c => c.Id == responseBody.Id);

                Assert.IsNotNull(response);
                Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
                Assert.IsNotNull(responseBody);
                Assert.AreEqual(insertedColumn, MapDtoToEntity(responseBody));
            }
        }

        [TestMethod]
        public async Task UpdateColumnWithoutId()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                testScope.AddSeedEntities(TestColumns);
                var client = testScope.CreateClient();

                var response = await client.PutAsJsonAsync("/api/columns/1", new BLL.Column(null, "Column4"));
                
                Assert.IsNotNull(response);
                Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task UpdateColumnWithInconsistentId()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                testScope.AddSeedEntities(TestColumns);
                var client = testScope.CreateClient();

                var response = await client.PutAsJsonAsync("/api/columns/1", new BLL.Column(2, "Column4"));

                Assert.IsNotNull(response);
                Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task UpdateColumnWithNotExistingId()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                testScope.AddSeedEntities(TestColumns);
                var client = testScope.CreateClient();

                var response = await client.PutAsJsonAsync("/api/columns/4", new BLL.Column(4, "Column4"));

                Assert.IsNotNull(response);
                Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task UpdateColumnWithNotUniqueTitle()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                testScope.AddSeedEntities(TestColumns);
                var client = testScope.CreateClient();

                var response = await client.PutAsJsonAsync("/api/columns/2", new BLL.Column(2, "Column1"));

                Assert.IsNotNull(response);
                Assert.AreEqual(System.Net.HttpStatusCode.Conflict, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task UpdateColumnSuccess()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                testScope.AddSeedEntities(TestColumns);
                var client = testScope.CreateClient();

                int id = 2;
                var update = new BLL.Column(id, "Column22");
                var response = await client.PutAsJsonAsync($"/api/columns/{id}", update);
                var updatedColumn = testScope.GetDbTableContent<DAL.Column>().Single(c => c.Id == id);

                Assert.IsNotNull(response);
                Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response.StatusCode);
                Assert.AreEqual(update, MapEntityToDto(updatedColumn));
            }
        }

        [TestMethod]
        public async Task DeleteNotExistingColumn()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                testScope.AddSeedEntities(TestColumns);
                var client = testScope.CreateClient();

                var response = await client.DeleteAsync("/api/columns/4");

                Assert.IsNotNull(response);
                Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task DeleteSuccess()
        {
            using (var testScope = TestWebAppFactory.Create())
            {
                testScope.AddSeedEntities(TestColumns);
                var client = testScope.CreateClient();

                int id = 1;
                var response = await client.DeleteAsync($"/api/columns/{id}");
                var deletedColumn = testScope.GetDbTableContent<DAL.Column>().SingleOrDefault(c => c.Id == id);

                Assert.IsNotNull(response);
                Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response.StatusCode);
                Assert.IsNull(deletedColumn);
            }
        }

    }
}
