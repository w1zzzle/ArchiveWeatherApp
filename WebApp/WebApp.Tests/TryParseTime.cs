using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.Linq;
using WebApp.Controllers;
using WebApp.Models;

namespace WebApp.Tests
{
    [TestFixture]
    public class HomeControllerTests
    {
        [Test]
        public void TestIsHeaderValid_InvalidHeader()
        {
            // Arrange
            var controller = new HomeController(null);
            var headerRowMock = new Mock<IRow>();

            headerRowMock.Setup(row => row.Cells.Count).Returns(3);

            string[] expectedHeaders = { "Дата", "Время", "Т", "Отн. влажность", "Td", "Атм. давление,", "Направление", "Скорость", "Облачность,", "h", "VV", "Погодные явления" };
            headerRowMock.Setup(row => row.GetCell(It.IsAny<int>())).Returns((int index) => CreateCellMock($"InvalidHeader{index}").Object);

            // Act
            var result = controller.IsHeaderValid(headerRowMock.Object);

            // Assert
            Assert.IsFalse(result);
        }

        private Mock<ICell> CreateCellMock(string value)
        {
            var cellMock = new Mock<ICell>();
            cellMock.Setup(cell => cell.StringCellValue).Returns(value);
            return cellMock;
        }
    }
}
