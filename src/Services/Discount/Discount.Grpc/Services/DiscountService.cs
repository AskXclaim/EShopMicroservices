namespace Discount.Grpc.Services;

public class DiscountService(DiscountContext dbContext, ILogger<DiscountContext> logger)
    : DiscountProtoService.DiscountProtoServiceBase
{
    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var requiredCoupon = await GetRequiredCouponAsync(request.ProductName) ?? GetNoDiscountCoupon();

        LogEvent(requiredCoupon, "Discount is retrieved for ProductName");

        var couponModel = requiredCoupon.Adapt<CouponModel>();

        return couponModel;
    }

    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        var couponToCreate = request.Coupon.Adapt<Coupon>();

        if (IsCouponNull(couponToCreate)) throw InvalidObjectException();

        if (IsNullOrWhiteSpace(request.Coupon.Amount)) throw AmountCannotBeNullOrWhiteSpaceException();

        var decimalAmount = DecimalTryParse(request.Coupon.Amount);
        if (decimalAmount is null) throw AmountMustBeNumericException();

        couponToCreate.Amount = decimalAmount.Value;

        var couponPresent = await GetRequiredCouponAsync(request.Coupon.ProductName);

        if (couponPresent is not null)
        {
            throw new RpcException(new Status(StatusCode.AlreadyExists,
                $"Discount already exists, try updating it instead." +
                $" ProductName:{couponPresent.ProductName}, amount:{couponPresent.Amount}"));
        }

        await dbContext.Coupons.AddAsync(couponToCreate);
        await dbContext.SaveChangesAsync();

        LogEvent(couponToCreate, "Discount successfully created.");

        var model = couponToCreate.Adapt<CouponModel>();
        return model;
    }

    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        var couponToUpdate = request.Coupon.Adapt<Coupon>();
        if (IsCouponNull(couponToUpdate)) throw InvalidObjectException();

        var couponPresent = await GetRequiredCouponAsync(request.Coupon.ProductName);

        if (couponPresent is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Discount does not exists, try creating it instead." +
                $" ProductName:{couponToUpdate.ProductName}, amount:{couponToUpdate.Amount}"));
        }

        PrepareCouponForUpdate(couponPresent, couponToUpdate);
        dbContext.Coupons.Update(couponPresent);
        await dbContext.SaveChangesAsync();

        LogEvent(couponPresent, "Discount successfully updated.");

        var couponModel = couponPresent.Adapt<CouponModel>();

        return couponModel;
    }

    private static void PrepareCouponForUpdate(Coupon couponPresent, Coupon couponToUpdate)
    {
        var id = couponPresent.Id;
        couponToUpdate.Adapt(couponPresent);
        couponPresent.Id = id;
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request,
        ServerCallContext context)
    {
        var couponToDelete = await GetRequiredCouponAsync(request.ProductName);

        var response = new DeleteDiscountResponse() { Success = true };

        if (couponToDelete is not null)
        {
            dbContext.Coupons.Remove(couponToDelete);
            await dbContext.SaveChangesAsync();

            LogEvent(couponToDelete, "Discount successfully deleted.");

            return response;
        }

        logger.LogInformation("No discount to delete found. Product name:{ProductName}", request.ProductName);
        response.Success = false;

        return response;
    }

    private static Coupon GetNoDiscountCoupon() =>
        new() { Id = 0, ProductName = "No Discount", Amount = 0, Description = "No Discount" };

    private Task<Coupon?> GetRequiredCouponAsync(string productName) =>
        dbContext.Coupons.SingleOrDefaultAsync(coupon => coupon.ProductName.ToLower() == productName.ToLower());

    private void LogEvent(Coupon coupon, string message) =>
        logger.LogInformation("{Message}: {ProductName}, Amount: {Amount}", message,
            coupon.ProductName, coupon.Amount);

    private static bool IsCouponNull(Coupon? coupon) => coupon is null;

    private static RpcException InvalidObjectException() =>
        new(new Status(StatusCode.InvalidArgument, "Invalid request object"));

    private static RpcException AmountCannotBeNullOrWhiteSpaceException() =>
        new(new Status(StatusCode.InvalidArgument, "Amount cannot be null, empty or whitespace"));

    private static decimal? DecimalTryParse(string amount) =>
        decimal.TryParse(amount, out var decimalAmount) ? decimalAmount : null;

    private static RpcException AmountMustBeNumericException() =>
        new(new Status(StatusCode.InvalidArgument, "Amount has to be convertable to a valid number"));

    private static bool IsNullOrWhiteSpace(string amount) => string.IsNullOrWhiteSpace(amount);
}