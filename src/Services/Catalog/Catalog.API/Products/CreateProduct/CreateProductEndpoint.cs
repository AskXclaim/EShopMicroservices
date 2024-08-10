namespace Catalog.API.Products.CreateProduct;

internal record CreateProductRequest(
    string Name,
    List<string> Category,
    string Description,
    string ImageFile,
    decimal Price);

internal record CreateProductResponse(Guid Id);

public class CreateProductEndpoint : ICarterModule
{
    private ApiResponse _apiResponse;

    public CreateProductEndpoint() => _apiResponse = new ApiResponse();


    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/products",
                async (CreateProductRequest request, ISender sender) =>
                {
                    try
                    {
                        var command = request.Adapt<CreateProductCommand>();
                        var result = await sender.Send(command);
                        var response = result.Adapt<CreateProductResponse>();
                        _apiResponse = ApiResponseFactory.GetSuccessfullyCreatedResponse(response);

                        return Results.Created($"/products/{response.Id}", _apiResponse);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return Results.BadRequest(_apiResponse);
                    }
                })
            .WithName("CreateProduct")
            .Produces<ApiResponse>()
            .WithSummary("Create Product")
            .WithDescription("Create product");
    }
}