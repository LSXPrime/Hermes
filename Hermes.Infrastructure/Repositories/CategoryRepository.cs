using Hermes.Domain.Entities;
using Hermes.Domain.Interfaces;
using Hermes.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Hermes.Infrastructure.Repositories;

public class CategoryRepository(HermesDbContext context) : GenericRepository<Category>(context), ICategoryRepository
{
    /// <summary>
    /// Retrieves a collection of subcategories for a given category ID.
    /// </summary>
    /// <param name="categoryId">The ID of the parent category.</param>
    /// <returns>An IEnumerable of Category objects representing the subcategories.</returns>
    public async Task<IEnumerable<Category>> GetSubcategoriesAsync(int categoryId)
    {
        return (await Context.Categories
            .Where(c => c.ParentCategoryId == categoryId)
            .ToListAsync());
    }

    /// <summary>
    /// Retrieves a category along with its associated products, based on the provided category ID.
    /// </summary>
    /// <param name="categoryId">The ID of the category to retrieve.</param>
    /// <returns>The retrieved Category object, including its products, or null if no matching category is found.</returns>
    public async Task<Category?> GetCategoryWithProductsAsync(int categoryId)
    {
        return await Context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == categoryId);
    }
}