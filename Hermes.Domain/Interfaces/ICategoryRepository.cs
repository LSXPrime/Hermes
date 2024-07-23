using Hermes.Domain.Entities;

namespace Hermes.Domain.Interfaces;

/// <summary>
/// Defines the repository interface for managing Category entities.
/// </summary>
public interface ICategoryRepository : IGenericRepository<Category>
{
    /// <summary>
    /// Retrieves a collection of subcategories for a given category ID.
    /// </summary>
    /// <param name="categoryId">The ID of the parent category.</param>
    /// <returns>An IEnumerable of Category objects representing the subcategories.</returns>
    Task<IEnumerable<Category>> GetSubcategoriesAsync(int categoryId);

    /// <summary>
    /// Retrieves a category along with its associated products, based on the provided category ID.
    /// </summary>
    /// <param name="categoryId">The ID of the category to retrieve.</param>
    /// <returns>The retrieved Category object, including its products, or null if no matching category is found.</returns>
    Task<Category?> GetCategoryWithProductsAsync(int categoryId);
}