namespace Catalog.API.Products.GetProducts;

internal record GetProductsQuery(int? PageNumber = 1, int? PageSize = 10) : IQuery<GetProductsResult>;

internal record GetProductsResult(IEnumerable<Product> Products);

internal class GetProductsQueryHandler(IDocumentSession session)
    : IQueryHandler<GetProductsQuery, GetProductsResult>
{
    const int DefaultPageNumber = 1;
    const int DefaultPageSize = 10;

    public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var products = await session.Query<Product>().ToPagedListAsync(query.PageNumber ?? DefaultPageNumber,
            query.PageSize ?? DefaultPageSize, cancellationToken);

        return new GetProductsResult(products);
    }
}