using System.ComponentModel.DataAnnotations;

namespace OrderApi.Dtos
{
    public class GetProductDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int InventoryCount { get; set; }
        public int Price { get; set; }
        
    }
}
