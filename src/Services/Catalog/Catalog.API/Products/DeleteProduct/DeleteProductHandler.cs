namespace Catalog.API.Products.DeleteProduct;

internal record DeleteProductCommand(Guid Id) : ICommand<DeleteProductResult>;

internal record DeleteProductResult(bool IsSuccessful);

internal class DeleteProductCommandHandler(IDocumentSession session, ILogger<DeleteProductCommandHandler> logger)
    : ICommandHandler<DeleteProductCommand, DeleteProductResult>
{
    public async Task<DeleteProductResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation(LoggerInformationFactory.GetHandlerCalledTextToLog(nameof(DeleteProductCommandHandler),
            nameof(Handle), command));

        session.Delete<Product>(command.Id);
        await session.SaveChangesAsync(cancellationToken);

        return new DeleteProductResult(true);
    }
}