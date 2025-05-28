using ProductEngine.Application.Models;

namespace ProductEngine.Application.Interfaces;

public interface IProductService
{
    Task<ProductDto> CreateProductAsync(ProductDto productDto);
    Task<ProductDto?> GetProductByIdAsync(string id);
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> UpdateProductAsync(string id, ProductDto productDto);
    Task<bool> DeleteProductAsync(string id);
}

