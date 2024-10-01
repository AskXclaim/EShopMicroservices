namespace Basket.API.DeleteBasket;

internal record DeleteBasketResult(bool IsSuccess);

internal record DeleteBasketCommand(string UserName) : ICommand<DeleteBasketResult>;

internal class DeleteBasketCommandValidator : AbstractValidator<DeleteBasketCommand>
{
    public DeleteBasketCommandValidator()
    {
        RuleFor(command => command.UserName).NotEmpty()
            .WithMessage($"{nameof(DeleteBasketCommand.UserName)} is required");
    }
}

internal class DeleteBasketCommandHandler : ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
{
    public async Task<DeleteBasketResult> Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
    {
        return new DeleteBasketResult(true);
    }
}