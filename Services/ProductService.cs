using ProductInventory.Models;
using ProductInventory.Repositories;
using ProductInventory.Shared;

namespace ProductInventory.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task CreateProductAsync(Product product)
        {
            await _productRepository.AddAsync(product);
        }
        public async Task<string?> GetProductNameByNameAsync(string productName)
        {
            return await _productRepository.GetProductNameByNameAsync(productName);
        }
        //public async Task<IEnumerable<Product>> GetProductsAsync(string? searchTerm, string? sortBy, int skip, int take)
        //{
        //    return await _productRepository.GetProductsAsync(searchTerm, sortBy, skip, take);
        //}

        public async Task<(IEnumerable<Product> Products, PaginationMetadata Pagination)> GetProductsAsync(string? searchTerm, string? sortBy, int pageNumber, int pageSize)
        {
            var skip = (pageNumber - 1) * pageSize;
            var products = await _productRepository.GetProductsAsync(searchTerm, sortBy, skip, pageSize);
            var totalCount = await _productRepository.GetTotalCountAsync(searchTerm);

            var paginationMetadata = new PaginationMetadata
            {
                TotalCount = totalCount,
                CurrentPageDataCount = products.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return (products, paginationMetadata);
        }
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }
        public async Task UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateAsync(product);
        }
        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }
    }
}
