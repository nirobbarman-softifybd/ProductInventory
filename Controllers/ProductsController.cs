using Microsoft.AspNetCore.Mvc;
using ProductInventory.Models;
using ProductInventory.Services;
using ProductInventory.Shared;
using ProductInventory.ViewModels;

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
        //public async Task<IActionResult> CreateProduct([FromBody] Product product)
        public async Task<IActionResult> CreateProduct([FromBody] ProductViewModel productViewModel)
        {
            //var response = new Response<Product>();
            var response = new Response<ProductViewModel>();

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

            var productName = await _productService.GetProductNameByNameAsync(productViewModel.ProductName!);

            if (productName != null)
            {
                response.StatusCode = StatusCodes.Status409Conflict;
                response.Success = false;
                response.Message = $"A product with the same name '{productName}' already exists.";
                response.Errors = new List<string> { "Product with the same name already exists." };

                return Conflict(response);
            }

            var product = await _productService.CreateProductAsync(productViewModel);

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
                //var notFoundResponse = new Response<Product>
                var notFoundResponse = new Response<ProductViewModel>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Success = false,
                    Message = $"Product with ID {id} not found.",
                    Data = null
                };

                return NotFound(notFoundResponse);
            }

            //var successResponse = new Response<Product>
            var successResponse = new Response<ProductViewModel>
            {
                StatusCode = StatusCodes.Status200OK,
                Success = true,
                Message = "Product retrieved successfully.",
                Data = product
            };

            return Ok(successResponse);
        }


        [HttpPut("{id}")]
        //public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductViewModel updatedProduct)
        {
            if (id != updatedProduct.Id)
            {
                //return BadRequest("Product ID in the URL does not match the ID in the request body.");
                var idMismatchResponse = new Response<ProductViewModel>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Success = false,
                    Message = "Product ID in the URL does not match the ID in the request body.",
                    Data = null
                };
                return BadRequest(idMismatchResponse);
            }

            var existingProduct = await _productService.GetProductByIdAsync(id);

            if (existingProduct == null)
            {
                var notFoundResponse = new Response<ProductViewModel>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Success = false,
                    Message = $"Product with ID {id} not found.",
                    Data = null
                };
                //return NotFound($"Product with ID {id} not found.");

                return NotFound(notFoundResponse);
            }

            if (!ModelState.IsValid)
            {
                //var response = new Response<Product>
                var response = new Response<ProductViewModel>
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

            // Check if the new product name already exists (excluding the current product ID)
            var existingProductWithName = await _productService.GetProductByNameExcludingIdAsync(updatedProduct.ProductName!, id);

            if (existingProductWithName != null)
            {
                //var conflictResponse = new Response<Product>
                var conflictResponse = new Response<ProductViewModel>
                {
                    StatusCode = StatusCodes.Status409Conflict,
                    Success = false,
                    //Message = $"A product with the name '{updatedProduct.ProductName}' already exists.",
                    Message = $"A product with the name '{updatedProduct.ProductName}' already exists with a different ID. Existing ID: {existingProductWithName.Id}.",
                    Data = null,
                    Errors = new List<string> { "Product with the same name already exists." }
                };

                return Conflict(conflictResponse);
            }

            //existingProduct.ProductName = updatedProduct.ProductName;
            //existingProduct.Description = updatedProduct.Description;
            //existingProduct.Price = updatedProduct.Price;
            //existingProduct.StockQuantity = updatedProduct.StockQuantity;

            //await _productService.UpdateProductAsync(existingProduct);
            await _productService.UpdateProductAsync(updatedProduct);

            //var successResponse = new Response<Product>
            var successResponse = new Response<ProductViewModel>
            {
                StatusCode = StatusCodes.Status200OK,
                Success = true,
                Message = "Product updated successfully.",
                //Data = existingProduct
                Data = updatedProduct
            };
            return Ok(successResponse);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var existingProduct = await _productService.GetProductByIdAsync(id);

            if (existingProduct == null)
            {
                //var notFoundResponse = new Response<Product>
                var notFoundResponse = new Response<ProductViewModel>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Success = false,
                    Message = $"Product with ID {id} not found.",
                    Data = null
                };

                return NotFound(notFoundResponse);
            }

            await _productService.DeleteProductAsync(id);

            //var successResponse = new Response<Product>
            var successResponse = new Response<ProductViewModel>
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
