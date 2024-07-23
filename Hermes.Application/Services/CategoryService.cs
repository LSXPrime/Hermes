using AutoMapper;
using Hermes.Application.DTOs;
using Hermes.Application.Interfaces;
using Hermes.Domain.Entities;
using Hermes.Domain.Interfaces;
using Hermes.Application.Exceptions;

namespace Hermes.Application.Services;

public class CategoryService(IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
{
    /// <summary>
    /// Retrieves a collection of all categories.
    /// </summary>
    /// <returns>An IEnumerable of CategoryDto objects representing all categories.</returns>
    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var query = unitOfWork.Categories.GetAllAsync();
        var categories = await unitOfWork.Categories.ExecuteQueryAsync(query);
        return mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    /// <summary>
    /// Retrieves a specific category by its ID.
    /// </summary>
    /// <param name="categoryId">The ID of the category to retrieve.</param>
    /// <returns>A CategoryDto object representing the specified category, or null if no category with the given ID is found.</returns>
    public async Task<CategoryDto?> GetCategoryByIdAsync(int categoryId)
    {
        var category = await unitOfWork.Categories.GetByIdAsync(categoryId);
        return mapper.Map<CategoryDto>(category);
    }

    /// <summary>
    /// Retrieves a collection of subcategories for a given category ID.
    /// </summary>
    /// <param name="categoryId">The ID of the parent category.</param>
    /// <returns>An IEnumerable of CategoryDto objects representing the subcategories.</returns>
    public async Task<IEnumerable<CategoryDto>> GetSubcategoriesAsync(int categoryId)
    {
        var category = await unitOfWork.Categories.ExistsAsync(categoryId);
        if (!category)
            throw new NotFoundException("Category not found.");
        
        var subcategories = await unitOfWork.Categories.GetSubcategoriesAsync(categoryId);
        return mapper.Map<IEnumerable<CategoryDto>>(subcategories);
    }

    /// <summary>
    /// Creates a new category.
    /// </summary>
    /// <param name="categoryDto">The CreateCategoryDto object containing the category data to create.</param>
    /// <returns>A CategoryDto object representing the newly created category.</returns>
    public async Task<CategoryDto?> CreateCategoryAsync(CreateCategoryDto categoryDto)
    {
        var category = mapper.Map<Category>(categoryDto);
        await unitOfWork.Categories.AddAsync(category); 
        return mapper.Map<CategoryDto>(category); 
    }

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    /// <param name="categoryId">The ID of the category to update.</param>
    /// <param name="categoryDto">The UpdateCategoryDto object containing the updated category data.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task UpdateCategoryAsync(int categoryId, UpdateCategoryDto categoryDto)
    {
        var existingCategory = await unitOfWork.Categories.GetByIdAsync(categoryId);

        if (existingCategory == null)
            throw new NotFoundException("Category not found.");

        mapper.Map(categoryDto, existingCategory);
        await unitOfWork.Categories.UpdateAsync(existingCategory);
    }

    /// <summary>
    /// Deletes an existing category.
    /// </summary>
    /// <param name="categoryId">The ID of the category to delete.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task DeleteCategoryAsync(int categoryId)
    {
        var category = await unitOfWork.Categories.GetByIdAsync(categoryId);

        if (category == null)
            throw new NotFoundException("Category not found.");

        await unitOfWork.Categories.DeleteAsync(category);
    }
}