namespace Basket.API.StoreBasket;

internal record StoreBasketResponse(string UserName);

internal record StoreBasketRequest(ShoppingCart Cart);

public class StoreBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost($"/{Constants.Constants.Basket}", async (StoreBasketRequest request, ISender sender) =>
            {
                var result = await sender.Send(request);
                var response = result.Adapt<StoreBasketResponse>();

                return Results.Created($"/{Constants.Constants.Basket}/{response.UserName}", response);
            })
            .WithName("Create basket/store")
            .Produces<StoreBasketResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Create basket/store")
            .WithDescription("Create a users basket/store");
    }
}