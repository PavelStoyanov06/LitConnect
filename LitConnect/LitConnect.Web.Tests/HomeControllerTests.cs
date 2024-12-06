using LitConnect.Web.Controllers;
using LitConnect.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace LitConnect.Web.Tests;

[TestFixture]
public class HomeControllerTests : IDisposable
{
    private Mock<ILogger<HomeController>> loggerMock = null!;
    private HomeController controller = null!;
    private bool isDisposed;

    [SetUp]
    public void Setup()
    {
        loggerMock = new Mock<ILogger<HomeController>>();
        controller = new HomeController(loggerMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Test]
    public void Index_ShouldReturnView()
    {
        // Act
        var actionResult = controller.Index();
        var result = actionResult as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void Privacy_ShouldReturnView()
    {
        // Act
        var actionResult = controller.Privacy();
        var result = actionResult as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void Error_ShouldReturnViewWithErrorViewModel()
    {
        // Act
        var actionResult = controller.Error();
        var result = actionResult as ViewResult;
        var model = result?.Model as ErrorViewModel;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
        });
    }

    [TearDown]
    public void TearDown() => Dispose();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!isDisposed)
        {
            if (disposing)
            {
                controller?.Dispose();
            }
            isDisposed = true;
        }
    }
}