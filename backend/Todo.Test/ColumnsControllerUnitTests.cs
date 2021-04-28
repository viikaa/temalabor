using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Todo.API.Controllers;
using Todo.BLL;
using Todo.BLL.Services;

namespace Todo.Test
{
    [TestClass]
    public class ColumnsControllerUnitTests
    {
        private static readonly IColumnService Service = new MockColumnService();
        private static readonly ColumnsController Controller = new ColumnsController(Service);

        [TestMethod]
        public void GetColumnsWithData()
        {
            var columnsList = new List<Column>
            {
                new Column(1, "Column1"),
                new Column(2, "Column1"),
                new Column(3, "Column1"),
            };
            MockColumnService.AddColumnsToMockDb(columnsList);

            var task = Controller.GetColumns();

            var resultValue = task.Result.Value;
            Assert.IsNotNull(task);
            Assert.IsNotNull(resultValue);
            Assert.IsInstanceOfType(task, typeof(Task<ActionResult<List<Column>>>));
            Assert.AreEqual(MockColumnService.GetMockDb(), resultValue);
        }

        [TestMethod]
        public void GetColumnsWhenEmpty()
        {
            var task = Controller.GetColumns();

            var resultValue = task.Result.Value;
            Assert.IsNotNull(task);
            Assert.IsNotNull(resultValue);
            Assert.IsInstanceOfType(task, typeof(Task<ActionResult<List<Column>>>));
            Assert.AreEqual(MockColumnService.GetMockDb(), resultValue);
            Assert.IsTrue(resultValue.Count == 0);
        }

        [TestMethod]
        public void GetSingleColumnSuccess()
        {
            var task = Controller.GetSingleColumn(5);

            var resultValue = task.Result.Value;
            Assert.IsNotNull(task);
            Assert.IsNotNull(resultValue);
            Assert.IsInstanceOfType(task, typeof(Task<ActionResult<Column>>));
            Assert.AreEqual(new Column(5, "Column5"), resultValue);
        }

