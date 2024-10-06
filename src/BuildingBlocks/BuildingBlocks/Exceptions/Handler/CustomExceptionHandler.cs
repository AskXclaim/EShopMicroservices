namespace BuildingBlocks.Exceptions.Handler;

public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError("Error Message: {ExceptionMessage}, Time of occurrence {Time}",
            exception.Message, DateTime.UtcNow);

        var details = GetDetails(exception);
        httpContext.Response.StatusCode = details.StatusCode;

        var problemDetail = GetProblemDetails(httpContext, details);

        AddExtensionToProblemDetails(httpContext.TraceIdentifier, exception, problemDetail);

        await httpContext.Response.WriteAsJsonAsync(problemDetail, cancellationToken: cancellationToken);
        return true;
    }

    private static (string Detail, string Title, int StatusCode) GetDetails(Exception exception)
    {
        (string Detail, string Title, int StatusCode) details = exception switch
        {
            InternalServerException => (exception.Message, exception.GetType().Name,
                StatusCodes.Status500InternalServerError),
            ValidationException => (exception.Message, exception.GetType().Name,
                StatusCodes.Status400BadRequest),
            BadRequestException => (exception.Message, exception.GetType().Name,
                StatusCodes.Status400BadRequest),
            NotFoundException => (exception.Message, exception.GetType().Name,
                StatusCodes.Status404NotFound),
            _ => (exception.Message, exception.GetType().Name,
                StatusCodes.Status500InternalServerError)
        };
        return details;
    }

    private static ProblemDetails GetProblemDetails(HttpContext httpContext,
        (string Detail, string Title, int StatusCode) details) => new()
    {
        Title = details.Title,
        Detail = details.Detail,
        Status = details.StatusCode,
        Instance = httpContext.Request.Path
    };

    private static void AddExtensionToProblemDetails(string traceIdentifier, Exception exception,
        ProblemDetails problemDetail)
    {
        problemDetail.Extensions.Add("traceId", traceIdentifier);

        if (exception is ValidationException validationException)
        {
            problemDetail.Extensions.Add("validationErrors", validationException.Errors);
        }
    }
}