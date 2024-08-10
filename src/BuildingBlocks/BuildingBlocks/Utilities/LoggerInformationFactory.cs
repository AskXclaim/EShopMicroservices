using BuildingBlocks.CQRS;
using MediatR;

namespace BuildingBlocks.Utilities;

public static class LoggerInformationFactory
{
    public static string GetHandlerCalledTextToLog(string handlerName, string handleMethodName,object query) =>
        $"{handlerName}.{handleMethodName} called with {query}";
}