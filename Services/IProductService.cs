using ProductInventory.Models;
using ProductInventory.Shared;
using ProductInventory.ViewModels;

namespace ProductInventory.Services
{
    public interface IProductService
    {
        //Task CreateProductAsync(Product product);
        //Task CreateProductAsync(ProductViewModel product);
        Task<ProductViewModel> CreateProductAsync(ProductViewModel product);

        Task<string?> GetProductNameByNameAsync(string productName);
        
        //Task<Product?> GetProductByNameExcludingIdAsync(string productName, int excludedId);
        Task<ProductViewModel?> GetProductByNameExcludingIdAsync(string productName, int excludedId);
        
        //Task<IEnumerable<Product>> GetProductsAsync(string? searchTerm, string? sortBy, int skip, int take);

        //Task<(IEnumerable<Product> Products, PaginationMetadata Pagination)> GetProductsAsync(string? searchTerm, string? sortBy, int pageNumber, int pageSize);
        Task<(IEnumerable<ProductViewModel> Products, PaginationMetadata Pagination)> GetProductsAsync(string? searchTerm, string? sortBy, int pageNumber, int pageSize);
        
        //Task<Product?> GetProductByIdAsync(int id);
        Task<ProductViewModel?> GetProductByIdAsync(int id);
        
        //Task UpdateProductAsync(Product product);
        Task UpdateProductAsync(ProductViewModel product);
        Task DeleteProductAsync(int id);
    }
}
