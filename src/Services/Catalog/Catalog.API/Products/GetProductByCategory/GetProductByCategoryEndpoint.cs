namespace Catalog.API.Products.GetProductByCategory;

//public record GetProductByCategoryRequest(string Category);

public record GetProductByCategoryResponse(IEnumerable<Product> Products);

public class GetProductByCategoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"/{Constants.Constants.ApiProductRoute}" +
                   $"/{Constants.Constants.ApiProductCategoryRoute}" +
                   "/{category}", async (string category, ISender sender) =>
            {
                var result = await sender.Send(new GetProductByCategoryQuery(category));

                var response = result.Adapt<GetProductByCategoryResponse>();

                return Results.Ok(ApiResponseFactory.GetOkResponse(response));
            })
            .WithName($"{nameof(GetProductByCategoryEndpoint).Replace("Endpoint", "")}")
            .Produces<ApiResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Product By Category")
            .WithDescription("Get Product By Category");
    }
}