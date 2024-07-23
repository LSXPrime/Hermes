using Hermes.API.Attributes;
using Hermes.Application.DTOs;
using Hermes.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hermes.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(ICategoryService categoryService) : ControllerBaseEx
{
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await categoryService.GetCategoryByIdAsync(id);
        return category == null ? NotFound() : Ok(category);
    }

    [HttpGet("{id:int}/subcategories")]
    public async Task<IActionResult> GetSubcategories(int id)
    {
        var subcategories = await categoryService.GetSubcategoriesAsync(id);
        return Ok(subcategories);
    }

    [AuthorizeMiddleware(["Admin"])]
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto categoryDto)
    {
        var newCategory = await categoryService.CreateCategoryAsync(categoryDto);
        return newCategory != null
            ? CreatedAtAction(nameof(GetCategoryById), new { id = newCategory.Id }, newCategory)
            : BadRequest("Failed to create category");
    }

    [AuthorizeMiddleware(["Admin"])]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto categoryDto)
    {
        if (categoryDto.Id != id)
        {
            return BadRequest("Category ID in request body does not match ID in URL.");
        }

        await categoryService.UpdateCategoryAsync(id, categoryDto);
        return NoContent();
    }

    [AuthorizeMiddleware(["Admin"])]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        await categoryService.DeleteCategoryAsync(id);
        return NoContent();
    }
}