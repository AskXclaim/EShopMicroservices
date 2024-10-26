using System.Text.Json;
using System.Transactions;
using Basket.API.Data.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.API.Data.Instances;

public class CachedBasketRepository(IBasketRepository repository, IDistributedCache cache) : IBasketRepository
{
    public async Task<ShoppingCart> GetBasket(string userName, CancellationToken cancellationToken = default)
    {
        var cachedBasket = await cache.GetStringAsync(userName, cancellationToken);

        if (!string.IsNullOrWhiteSpace(cachedBasket))
            return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket);

        var requiredBasket = await repository.GetBasket(userName, cancellationToken);
        await cache.SetStringAsync(userName, JsonSerializer.Serialize(requiredBasket));

        return requiredBasket;
    }

    public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellationToken = default)
    {
        var storedBasket = await repository.StoreBasket(basket, cancellationToken);
        await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(storedBasket));
        return storedBasket;
    }

    public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
    {
        var isDeleted = await repository.DeleteBasket(userName, cancellationToken);
        //Todo should we safe guard for instances where one is deleted and the other isn't?
        if (isDeleted) await cache.RemoveAsync(userName);
        else throw new TransactionException($"Basket with username '{userName}' was not deleted properly");
        
        return isDeleted;
    }
}