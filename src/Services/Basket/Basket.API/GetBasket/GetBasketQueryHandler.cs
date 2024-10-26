using Basket.API.Data;
using Basket.API.Data.Interfaces;

namespace Basket.API.GetBasket;

internal record GetBasketResult(ShoppingCart Cart);

internal record GetBasketQuery(string UserName) : IQuery<GetBasketResult>;

internal class GetBasketQueryHandler(IBasketRepository repository) : IQueryHandler<GetBasketQuery, GetBasketResult>
{
    public async Task<GetBasketResult> Handle(GetBasketQuery query, CancellationToken cancellationToken)
    {
        var basket = await repository.GetBasket(query.UserName);
        return new GetBasketResult(basket);
    }
}