using Microsoft.AspNetCore.Mvc;
using ProductInventory.Models;
using ProductInventory.Services;
using ProductInventory.Shared;

namespace ProductInventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            var response = new Response<Product>();

            if (!ModelState.IsValid)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Success = false;
                response.Message = "Validation failed.";
                response.Errors = new List<string>();

                foreach (var state in ModelState.Values)
                {
                    foreach (var error in state.Errors)
                    {
                        response.Errors.Add(error.ErrorMessage);
                    }
                }
                return BadRequest(response);
            }

            var productName = await _productService.GetProductNameByNameAsync(product.ProductName!);

            if (productName != null)
            {
                response.StatusCode = StatusCodes.Status409Conflict;
                response.Success = false;
                response.Message = $"A product with the same name '{productName}' already exists.";
                response.Errors = new List<string> { "Product with the same name already exists." };

                return Conflict(response);
            }

            await _productService.CreateProductAsync(product);

            response.StatusCode = StatusCodes.Status201Created;
            response.Success = true;
            response.Message = "Product created successfully.";
            response.Data = product;

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] string? searchTerm = null, [FromQuery] string? sortBy = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize <= 0)
            {
                return BadRequest("Page number must be greater than zero and page size must be greater than zero.");
            }

            var (products, paginationMetadata) = await _productService.GetProductsAsync(searchTerm, sortBy, pageNumber, pageSize);

            var response = new Response<object>
            {
                StatusCode = StatusCodes.Status200OK,
                Success = true,
                Message = paginationMetadata.TotalCount > 0 && paginationMetadata.CurrentPageDataCount > 0 ? "Products retrieved successfully." : "No products found",
                Data = new
                {
                    paginationMetadata,
                    products
                }
            };

            return Ok(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                var notFoundResponse = new Response<Product>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Success = false,
                    Message = $"Product with ID {id} not found.",
                    Data = null
                };

                return NotFound(notFoundResponse);
            }

            var successResponse = new Response<Product>
            {
                StatusCode = StatusCodes.Status200OK,
                Success = true,
                Message = "Product retrieved successfully.",
                Data = product
            };

            return Ok(successResponse);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            if (id != updatedProduct.Id)
            {
                return BadRequest("Product ID in the URL does not match the ID in the request body.");
            }

            var existingProduct = await _productService.GetProductByIdAsync(id);

            if (existingProduct == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            if (!ModelState.IsValid)
            {
                var response = new Response<Product>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Success = false,
                    Message = "Validation failed.",
                    Data = null
                };

                foreach (var state in ModelState.Values)
                {
                    foreach (var error in state.Errors)
                    {
                        response.Errors!.Add(error.ErrorMessage);
                    }
                }
                return BadRequest(response);
            }

            // Check if the new product name already exists (excluding the current product)
            var existingProductWithName = await _productService.GetProductNameByNameAsync(updatedProduct.ProductName!);

            if (existingProductWithName != null && existingProductWithName != existingProduct.ProductName)
            {
                var conflictResponse = new Response<Product>
                {
                    StatusCode = StatusCodes.Status409Conflict,
                    Success = false,
                    Message = $"A product with the name '{updatedProduct.ProductName}' already exists.",
                    Data = null,
                    Errors = new List<string> { "Product with the same name already exists." }
                };

                return Conflict(conflictResponse);
            }

            existingProduct.ProductName = updatedProduct.ProductName;
            existingProduct.Description = updatedProduct.Description;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.StockQuantity = updatedProduct.StockQuantity;

            await _productService.UpdateProductAsync(existingProduct);

            var successResponse = new Response<Product>
            {
                StatusCode = StatusCodes.Status200OK,
                Success = true,
                Message = "Product updated successfully.",
                Data = existingProduct
            };
            return Ok(successResponse);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var existingProduct = await _productService.GetProductByIdAsync(id);

            if (existingProduct == null)
            {
                var notFoundResponse = new Response<Product>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Success = false,
                    Message = $"Product with ID {id} not found.",
                    Data = null
                };

                return NotFound(notFoundResponse);
            }

            await _productService.DeleteProductAsync(id);

            var successResponse = new Response<Product>
            {
                StatusCode = StatusCodes.Status204NoContent,
                Success = true,
                Message = "Product deleted successfully.",
                Data = existingProduct
            };

            return Ok(successResponse);
        }

    }
}
