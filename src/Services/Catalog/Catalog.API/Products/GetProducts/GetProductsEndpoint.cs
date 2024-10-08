namespace Catalog.API.Products.GetProducts;

public record GetProductsRequest(int? PageNumber = 1, int? PageSize = 10);

public record GetProductsResponse(IEnumerable<Product> Products);

public class GetProductsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"/{Constants.Constants.ApiProductRoute}",
                async ([AsParameters] GetProductsRequest request, ISender sender) =>
                {
                    var query = request.Adapt<GetProductsQuery>();
                    var result = await sender.Send(query);
                    var response = result.Adapt<GetProductsResponse>();

                    return Results.Ok(ApiResponseFactory.GetOkResponse(response));
                })
            .WithName($"{nameof(GetProductsEndpoint).Replace("Endpoint", "")}")
            .Produces<ApiResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Products")
            .WithDescription("Get Products");
    }
}