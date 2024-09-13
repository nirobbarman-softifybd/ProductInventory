using ProductInventory.Models;

namespace ProductInventory.Repositories
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        Task<string?> GetProductNameByNameAsync(string productName);
        Task<IEnumerable<Product>> GetProductsAsync(string? searchTerm, string? sortBy, int skip, int take);
        Task<int> GetTotalCountAsync(string? searchTerm);
        Task<Product?> GetByIdAsync(int id);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}
