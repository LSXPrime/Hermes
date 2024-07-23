using Hermes.Application.DTOs;

namespace Hermes.Application.Interfaces;

/// <summary>
/// Defines the interface for a service that manages Product operations.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Retrieves a paged collection of all products.
    /// </summary>
    /// <param name="page">The page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
    /// <returns>A PagedResult object containing the retrieved products and pagination information.</returns>
    Task<PagedResult<ProductDto>> GetAllProductsAsync(int page, int pageSize);

    /// <summary>
    /// Retrieves a specific product by its ID.
    /// </summary>
    /// <param name="productId">The ID of the product to retrieve.</param>
    /// <returns>A ProductDto object representing the specified product, or null if no product with the given ID is found.</returns>
    Task<ProductDto?> GetProductByIdAsync(int productId);

    /// <summary>
    /// Retrieves a paged collection of products associated with a specific category.
    /// </summary>
    /// <param name="categoryId">The ID of the category to filter products by.</param>
    /// <param name="page">The page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
    /// <returns>A PagedResult object containing the retrieved products and pagination information.</returns>
    Task<PagedResult<ProductDto>> GetProductsByCategoryAsync(int categoryId, int page, int pageSize);

    /// <summary>
    /// Searches for products based on specified criteria.
    /// </summary>
    /// <param name="searchTerm">The search term to use for filtering.</param>
    /// <param name="page">The page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
    /// <param name="categoryId">Optional category ID to filter by.</param>
    /// <param name="minPrice">Optional minimum price filter.</param>
    /// <param name="maxPrice">Optional maximum price filter.</param>
    /// <param name="tags">Optional list of tags to filter by.</param>
    /// <param name="seller">Optional seller name to filter by.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortOrder">Optional sort order (ascending or descending).</param>
    /// <returns>A PagedResult object containing the retrieved products and pagination information.</returns>
    Task<PagedResult<ProductDto>> SearchProductsAsync(string searchTerm, int page, int pageSize, int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null, List<string>? tags = null, string? seller = null, string? sortBy = null,
        string? sortOrder = null);

    /// <summary>
    /// Retrieves a paged collection of the top-selling products.
    /// </summary>
    /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
    /// <returns>A PagedResult object containing the retrieved products and pagination information.</returns>
    Task<PagedResult<ProductDto>> GetTopSellingProductsAsync(int pageSize);

    /// <summary>
    /// Retrieves a paged collection of the latest products.
    /// </summary>
    /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
    /// <returns>A PagedResult object containing the retrieved products and pagination information.</returns>
    Task<PagedResult<ProductDto>> GetLatestProductsAsync(int pageSize);

    /// <summary>
    /// Retrieves detailed information for a specific product.
    /// </summary>
    /// <param name="productId">The ID of the product to retrieve details for.</param>
    /// <returns>The retrieved ProductDto object, or null if no matching product is found.</returns>
    Task<ProductDto?> GetProductDetailsAsync(int productId);

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="productDto">The CreateProductDto object containing the product data to create.</param>
    /// <returns>A ProductDto object representing the newly created product.</returns>
    Task<ProductDto?> CreateProductAsync(CreateProductDto productDto); 

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="productId">The ID of the product to update.</param>
    /// <param name="productDto">The UpdateProductDto object containing the updated product data.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task UpdateProductAsync(int productId, UpdateProductDto productDto);

    /// <summary>
    /// Deletes an existing product.
    /// </summary>
    /// <param name="productId">The ID of the product to delete.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task DeleteProductAsync(int productId);

    /// <summary>
    /// Retrieves a product based on its associated product variant ID.
    /// </summary>
    /// <param name="variantId">The ID of the product variant associated with the product to retrieve.</param>
    /// <returns>The retrieved ProductDto object, or null if no matching product is found.</returns>
    Task<ProductDto?> GetProductByVariantIdAsync(int variantId);

    /// <summary>
    /// Retrieves a collection of product variants associated with a specific product.
    /// </summary>
    /// <param name="productId">The ID of the product to retrieve variants for.</param>
    /// <returns>An IEnumerable of ProductVariantDto objects representing the product's variants.</returns>
    Task<IEnumerable<ProductVariantDto>> GetProductVariantsAsync(int productId);

    /// <summary>
    /// Retrieves a specific product variant by its ID.
    /// </summary>
    /// <param name="variantId">The ID of the product variant to retrieve.</param>
    /// <returns>A ProductVariantDto object representing the specified product variant, or null if no variant with the given ID is found.</returns>
    Task<ProductVariantDto?> GetProductVariantByIdAsync(int variantId);

    /// <summary>
    /// Creates a new product variant for a specific product.
    /// </summary>
    /// <param name="productId">The ID of the product to create a variant for.</param>
    /// <param name="variantDto">The CreateProductVariantDto object containing the variant data to create.</param>
    /// <returns>A ProductVariantDto object representing the newly created variant.</returns>
    Task<ProductVariantDto?> CreateProductVariantAsync(int productId, CreateProductVariantDto variantDto);

    /// <summary>
    /// Updates an existing product variant.
    /// </summary>
    /// <param name="variantId">The ID of the product variant to update.</param>
    /// <param name="variantDto">The UpdateProductVariantDto object containing the updated variant data.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task UpdateProductVariantAsync(int variantId, UpdateProductVariantDto variantDto);

    /// <summary>
    /// Deletes an existing product variant.
    /// </summary>
    /// <param name="variantId">The ID of the product variant to delete.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task DeleteProductVariantAsync(int variantId);
}