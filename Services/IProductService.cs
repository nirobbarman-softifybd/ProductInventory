using ProductInventory.Models;
using ProductInventory.Shared;

namespace ProductInventory.Services
{
    public interface IProductService
    {
        Task CreateProductAsync(Product product);
        Task<string?> GetProductNameByNameAsync(string productName);
        //Task<IEnumerable<Product>> GetProductsAsync(string? searchTerm, string? sortBy, int skip, int take);
        Task<(IEnumerable<Product> Products, PaginationMetadata Pagination)> GetProductsAsync(string? searchTerm, string? sortBy, int pageNumber, int pageSize);
        Task<Product?> GetProductByIdAsync(int id);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
    }
}
