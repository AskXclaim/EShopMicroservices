namespace Catalog.API.Products.UpdateProduct.Validations;

internal class UpdateProductValidator:AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(command => command.Id).NotEmpty().WithMessage("Product Id cannot be empty");
        RuleFor(command => command.Price).GreaterThan(0).WithMessage("Price must be greater than zero");
    }
}