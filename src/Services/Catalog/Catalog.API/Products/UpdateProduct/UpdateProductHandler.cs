namespace Catalog.API.Products.UpdateProduct;

internal record UpdateProductCommand(
    Guid Id,
    string Name,
    List<string> Category,
    string Description,
    string ImageFile,
    decimal Price)
    : ICommand<UpdateProductResult>;

internal record UpdateProductResult(bool IsSuccessful);

internal class UpdateProductCommandHandler(IDocumentSession session, ILogger<UpdateProductCommandHandler> logger)
    : ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation(LoggerInformationFactory.GetHandlerCalledTextToLog(nameof(UpdateProductCommandHandler),
            nameof(Handle), command));

        var productToUpdate = await session.LoadAsync<Product>(command.Id, cancellationToken);
        if (productToUpdate is null) throw new ProductNotFoundException();

        command.Adapt(productToUpdate);

        session.Update(productToUpdate);
        await session.SaveChangesAsync(cancellationToken);

        return new UpdateProductResult(true);
    }
}