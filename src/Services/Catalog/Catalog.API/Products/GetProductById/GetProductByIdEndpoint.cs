namespace Catalog.API.Products.GetProductById;

//public record GetProductByIdRequest(Guid Id);

public record GetProductByIdResponse(Product Product);

public class GetProductByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/Products/{id:guid}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetProductByIdQuery(id));
                var response = result.Adapt<GetProductByIdResponse>();
                var apiResponse = ApiResponseFactory.GetSuccessfullyCreatedResponse(response);

                return Results.Ok(apiResponse);
            })
            .WithName($"{nameof(GetProductByIdEndpoint).Replace("Endpoint", "")}")
            .Produces<ApiResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Product By Id")
            .WithDescription("Get Product By Id");
    }
}