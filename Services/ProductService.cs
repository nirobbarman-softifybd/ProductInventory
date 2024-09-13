using ProductInventory.Models;
using ProductInventory.Repositories;
using ProductInventory.Shared;
using ProductInventory.ViewModels;

namespace ProductInventory.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        
        //public async Task CreateProductAsync(Product product)
        //public async Task CreateProductAsync(ProductViewModel productViewModel)
        public async Task<ProductViewModel> CreateProductAsync(ProductViewModel productViewModel)
        {
            //await _productRepository.AddAsync(product);
            //var product = new Product
            //{
            //    Id = productViewModel.Id,
            //    ProductName = productViewModel.ProductName,
            //    Description = productViewModel.Description,
            //    Price = productViewModel.Price,
            //    StockQuantity = productViewModel.StockQuantity
            //};
            var product = MapToEntity(productViewModel);
            await _productRepository.AddAsync(product);
            productViewModel.Id = product.Id;
            return productViewModel;
        }
        public async Task<string?> GetProductNameByNameAsync(string productName)
        {
            return await _productRepository.GetProductNameByNameAsync(productName);
        }
        //public async Task<IEnumerable<Product>> GetProductsAsync(string? searchTerm, string? sortBy, int skip, int take)
        //{
        //    return await _productRepository.GetProductsAsync(searchTerm, sortBy, skip, take);
        //}

        //public async Task<(IEnumerable<Product> Products, PaginationMetadata Pagination)> GetProductsAsync(string? searchTerm, string? sortBy, int pageNumber, int pageSize)
        public async Task<(IEnumerable<ProductViewModel> Products, PaginationMetadata Pagination)> GetProductsAsync(string? searchTerm, string? sortBy, int pageNumber, int pageSize)
        {
            var skip = (pageNumber - 1) * pageSize;
            var products = await _productRepository.GetProductsAsync(searchTerm, sortBy, skip, pageSize);
            var totalCount = await _productRepository.GetTotalCountAsync(searchTerm);

            var productViewModels = products.Select(MapToViewModel);

            var paginationMetadata = new PaginationMetadata
            {
                TotalCount = totalCount,
                //CurrentPageDataCount = products.Count(),
                CurrentPageDataCount = productViewModels.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            //return (products, paginationMetadata);
            return (productViewModels, paginationMetadata);
        }
        //public async Task<Product?> GetProductByIdAsync(int id)
        public async Task<ProductViewModel?> GetProductByIdAsync(int id)
        {
            //return await _productRepository.GetByIdAsync(id);
            var product = await _productRepository.GetByIdAsync(id);

            // Return null if the product is not found
            if (product == null)
            {
                return null;
            }

            //return new ProductViewModel
            //{
            //    Id = product!.Id,
            //    ProductName = product.ProductName,
            //    Description = product.Description,
            //    Price = product.Price,
            //    StockQuantity = product.StockQuantity
            //};
            return MapToViewModel(product);
        }

        //public async Task<Product?> GetProductByNameExcludingIdAsync(string productName, int excludedId)
        public async Task<ProductViewModel?> GetProductByNameExcludingIdAsync(string productName, int excludedId)
        {
            //return await _productRepository.GetProductByNameExcludingIdAsync(productName, excludedId);

            var product = await _productRepository.GetProductByNameExcludingIdAsync(productName, excludedId);
            return product != null ? MapToViewModel(product) : null;
        }

        //public async Task UpdateProductAsync(Product product)
        public async Task UpdateProductAsync(ProductViewModel productViewModel)
        {
            //await _productRepository.UpdateAsync(product);
            //var product = new Product
            //{
            //    Id = productViewModel.Id,
            //    ProductName = productViewModel.ProductName,
            //    Description = productViewModel.Description,
            //    Price = productViewModel.Price,
            //    StockQuantity = productViewModel.StockQuantity
            //};
            //var product = MapToEntity(productViewModel);

            var existingProduct = await _productRepository.GetByIdAsync(productViewModel.Id);

            existingProduct!.ProductName = productViewModel.ProductName;
            existingProduct.Description = productViewModel.Description;
            existingProduct.Price = productViewModel.Price;
            existingProduct.StockQuantity = productViewModel.StockQuantity;
            //await _productRepository.UpdateAsync(product);
            await _productRepository.UpdateAsync(existingProduct);
        }
        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }


        //public async Task UpdateProductAsync(ProductViewModel productViewModel)
        //{
        //    var product = MapToEntity(productViewModel);
        //    await _productRepository.UpdateAsync(product);
        //}

        private ProductViewModel MapToViewModel(Product product)
        {
            return new ProductViewModel
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity
            };
        }

        private Product MapToEntity(ProductViewModel viewModel)
        {
            return new Product
            {
                Id = viewModel.Id,
                ProductName = viewModel.ProductName,
                Description = viewModel.Description,
                Price = viewModel.Price,
                StockQuantity = viewModel.StockQuantity
            };
        }
    }
}
