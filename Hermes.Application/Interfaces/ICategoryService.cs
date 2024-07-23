using Hermes.Application.DTOs;

namespace Hermes.Application.Interfaces;

/// <summary>
/// Defines the interface for a service that manages Category operations.
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Retrieves a collection of all categories.
    /// </summary>
    /// <returns>An IEnumerable of CategoryDto objects representing all categories.</returns>
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();

    /// <summary>
    /// Retrieves a specific category by its ID.
    /// </summary>
    /// <param name="categoryId">The ID of the category to retrieve.</param>
    /// <returns>A CategoryDto object representing the specified category, or null if no category with the given ID is found.</returns>
    Task<CategoryDto?> GetCategoryByIdAsync(int categoryId);

    /// <summary>
    /// Retrieves a collection of subcategories for a given category ID.
    /// </summary>
    /// <param name="categoryId">The ID of the parent category.</param>
    /// <returns>An IEnumerable of CategoryDto objects representing the subcategories.</returns>
    Task<IEnumerable<CategoryDto>> GetSubcategoriesAsync(int categoryId);

    /// <summary>
    /// Creates a new category.
    /// </summary>
    /// <param name="categoryDto">The CreateCategoryDto object containing the category data to create.</param>
    /// <returns>A CategoryDto object representing the newly created category.</returns>
    Task<CategoryDto?> CreateCategoryAsync(CreateCategoryDto categoryDto);

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    /// <param name="categoryId">The ID of the category to update.</param>
    /// <param name="categoryDto">The UpdateCategoryDto object containing the updated category data.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task UpdateCategoryAsync(int categoryId, UpdateCategoryDto categoryDto);

    /// <summary>
    /// Deletes an existing category.
    /// </summary>
    /// <param name="categoryId">The ID of the category to delete.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task DeleteCategoryAsync(int categoryId); 
}