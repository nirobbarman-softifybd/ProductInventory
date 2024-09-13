using ProductInventory.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Use the extension method to configure services
builder.Services.AddCorsPolicies();// Call to configure CORS policies
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddServiceServices(); // Configure application services
builder.Services.AddRepositoryServices(); // Configure repositories

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCorsPolicies();

app.UseAuthorization();

app.MapControllers();

app.Run();
