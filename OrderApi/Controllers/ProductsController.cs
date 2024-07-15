using Microsoft.AspNetCore.Mvc;
using OrderApi.Data.Models;
using OrderApi.Domain.Exceptions.YourProject.Domain.Exceptions;
using OrderApi.Dtos;
using OrderApi.Services;

namespace OrderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet("GetProductById")]
        public async Task<ActionResult<GetProductDto>> GetProductById(Guid Id)
        {
            try
            {
                return Ok(await _productService.GetProductByIdAsync(Id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex is BusinessException ? ex.Message : string.Empty);
            }
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto product)
        {
            try
            {
                await _productService.CreateProductAsync(product);
                return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex is BusinessException ? (ex as BusinessException).Message : string.Empty);
            }
        }

        [HttpPost("IncreaseInventoryCount")]
        public async Task<IActionResult> IncreaseInventoryCount([FromBody] IncreaseProductInventoryCountDto increasingProduct)
        {
            try
            {
                await _productService.IncreaseInventoryCount(increasingProduct);
                return Ok("Product Inventory Increased successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex is BusinessException ? ex.Message : string.Empty);
            }
        }

        [HttpPost("BuyProduct")]
        public async Task<IActionResult> BuyProduct([FromBody] BuyProductDto buyProduct)
        {
            try
            {
                await _productService.BuyProduct(buyProduct);
                return Ok("Order Inserted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex is BusinessException ? ex.Message : string.Empty);
            }
        }

    }
}
