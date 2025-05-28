using ProductEngine.Application.Interfaces;
using ProductEngine.Application.Models;
using ProductEngine.Domain;

namespace ProductEngine.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto> CreateProductAsync(ProductDto productDto)
    {
        var product = new Product
        {
            Id = string.IsNullOrEmpty(productDto.Id) ? Guid.NewGuid().ToString() : productDto.Id,
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            Stock = productDto.Stock
        };
        var created = await _repository.CreateAsync(product);
        return ToDto(created);
    }

    public async Task<ProductDto?> GetProductByIdAsync(string id)
    {
        var product = await _repository.GetByIdAsync(id);
        return product == null ? null : ToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _repository.GetAllAsync();
        return products.Select(ToDto);
    }

    public async Task<ProductDto?> UpdateProductAsync(string id, ProductDto productDto)
    {
        var product = new Product
        {
            Id = id,
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            Stock = productDto.Stock
        };
        var updated = await _repository.UpdateAsync(id, product);
        return updated == null ? null : ToDto(updated);
    }

    public async Task<bool> DeleteProductAsync(string id)
    {
        return await _repository.DeleteAsync(id);
    }

    private static ProductDto ToDto(Product product) => new ProductDto
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        Stock = product.Stock
    };
}
