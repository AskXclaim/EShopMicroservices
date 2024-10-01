namespace Basket.API.Models;

public class ShoppingCart
{
    public string UserName { get; set; } = default!;
    public ICollection<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
    public decimal TotalPrice => Items.Sum(item => item.Price * item.Quantity);

    public ShoppingCart(string userName) => UserName = userName;

    // Required for mapping
    public ShoppingCart()
    {
    }
}