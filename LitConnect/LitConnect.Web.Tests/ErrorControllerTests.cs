using LitConnect.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace LitConnect.Web.Tests;

[TestFixture]
public class ErrorControllerTests : IDisposable
{
    private ErrorController controller = null!;
    private bool isDisposed;

    [SetUp]
    public void Setup()
    {
        controller = new ErrorController();
    }

    [Test]
    public void HttpStatusCodeHandler_404_ShouldReturnNotFoundView()
    {
        // Act
        var actionResult = controller.HttpStatusCodeHandler(404);
        var result = actionResult as ViewResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ViewName, Is.EqualTo("NotFound"));
        });
    }

    [Test]
    public void HttpStatusCodeHandler_401_ShouldRedirectToLogin()
    {
        // Act
        var actionResult = controller.HttpStatusCodeHandler(401);
        var result = actionResult as RedirectToPageResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.PageName, Is.EqualTo("/Identity/Account/Login"));
        });
    }

    [Test]
    public void HttpStatusCodeHandler_OtherError_ShouldReturnErrorView()
    {
        // Act
        var actionResult = controller.HttpStatusCodeHandler(500);
        var result = actionResult as ViewResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ViewName, Is.EqualTo("Error"));
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