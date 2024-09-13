namespace ProductInventory.Configurations
{
    public static class CorsConfigurations
    {
        public static void AddCorsPolicies(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });
        }

        public static void UseCorsPolicies(this IApplicationBuilder app)
        {
            app.UseCors();
        }
    }
}
