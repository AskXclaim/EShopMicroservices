using System.ComponentModel.DataAnnotations;

namespace Discount.Grpc.Models;

public class Coupon
{
    [Key] public int Id { get; set; }
    [MinLength(3)] [MaxLength(200)] public string ProductName { get; set; } = default!;
    [MinLength(3)] [MaxLength(200)] public string Description { get; set; } = default!;
    public decimal Amount { get; set; }
}