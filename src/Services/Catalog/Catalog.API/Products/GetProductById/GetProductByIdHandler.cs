using Catalog.API.Exceptions;

namespace Catalog.API.Products.GetProductById;

internal record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResult>;

internal record GetProductByIdResult(Product Product);

internal class GetProductByIdQueryHandler(IDocumentSession session, ILogger<GetProductByIdQueryHandler> logger)
    : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
{
    public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation($"{nameof(GetProductByIdQueryHandler)}." +
                              $"{nameof(Handle)} called with {query}");

        var product = await session.LoadAsync<Product>(query.Id, cancellationToken);

        if (product is null) throw new ProductNotFound();

        return new GetProductByIdResult(product);
    }
}