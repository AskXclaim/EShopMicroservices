namespace Discount.Grpc.Data;

public static class Extensions
{
    public static async Task<IApplicationBuilder> UseMigration(this IApplicationBuilder app)
    {
        await using var scope = app.ApplicationServices.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<DiscountContext>();
        await dbContext.Database.MigrateAsync();

        return app;
    }
}