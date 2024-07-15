namespace OrderApi.Data.Models;

public class Order
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public DateTime CreationDate { get; set; }
    public Guid BuyerId { get; set; }
    public User Buyer { get; set; }
}