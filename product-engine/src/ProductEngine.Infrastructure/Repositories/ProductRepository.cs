using Microsoft.Azure.Cosmos;
using ProductEngine.Application.Interfaces;
using ProductEngine.Domain;

namespace ProductEngine.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly Container _container;

    public ProductRepository(CosmosClient cosmosClient, string databaseId, string containerId)
    {
        _container = cosmosClient.GetContainer(databaseId, containerId);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        var response = await _container.CreateItemAsync(product, new PartitionKey(product.Id));
        return response.Resource;
    }

    public async Task<Product?> GetByIdAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<Product>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        var query = _container.GetItemQueryIterator<Product>("SELECT * FROM c");
        var results = new List<Product>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }
        return results;
    }

    public async Task<Product?> UpdateAsync(string id, Product product)
    {
        var response = await _container.UpsertItemAsync(product, new PartitionKey(id));
        return response.Resource;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        try
        {
            await _container.DeleteItemAsync<Product>(id, new PartitionKey(id));
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}
