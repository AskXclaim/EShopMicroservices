namespace BuildingBlocks.Behaviours;

public class LoggingBehaviour<TRequest, TResponse>(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("[START] Handle request={RequestName}{NewLineOne}" +
                        "-Response={ResponseName}{NewLineTwo}" +
                        "-Request data={Data}", typeof(TRequest).Name, Environment.NewLine,
            typeof(TResponse).Name, Environment.NewLine, request);

        var timer = new Stopwatch();
        timer.Start();
        var response = await next();
        timer.Stop();
        var executionTime = timer.Elapsed;
        if (executionTime.Seconds > 3)
        {
            logger.LogWarning("[PERFORMANCE] The request {Request} took {TimeTaken}",
                typeof(TRequest).Name, executionTime.Seconds);
        }

        logger.LogInformation("[END] Handled {Request} with {Response}",
            typeof(TRequest).Name, typeof(TResponse).Name);

        return response;
    }
}