        [TestMethod]
        public void GetSingleColumnNotFound()
        {
            var task = Controller.GetSingleColumn(1);

            var actionResult = task.Result.Result;
            Assert.IsNotNull(task);
            Assert.IsInstanceOfType(task, typeof(Task<ActionResult<Column>>));
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetSingleColumnDatabaseError()
        {
            var task = Controller.GetSingleColumn(2);

            var actionResult = task.Result.Result;
            var statusCode = (actionResult as ObjectResult).StatusCode;
            var problemDetails = ((actionResult as ObjectResult).Value as ProblemDetails).Detail;
            Assert.IsNotNull(task);
            Assert.IsInstanceOfType(task, typeof(Task<ActionResult<Column>>));
            Assert.IsInstanceOfType(actionResult, typeof(ObjectResult));
            Assert.AreEqual(500, statusCode);
            Assert.AreEqual("Database not found", problemDetails);
        }

        [TestMethod]
        public void GetSingleColumnMoreColumnsWithSameId()
        {
            var task = Controller.GetSingleColumn(3);

            var actionResult = task.Result.Result;
            var statusCode = (actionResult as ObjectResult).StatusCode;
            var problemDetails = ((actionResult as ObjectResult).Value as ProblemDetails).Detail;
            Assert.IsNotNull(task);
            Assert.IsInstanceOfType(task, typeof(Task<ActionResult<Column>>));
            Assert.IsInstanceOfType(actionResult, typeof(ObjectResult));
            Assert.AreEqual(500, statusCode);
            Assert.AreEqual("There is more than one column with the given id.", problemDetails);
        }

        [TestMethod]
        public void GetSingleColumnOtherProblems()
        {
            var task = Controller.GetSingleColumn(4);

            var actionResult = task.Result.Result;
            var statusCode = (actionResult as ObjectResult).StatusCode;
            var problemDetails = ((actionResult as ObjectResult).Value as ProblemDetails).Detail;
            Assert.IsNotNull(task);
            Assert.IsInstanceOfType(task, typeof(Task<ActionResult<Column>>));
            Assert.IsInstanceOfType(actionResult, typeof(ObjectResult));
            Assert.AreEqual(statusCode, 500);
            Assert.IsNull(problemDetails);
        }

        [TestMethod]
        public void PostColumnSuccess()
        {
            var column = new Column(1, "Column1");

            var task = Controller.Post(column);
            var actionResult = task.Result.Result;
            var statusCode = (actionResult as CreatedAtActionResult).StatusCode;
            var actionName = (actionResult as CreatedAtActionResult).ActionName;
            var routerValues = (actionResult as CreatedAtActionResult).RouteValues;
            var value = (actionResult as CreatedAtActionResult).Value;


            Assert.IsNotNull(task);
            Assert.IsInstanceOfType(actionResult, typeof(CreatedAtActionResult));
            Assert.AreEqual(201, statusCode);
            Assert.AreEqual(nameof(Controller.GetSingleColumn), actionName);
            Assert.IsTrue(routerValues.ContainsKey("id"));
            Assert.AreEqual(1, routerValues["id"]);
            Assert.AreEqual(column, value);
        }

        [TestMethod]
        public void PostColumnInvalidRequestBody()
        {
            var task = Controller.Post(new Column(2, "Column2"));

            var actionResult = task.Result.Result;
            Assert.IsNotNull(task);
            Assert.IsInstanceOfType(task, typeof(Task<ActionResult<Column>>));
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public void PostColumnNotUnique()
        {
            var task = Controller.Post(new Column(3, "Column3"));

            var actionResult = task.Result.Result;
            Assert.IsNotNull(task);
            Assert.IsInstanceOfType(task, typeof(Task<ActionResult<Column>>));
            Assert.IsInstanceOfType(actionResult, typeof(ConflictResult));
        }

        [TestMethod]
        public void PutColumnSuccess()
        {
            var task = Controller.Put(1, new Column(1, "Column1"));

            var actionResult = task.Result;
            var statusCode = (actionResult as NoContentResult).StatusCode;
            Assert.IsNotNull(task);
            Assert.IsInstanceOfType(task, typeof(Task<IActionResult>));
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
            Assert.AreEqual(204, statusCode);
        }

        [TestMethod]
        public void PutColumnInvalidRequestBody()
        {
            var task = Controller.Put(2, new Column(2, "Column1"));

            var actionResult = task.Result;
            var statusCode = (actionResult as BadRequestResult).StatusCode;
            Assert.IsNotNull(task);
            Assert.IsInstanceOfType(task, typeof(Task<IActionResult>));
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
            Assert.AreEqual(400, statusCode);
        }

        [TestMethod]
        public void PutColumnNotUniqueColumn()
        {
            var task = Controller.Put(2, new Column(3, "Column1"));

            var actionResult = task.Result;
            var statusCode = (actionResult as ConflictResult).StatusCode;
            Assert.IsNotNull(task);
            Assert.IsInstanceOfType(task, typeof(Task<IActionResult>));
            Assert.IsInstanceOfType(actionResult, typeof(ConflictResult));
            Assert.AreEqual(409, statusCode);
        }

        [TestMethod]
        public void DeleteColumnSuccess()
        {
            var task = Controller.Delete(1);

            var result = task.Result;
            Assert.IsNotNull(task);
            Assert.IsInstanceOfType(task, typeof(Task<IActionResult>));
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public void DeleteColumnFail()
        {
            var task = Controller.Delete(2);

            var result = task.Result;
            var statusCode = (result as ObjectResult).StatusCode;
            var problemDetails = ((result as ObjectResult).Value as ProblemDetails).Detail;
            Assert.IsNotNull(task);
            Assert.IsInstanceOfType(task, typeof(Task<IActionResult>));
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(404, statusCode);
            Assert.AreEqual("Column with the given id does not exist, or could not be deleted", problemDetails);
        }
    }
}
