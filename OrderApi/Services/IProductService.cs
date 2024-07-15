using OrderApi.Dtos;

namespace OrderApi.Services
{
    public interface IProductService
    {
        Task CreateProductAsync(CreateProductDto product);
        Task IncreaseInventoryCount(IncreaseProductInventoryCountDto product);
        Task<GetProductDto> GetProductByIdAsync(Guid id);
        Task BuyProduct(BuyProductDto buyProduct);
    }
}
