using FluentValidation;
using QuizMaker.API.Middlewares.CustomResponses;
using QuizMaker.API.Utils;
using QuizMaker.Domain.Exceptions;

namespace QuizMaker.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedAccessException exception)
        {
            await HandleUnauthorizedAccessExceptionAsync(context, exception);
            _logger.LogWarning(exception, exception.Message, exception.StackTrace);
        }
        catch (ValidationException exception)
        {
            await HandleValidationExceptionAsync(context, exception);
            _logger.LogInformation(exception, exception.Message, exception.Errors.ToLogMessage());
        }
        catch (RecordNotFoundException exception)
        {
            await HandleNotFoundExceptionAsync(context, exception);
            _logger.LogWarning(exception, exception.Message);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
            _logger.LogError(exception, exception.Message);
        }
        finally
        {
            _logger.LogInformation(
                "Request {method} {url} => {statusCode}",
                context.Request?.Method,
                context.Request?.Path.Value,
                context.Response?.StatusCode);
        }
    }

    private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        var validationDetails = new CustomValidationErrorDetails(exception.Message, exception.Errors);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)validationDetails.Status;
        await context.Response.WriteAsync(validationDetails.ToString());
    }

    private async Task HandleNotFoundExceptionAsync(HttpContext context, RecordNotFoundException exception)
    {
        var errorDetails = new NotFoundErrorDetails(exception.Message);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = errorDetails.Status;
        await context.Response.WriteAsync(errorDetails.ToString());
    }

    private async Task HandleUnauthorizedAccessExceptionAsync(HttpContext context,
        UnauthorizedAccessException exception)
    {
        var errorDetails = new UnauthorizedErrorDetails(exception.Message);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = errorDetails.Status;
        await context.Response.WriteAsync(errorDetails.ToString());
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var errorDetails = new ServerErrorDetails(exception.Message);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = errorDetails.Status;
        await context.Response.WriteAsync(errorDetails.ToString());
    }
}