using Hermes.Domain.Entities;

namespace Hermes.Domain.Interfaces;

/// <summary>
/// Defines the repository interface for managing Product entities.
/// </summary>
public interface IProductRepository : IGenericRepository<Product>
{
    /// <summary>
    /// Retrieves a queryable collection of products associated with a specific category.
    /// </summary>
    /// <param name="categoryId">The ID of the category to filter products by.</param>
    /// <returns>An IQueryable of Product objects representing the products in the category.</returns>
    IQueryable<Product> GetProductsByCategoryAsync(int categoryId);

    /// <summary>
    /// Retrieves a queryable collection of the top-selling products, sorted by sales volume.
    /// </summary>
    /// <param name="count">The number of top-selling products to retrieve.</param>
    /// <returns>An IQueryable of Product objects representing the top-selling products.</returns>
    IQueryable<Product> GetTopSellingProductsAsync(int count);

    /// <summary>
    /// Retrieves a queryable collection of the latest products, sorted by creation date.
    /// </summary>
    /// <param name="count">The number of latest products to retrieve.</param>
    /// <returns>An IQueryable of Product objects representing the latest products.</returns>
    IQueryable<Product> GetLatestProductsAsync(int count);

    /// <summary>
    /// Retrieves a collection of product variants associated with a specific product.
    /// </summary>
    /// <param name="productId">The ID of the product to retrieve variants for.</param>
    /// <returns>An IEnumerable of ProductVariant objects representing the product's variants.</returns>
    Task<IEnumerable<ProductVariant>> GetProductVariantsAsync(int productId);

    /// <summary>
    /// Adds a new product variant to the repository.
    /// </summary>
    /// <param name="variant">The ProductVariant object to add.</param>
    Task AddProductVariantAsync(ProductVariant variant);

    /// <summary>
    /// Updates an existing product variant in the repository.
    /// </summary>
    /// <param name="variant">The ProductVariant object to update.</param>
    Task UpdateProductVariantAsync(ProductVariant variant);

    /// <summary>
    /// Deletes a product variant from the repository.
    /// </summary>
    /// <param name="variant">The ProductVariant object to delete.</param>
    Task DeleteProductVariantAsync(ProductVariant variant);

    /// <summary>
    /// Retrieves a product variant from the repository based on its ID.
    /// </summary>
    /// <param name="variantId">The ID of the product variant to retrieve.</param>
    /// <returns>The retrieved ProductVariant object, or null if no matching variant is found.</returns>
    Task<ProductVariant?> GetProductVariantByIdAsync(int variantId);

    /// <summary>
    /// Searches for products based on specified criteria.
    /// </summary>
    /// <param name="searchTerm">The search term to use for filtering.</param>
    /// <param name="categoryId">Optional category ID to filter by.</param>
    /// <param name="minPrice">Optional minimum price filter.</param>
    /// <param name="maxPrice">Optional maximum price filter.</param>
    /// <param name="tags">Optional list of tags to filter by.</param>
    /// <param name="seller">Optional seller name to filter by.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortOrder">Optional sort order (ascending or descending).</param>
    /// <returns>An IQueryable of Product objects matching the search criteria.</returns>
    IQueryable<Product> SearchProductsAsync(string searchTerm, int? categoryId = null, decimal? minPrice = null,
        decimal? maxPrice = null, List<string>? tags = null, string? seller = null, string? sortBy = null,
        string? sortOrder = null);
}