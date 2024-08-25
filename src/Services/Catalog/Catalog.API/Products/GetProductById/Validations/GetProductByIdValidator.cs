namespace Catalog.API.Products.GetProductById.Validations;

internal class GetProductByIdValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdValidator() =>
        RuleFor(query => query.Id).NotEmpty().WithMessage("Product Id cannot be empty");
}