namespace Basket.API.GetBasket;

internal record GetBasketResult(ShoppingCart Cart);
internal record GetBasketQuery(string UserName) : IQuery<GetBasketResult>;


internal class GetBasketQueryHandler : IQueryHandler<GetBasketQuery, GetBasketResult>
{
    public async Task<GetBasketResult> Handle(GetBasketQuery query, CancellationToken cancellationToken)
    {
        return new GetBasketResult(new ShoppingCart("developing-test"));
    }
}