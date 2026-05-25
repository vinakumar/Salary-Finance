using System.Net;
using FluentAssertions;
using FullStack.Api.Middleware;
using FullStack.Domain.Common;
using FullStack.Domain.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace FullStack.Api.UnitTests.Tests.Middleware;

[TestFixture]
public class GlobalExceptionMiddlewareTests
{
    private Mock<ILogger<GlobalExceptionMiddleware>> _loggerMock = null!;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionMiddleware>>();
    }

    [Test]
    public async Task InvokeAsync_Returns404_ForNotFoundException()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new GlobalExceptionMiddleware(
            _ => throw new NotFoundException("Product", 99),
            _loggerMock.Object);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(404);
        context.Response.ContentType.Should().Be("application/problem+json");
    }

    [Test]
    public async Task InvokeAsync_Returns409_ForConflictException()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new GlobalExceptionMiddleware(
            _ => throw new ConflictException("Already exists"),
            _loggerMock.Object);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(409);
    }

    [Test]
    public async Task InvokeAsync_Returns422_ForValidationException()
    {
        var errors = new[] { new ApiErrorDetail { Field = "Name", Message = "Required" } };
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new GlobalExceptionMiddleware(
            _ => throw new Domain.Common.ValidationException(errors),
            _loggerMock.Object);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(422);
    }

    [Test]
    public async Task InvokeAsync_Returns500_ForUnhandledException()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new GlobalExceptionMiddleware(
            _ => throw new InvalidOperationException("Oops"),
            _loggerMock.Object);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(500);
    }
}
