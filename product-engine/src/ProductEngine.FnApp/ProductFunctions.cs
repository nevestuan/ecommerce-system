using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using ProductEngine.Application.Interfaces;
using ProductEngine.Application.Models;

namespace ProductEngine.FnApp;

public class ProductFunctions
{
    private readonly IProductService _service;
    private readonly ILogger _logger;

    public ProductFunctions(IProductService service, ILoggerFactory loggerFactory)
    {
        _service = service;
        _logger = loggerFactory.CreateLogger<ProductFunctions>();
    }

    [Function("CreateProduct")]
    public async Task<HttpResponseData> CreateProduct(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "products")] HttpRequestData req)
    {
        var product = await req.ReadFromJsonAsync<ProductDto>();
        if (product == null)
            return req.CreateResponse(HttpStatusCode.BadRequest);
        var created = await _service.CreateProductAsync(product);
        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(created);
        return response;
    }

    [Function("GetProductById")]
    public async Task<HttpResponseData> GetProductById(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products/{id}")] HttpRequestData req,
        string id)
    {
        var product = await _service.GetProductByIdAsync(id);
        if (product == null)
            return req.CreateResponse(HttpStatusCode.NotFound);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(product);
        return response;
    }

    [Function("GetAllProducts")]
    public async Task<HttpResponseData> GetAllProducts(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products")] HttpRequestData req)
    {
        var products = await _service.GetAllProductsAsync();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(products);
        return response;
    }

    [Function("UpdateProduct")]
    public async Task<HttpResponseData> UpdateProduct(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "products/{id}")] HttpRequestData req,
        string id)
    {
        var product = await req.ReadFromJsonAsync<ProductDto>();
        if (product == null)
            return req.CreateResponse(HttpStatusCode.BadRequest);
        var updated = await _service.UpdateProductAsync(id, product);
        if (updated == null)
            return req.CreateResponse(HttpStatusCode.NotFound);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(updated);
        return response;
    }

    [Function("DeleteProduct")]
    public async Task<HttpResponseData> DeleteProduct(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "products/{id}")] HttpRequestData req,
        string id)
    {
        var deleted = await _service.DeleteProductAsync(id);
        var response = req.CreateResponse(deleted ? HttpStatusCode.NoContent : HttpStatusCode.NotFound);
        return response;
    }
}
