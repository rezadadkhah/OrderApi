using System.ComponentModel.DataAnnotations;

namespace OrderApi.Dtos
{
    public class BuyProductDto
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public Guid BuyerUserId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Buying count must be a positive number.")]
        [Required]
        public int Count { get; set; }
    }
}
