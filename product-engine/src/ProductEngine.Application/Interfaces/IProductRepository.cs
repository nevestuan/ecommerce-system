using ProductEngine.Domain;

namespace ProductEngine.Application.Interfaces;

public interface IProductRepository
{
    Task<Product> CreateAsync(Product product);
    Task<Product?> GetByIdAsync(string id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> UpdateAsync(string id, Product product);
    Task<bool> DeleteAsync(string id);
}