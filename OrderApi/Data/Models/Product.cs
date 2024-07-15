using System.ComponentModel.DataAnnotations;

namespace OrderApi.Data.Models;

public class Product
{
    public Guid Id { get; set; }

    [Required] [MaxLength(40)] 
    public string Title { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Inventorycount must be a positive number.")]
    public int InventoryCount { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Price must be a positive number.")]
    public int Price { get; set; }

    [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
    public int Discount { get; set; }
}