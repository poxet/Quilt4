using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Quilt4.Interface;
using Quilt4.Web.Controllers;

namespace Quilt4.Web.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            var initiativeBusinessMock = new Mock<IInitiativeBusiness>(MockBehavior.Strict);
            var applicationVersionBusinessMock = new Mock<IApplicationVersionBusiness>(MockBehavior.Strict);
            var accountRepositoryMock = new Mock<IAccountRepository>(MockBehavior.Strict);
            var sessionBusinessMock = new Mock<ISessionBusiness>(MockBehavior.Strict);
            var issueBusinessMock = new Mock<IIssueBusiness>(MockBehavior.Strict);
            var controller = new HomeController(initiativeBusinessMock.Object, applicationVersionBusinessMock.Object, accountRepositoryMock.Object, sessionBusinessMock.Object, issueBusinessMock.Object);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            var initiativeBusinessMock = new Mock<IInitiativeBusiness>(MockBehavior.Strict);
            var applicationVersionBusinessMock = new Mock<IApplicationVersionBusiness>(MockBehavior.Strict);
            var accountRepositoryMock = new Mock<IAccountRepository>(MockBehavior.Strict);
            var sessionBusinessMock = new Mock<ISessionBusiness>(MockBehavior.Strict);
            var issueBusinessMock = new Mock<IIssueBusiness>(MockBehavior.Strict);
            var controller = new HomeController(initiativeBusinessMock.Object, applicationVersionBusinessMock.Object, accountRepositoryMock.Object, sessionBusinessMock.Object, issueBusinessMock.Object);

            // Act
            var result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            var initiativeBusinessMock = new Mock<IInitiativeBusiness>(MockBehavior.Strict);
            var applicationVersionBusinessMock = new Mock<IApplicationVersionBusiness>(MockBehavior.Strict);
            var accountRepositoryMock = new Mock<IAccountRepository>(MockBehavior.Strict);
            var sessionBusinessMock = new Mock<ISessionBusiness>(MockBehavior.Strict);
            var issueBusinessMock = new Mock<IIssueBusiness>(MockBehavior.Strict);
            var controller = new HomeController(initiativeBusinessMock.Object, applicationVersionBusinessMock.Object, accountRepositoryMock.Object, sessionBusinessMock.Object, issueBusinessMock.Object);

            // Act
            var result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
