using Hermes.API.Attributes;
using Hermes.Application.DTOs;
using Hermes.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hermes.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController(IReviewService reviewService) : ControllerBaseEx
{
    [AuthorizeMiddleware(["User", "Seller", "Admin"])]
    [HttpPost("products/{productId:int}")]
    public async Task<IActionResult> CreateReview(int productId, [FromBody] CreateReviewDto reviewDto)
    {
        reviewDto.ProductId = productId;
        reviewDto.UserId = CurrentUserId; 

        var newReview = await reviewService.CreateReviewAsync(reviewDto);
        return newReview != null
            ? CreatedAtAction(nameof(GetReviewById), new { id = newReview.Id }, newReview)
            : BadRequest("Failed to create review.");
    }

    [HttpGet("products/{productId:int}")]
    public async Task<IActionResult> GetReviewsForProduct(int productId)
    {
        var reviews = await reviewService.GetReviewsByProductAsync(productId);
        return Ok(reviews);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetReviewById(int id)
    {
        var review = await reviewService.GetReviewByIdAsync(id);
        return review != null ? Ok(review) : NotFound();
    }

    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateReview(int id, [FromBody] CreateReviewDto reviewDto)
    {
        var review = await reviewService.GetReviewByIdAsync(id);

        if (review == null || (review.UserId != CurrentUserId && CurrentUserRole != "Admin"))
        {
            return Forbid(); 
        }

        return await reviewService.UpdateReviewAsync(id, reviewDto)
            ? NoContent()
            : BadRequest($"Failed to update review with ID {id}.");
    }

    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var review = await reviewService.GetReviewByIdAsync(id);

        if (review == null || (review.UserId != CurrentUserId && CurrentUserRole != "Admin"))
            return Forbid();

        return await reviewService.DeleteReviewAsync(id)
            ? NoContent()
            : BadRequest($"Failed to delete review with ID {id}.");
    }
}