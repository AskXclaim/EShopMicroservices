namespace Catalog.API.Exceptions;

public class ProductNotFound : Exception
{
    public ProductNotFound() : base("Product not found!")
    {
    }
}