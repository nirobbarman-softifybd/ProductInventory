using Microsoft.EntityFrameworkCore;
using ProductInventory.Data;
using ProductInventory.Models;

namespace ProductInventory.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }
        //public async Task AddAsync(Product product)
        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<string?> GetProductNameByNameAsync(string productName)
        {
            return await _context.Products.Where(p => p.ProductName == productName).Select(p => p.ProductName).FirstOrDefaultAsync();
        }

        //public async Task<Product?> GetProductByNameExcludingIdAsync(string productName, int excludedId)
        public async Task<Product?> GetProductByNameExcludingIdAsync(string productName, int excludedId)
        {
            return await _context.Products
                .Where(p => p.ProductName == productName && p.Id != excludedId)
                .FirstOrDefaultAsync();
        }
        //public async Task<IEnumerable<Product>> GetProductsAsync(string? searchTerm, string? sortBy, int skip, int take)
        //{
        //    var query = _context.Products.AsQueryable();

        //    if (!string.IsNullOrWhiteSpace(searchTerm))
        //    {
        //        query = query.Where(p => p.ProductName!.Contains(searchTerm));
        //    }

        //    if (sortBy == "price")
        //    {
        //        query = query.OrderBy(p => p.Price);
        //    }
        //    else if (sortBy == "price_desc")
        //    {
        //        query = query.OrderByDescending(p => p.Price);
        //    }
        //    else if (sortBy == "name")
        //    {
        //        query = query.OrderBy(p => p.ProductName);
        //    }
        //    else if (sortBy == "name_desc")
        //    {
        //        query = query.OrderByDescending(p => p.ProductName);
        //    }

        //    return await query.Skip(skip).Take(take).ToListAsync();
        //}

        public async Task<IEnumerable<Product>> GetProductsAsync(string? searchTerm, string? sortBy, int skip, int take)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.ProductName!.Contains(searchTerm));
            }

            query = sortBy switch
            {
                "price" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "name" => query.OrderBy(p => p.ProductName),
                "name_desc" => query.OrderByDescending(p => p.ProductName),
                _ => query
            };

            return await query.Skip(skip).Take(take).ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(string? searchTerm)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.ProductName!.Contains(searchTerm));
            }
            return await query.CountAsync();
        }
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
