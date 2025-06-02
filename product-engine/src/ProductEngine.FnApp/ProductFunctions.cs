using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using ProductEngine.Application.Interfaces;
using ProductEngine.Application.Models;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

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
    [OpenApiOperation(operationId: "CreateProduct", tags: new[] { "Product" }, Summary = "Create a new product", Description = "Creates a new product.")]
    [OpenApiRequestBody("application/json", typeof(ProductDto), Description = "Product to create", Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(ProductDto), Description = "The created product")] 
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "Invalid input")]
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
    [OpenApiOperation(operationId: "GetProductById", tags: new[] { "Product" }, Summary = "Get product by ID", Description = "Gets a product by its ID.")]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Product ID", Description = "The ID of the product to retrieve")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ProductDto), Description = "The product")] 
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Product not found")]
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
    [OpenApiOperation(operationId: "GetAllProducts", tags: new[] { "Product" }, Summary = "Get all products", Description = "Gets all products.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<ProductDto>), Description = "List of products")]
    public async Task<HttpResponseData> GetAllProducts(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products")] HttpRequestData req)
    {
        var products = await _service.GetAllProductsAsync();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(products);
        return response;
    }

    [Function("UpdateProduct")]
    [OpenApiOperation(operationId: "UpdateProduct", tags: new[] { "Product" }, Summary = "Update a product", Description = "Updates an existing product.")]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Product ID", Description = "The ID of the product to update")]
    [OpenApiRequestBody("application/json", typeof(ProductDto), Description = "Product data to update", Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ProductDto), Description = "The updated product")] 
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "Invalid input")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Product not found")]
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
    [OpenApiOperation(operationId: "DeleteProduct", tags: new[] { "Product" }, Summary = "Delete a product", Description = "Deletes a product by its ID.")]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Product ID", Description = "The ID of the product to delete")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NoContent, Description = "Product deleted successfully")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Product not found")]
    public async Task<HttpResponseData> DeleteProduct(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "products/{id}")] HttpRequestData req,
        string id)
    {
        var deleted = await _service.DeleteProductAsync(id);
        var response = req.CreateResponse(deleted ? HttpStatusCode.NoContent : HttpStatusCode.NotFound);
        return response;
    }
}
