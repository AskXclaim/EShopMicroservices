namespace Catalog.API.Products.GetProductByCategory.Validations;

internal class GetProductByCategoryValidator : AbstractValidator<GetProductByCategoryQuery>
{
    public GetProductByCategoryValidator() =>
        RuleFor(query => query.Category).NotEmpty().WithMessage("Product category cannot be empty");
}