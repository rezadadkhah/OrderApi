using System.ComponentModel.DataAnnotations;

namespace OrderApi.Dtos
{
    public class IncreaseProductInventoryCountDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Inventorycount must be a positive number.")]
        public int InventoryCount { get; set; }
        
    }
}
