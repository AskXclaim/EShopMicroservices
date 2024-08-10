using System.Net;
using BuildingBlocks.Models;

namespace BuildingBlocks.Utilities;

public static class ApiResponseFactory
{
    public static ApiResponse GetSuccessfullyCreatedResponse(object? response) =>
        new ApiResponse()
        {
            Status = HttpStatusCode.Created,
            IsSuccess = true,
            Result = response
        };

    public static ApiResponse GetInternalServerErrorResponse(Exception exception) =>
        new ApiResponse()
        {
            Status = HttpStatusCode.InternalServerError,
            IsSuccess = false,
            Error = new List<string>()
                { exception.Message },
            Result = exception
        };
}