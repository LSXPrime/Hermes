using Hermes.API.Attributes;
using Hermes.Application.DTOs;
using Hermes.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hermes.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IProductService productService, IImageHelper imageHelper) : ControllerBaseEx
{
    [HttpGet]
    public async Task<IActionResult> GetAllProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var products = await productService.GetAllProductsAsync(page, pageSize);
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await productService.GetProductDetailsAsync(id);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpGet("category/{categoryId:int}")]
    public async Task<IActionResult> GetProductsByCategory(
        int categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var products = await productService.GetProductsByCategoryAsync(categoryId, page, pageSize);
        return Ok(products);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts(
        [FromQuery] string searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? categoryId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] List<string>? tags = null,
        [FromQuery] string? seller = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null
    )
    {
        var products = await productService.SearchProductsAsync(searchTerm, page, pageSize, categoryId, minPrice,
            maxPrice, tags, seller, sortBy, sortOrder);
        return Ok(products);
    }

    [HttpGet("top-selling")]
    public async Task<IActionResult> GetTopSellingProducts([FromQuery] int pageSize = 10)
    {
        var products = await productService.GetTopSellingProductsAsync(pageSize);
        return Ok(products);
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestProducts([FromQuery] int pageSize = 10)
    {
        var products = await productService.GetLatestProductsAsync(pageSize);
        return Ok(products);
    }

    [AuthorizeMiddleware(["Admin", "Seller"])]
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
    {
        if (productDto.SellerId != CurrentUserId && CurrentUserRole != "Admin")
            return Forbid();

        if (CurrentUserRole == "Seller")
            productDto.SellerId = CurrentUserId;
        
        var newProduct = await productService.CreateProductAsync(productDto);
        return newProduct != null
            ? CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct)
            : BadRequest("Failed to create product");
    }

    [AuthorizeMiddleware(["Admin", "Seller"])]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto productDto)
    {
        var product = await productService.GetProductByIdAsync(id);
        if (product == null || (product.SellerId != CurrentUserId && CurrentUserRole != "Admin"))
            return Forbid();

        await productService.UpdateProductAsync(id, productDto);
        return NoContent();
    }

    [AuthorizeMiddleware(["Admin", "Seller"])]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await productService.GetProductByIdAsync(id);
        if (product == null || (product.SellerId != CurrentUserId && CurrentUserRole != "Admin"))
            return Forbid();

        await productService.DeleteProductAsync(id);
        return NoContent();
    }

    [AuthorizeMiddleware(["Admin", "Seller"])]
    [HttpPost("variants/{productId:int}")]
    public async Task<IActionResult> CreateProductVariant(int productId, [FromBody] CreateProductVariantDto variantDto)
    {
        var newVariant = await productService.CreateProductVariantAsync(productId, variantDto);
        return newVariant != null
            ? CreatedAtAction(nameof(GetProductById), new { id = newVariant.Id }, newVariant)
            : BadRequest("Failed to create product variant");
    }

    [AuthorizeMiddleware(["Admin", "Seller"])]
    [HttpPut("variants/{variantId:int}")]
    public async Task<IActionResult> UpdateProductVariant(int variantId, [FromBody] UpdateProductVariantDto variantDto)
    {
        var product = await productService.GetProductByVariantIdAsync(variantId);
        if (product == null || (product.SellerId != CurrentUserId && CurrentUserRole != "Admin"))
            return Forbid();

        await productService.UpdateProductVariantAsync(variantId, variantDto);
        return NoContent();
    }

    [AuthorizeMiddleware(["Admin", "Seller"])]
    [HttpDelete("variants/{variantId:int}")]
    public async Task<IActionResult> DeleteProductVariant(int variantId)
    {
        var product = await productService.GetProductByVariantIdAsync(variantId);
        if (product == null || (product.SellerId != CurrentUserId && CurrentUserRole != "Admin"))
            return Forbid();

        await productService.DeleteProductVariantAsync(variantId);
        return NoContent();
    }

    [AuthorizeMiddleware(["Admin", "Seller"])]
    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadProductImage([FromForm] IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            return BadRequest("Image file is required.");
        }

        var imageUrl =
            await imageHelper.ProcessAndUploadImageAsync(imageFile.OpenReadStream(), imageFile.FileName,
                "products-images");
        return Ok(new { ImageUrl = imageUrl });
    }
}