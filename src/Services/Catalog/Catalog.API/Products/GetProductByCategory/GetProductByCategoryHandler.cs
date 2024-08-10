namespace Catalog.API.Products.GetProductByCategory;

internal record GetProductByCategoryQuery(string Category) : IQuery<GetProductByCategoryResult>;

internal record GetProductByCategoryResult(IEnumerable<Product> Products);

internal class GetProductByCategoryQueryHandler(
    IDocumentSession session,
    ILogger<GetProductByCategoryQueryHandler> logger)
    : IQueryHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
{
    public async Task<GetProductByCategoryResult> Handle
        (GetProductByCategoryQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation(LoggerInformationFactory.GetHandlerCalledTextToLog(
            nameof(GetProductByCategoryQueryHandler),
            nameof(Handle), query));

        var products = await session.Query<Product>()
            .Where(product => product.Category.Contains(query.Category))
            .ToListAsync(token: cancellationToken);

        return new GetProductByCategoryResult(products);
    }
}