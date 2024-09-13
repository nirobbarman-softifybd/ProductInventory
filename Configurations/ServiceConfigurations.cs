using Microsoft.EntityFrameworkCore;
using ProductInventory.Data;
using ProductInventory.Repositories;
using ProductInventory.Services;

namespace ProductInventory.Configurations
{
    public static class ServiceConfigurations
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext with the connection string
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Add other services here as needed

            return services;
        }

        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            // Register the ProductRepository and IProductRepository
            services.AddScoped<IProductRepository, ProductRepository>();

            // Add other repositories here as needed

            return services;
        }

        public static IServiceCollection AddServiceServices(this IServiceCollection services)
        {
            // Register the ProductService and IProductService
            services.AddScoped<IProductService, ProductService>();

            // Add other services here as needed

            return services;
        }
    }
}
