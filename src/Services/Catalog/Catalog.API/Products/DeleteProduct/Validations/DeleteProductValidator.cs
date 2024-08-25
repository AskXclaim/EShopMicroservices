namespace Catalog.API.Products.DeleteProduct.Validations;

internal class DeleteProductValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductValidator() =>
        RuleFor(command => command.Id).NotEmpty().WithMessage("Product Id cannot be empty");
}