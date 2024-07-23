using Hermes.Domain.Entities;
using Hermes.Domain.Interfaces;
using Hermes.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Hermes.Infrastructure.Repositories;

public class ProductRepository(HermesDbContext context) : GenericRepository<Product>(context), IProductRepository
{
    /// <summary>
    /// Retrieves an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <returns>
    /// The entity with the specified ID, or null if no such entity exists.
    /// </returns>
    public new async Task<Product?> GetByIdAsync(int id)
    {
        return await Context.Products
            .Include(p => p.Seller)
            .ThenInclude(x => x.Address)
            .Include(p => p.Reviews)
            .Include(p => p.Category)
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Retrieves entities by their IDs.
    /// </summary>
    /// <param name="ids">The IDs of the entities to retrieve.</param>
    /// <returns>
    /// The entities with the specified IDs, or null if no such entities exist.
    /// </returns>
    public new async Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return await Context.Set<Product>()
            .Include(x => x.Seller)
            .ThenInclude(x => x.Address)
            .Include(x => x.Category)
            .Include(x => x.Variants)
            .Where(x => ids.Contains(x.Id))
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a queryable collection of products associated with a specific category.
    /// </summary>
    /// <param name="categoryId">The ID of the category to filter products by.</param>
    /// <returns>An IQueryable of Product objects representing the products in the category.</returns>
    public IQueryable<Product> GetProductsByCategoryAsync(int categoryId)
    {
        return Context.Products
            .Include(p => p.Seller)
            .ThenInclude(x => x.Address)
            .Where(p => p.CategoryId == categoryId);
    }

    /// <summary>
    /// Retrieves a queryable collection of the top-selling products, sorted by sales volume.
    /// </summary>
    /// <param name="count">The number of top-selling products to retrieve.</param>
    /// <returns>An IQueryable of Product objects representing the top-selling products.</returns>
    public IQueryable<Product> GetTopSellingProductsAsync(int count)
    {
        return Context.Products
            .Include(p => p.Seller)
            .ThenInclude(x => x.Address)
            .OrderByDescending(p => p.OrderItems.Sum(oi => oi.Quantity))
            .Take(count);
    }

    /// <summary>
    /// Retrieves a queryable collection of the latest products, sorted by creation date.
    /// </summary>
    /// <param name="count">The number of latest products to retrieve.</param>
    /// <returns>An IQueryable of Product objects representing the latest products.</returns>
    public IQueryable<Product> GetLatestProductsAsync(int count)
    {
        return Context.Products
            .Include(p => p.Seller)
            .ThenInclude(x => x.Address)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count);
    }

    /// <summary>
    /// Retrieves a collection of product variants associated with a specific product.
    /// </summary>
    /// <param name="productId">The ID of the product to retrieve variants for.</param>
    /// <returns>An IEnumerable of ProductVariant objects representing the product's variants.</returns>
    public async Task<IEnumerable<ProductVariant>> GetProductVariantsAsync(int productId)
    {
        return await Context.ProductVariants
            .Include(pv => pv.Options)
            .Where(pv => pv.ProductId == productId)
            .Include(pv => pv.Product)
            .ThenInclude(p => p.Seller)
            .ThenInclude(x => x.Address)
            .ToListAsync();
    }

    /// <summary>
    /// Adds a new product variant to the repository.
    /// </summary>
    /// <param name="variant">The ProductVariant object to add.</param>
    public async Task AddProductVariantAsync(ProductVariant variant)
    {
        await Context.ProductVariants.AddAsync(variant);
        await Context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates an existing product variant in the repository.
    /// </summary>
    /// <param name="variant">The ProductVariant object to update.</param>
    public async Task UpdateProductVariantAsync(ProductVariant variant)
    {
        Context.ProductVariants.Update(variant);
        await Context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a product variant from the repository.
    /// </summary>
    /// <param name="variant">The ProductVariant object to delete.</param>
    public async Task DeleteProductVariantAsync(ProductVariant variant)
    {
        Context.ProductVariants.Remove(variant);
        await Context.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves a product variant from the repository based on its ID.
    /// </summary>
    /// <param name="variantId">The ID of the product variant to retrieve.</param>
    /// <returns>The retrieved ProductVariant object, or null if no matching variant is found.</returns>
    public async Task<ProductVariant?> GetProductVariantByIdAsync(int variantId)
    {
        return await Context.ProductVariants
            .Include(pv => pv.Options)
            .Include(pv => pv.Product)
            .ThenInclude(p => p.Seller)
            .ThenInclude(x => x.Address)
            .FirstOrDefaultAsync(pv => pv.Id == variantId);
    }

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
    public IQueryable<Product> SearchProductsAsync(string searchTerm, int? categoryId = null, decimal? minPrice = null,
        decimal? maxPrice = null, List<string>? tags = null, string? seller = null, string? sortBy = null,
        string? sortOrder = null)
    {
        var query = Context.Products
            .Include(p => p.Seller)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) ||
                                     p.Description.Contains(searchTerm));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        if (tags is { Count: > 0 })
        {
            query = query.Where(p => p.Tags.Intersect(tags).Any());
        }

        if (!string.IsNullOrEmpty(seller))
        {
            query = query.Where(p => p.Seller.Username.Contains(seller));
        }

        if (!string.IsNullOrEmpty(sortBy))
        {
            query = sortBy.ToUpper() switch
            {
                "NAME" => sortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name),
                "PRICE" => sortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(p => p.Price)
                    : query.OrderBy(p => p.Price),
                _ => sortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(p => p.CreatedAt)
                    : query.OrderBy(p => p.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(p => p.CreatedAt);
        }

        return query;
    }
}