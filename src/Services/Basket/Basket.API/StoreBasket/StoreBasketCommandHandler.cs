namespace Basket.API.StoreBasket;

internal record StoreBasketResult(string UserName);

internal record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;

internal class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
{
    public StoreBasketCommandValidator()
    {
        RuleFor(command => command.Cart).NotNull().WithMessage($"{nameof(StoreBasketCommand.Cart)} cannot be null");
        RuleFor(command => command.Cart.UserName).NotEmpty()
            .WithMessage($"{nameof(StoreBasketCommand.Cart.UserName)} is required");
    }
}

internal class StoreBasketCommandHandler : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        return new StoreBasketResult("developing-test");
    }
}