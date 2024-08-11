namespace Catalog.API.Products.DeleteProduct;

//public record DeleteProductRequest(Guid Id);

public record DeleteProductResponse(bool IsSuccessful);

public class DeleteProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete($"/{Constants.Constants.ApiProductRoute}" + "/{id:guid}",
                async (Guid id, ISender sender) =>
                {
                    var result = await sender.Send(new DeleteProductCommand(id));
                    var response = result.Adapt<DeleteProductResponse>();

                    var apiResponse = ApiResponseFactory.GetOkResponse(response);

                    return Results.Ok(apiResponse);
                })
            .WithName($"{nameof(DeleteProductEndpoint).Replace("Endpoint", "")}")
            .Produces<ApiResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete Product")
            .WithDescription("Delete Product");
        ;
    }
}