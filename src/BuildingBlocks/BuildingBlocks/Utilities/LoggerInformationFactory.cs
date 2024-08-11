namespace BuildingBlocks.Utilities;

public static class LoggerInformationFactory
{
    public static string GetHandlerCalledTextToLog(string handlerName, string handleMethodName,object queryOrCommand) =>
        $"{handlerName}.{handleMethodName} called with {queryOrCommand}";
}