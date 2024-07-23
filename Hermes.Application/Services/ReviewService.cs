using AutoMapper;
using Hermes.Application.DTOs;
using Hermes.Application.Exceptions;
using Hermes.Application.Interfaces;
using Hermes.Domain.Entities;
using Hermes.Domain.Interfaces;

namespace Hermes.Application.Services;

public class ReviewService(IUnitOfWork unitOfWork, IMapper mapper) : IReviewService
{
    /// <summary>
    /// Retrieves a specific review by its ID.
    /// </summary>
    /// <param name="reviewId">The ID of the review to retrieve.</param>
    /// <returns>A ReviewDto object representing the specified review, or null if no review with the given ID is found.</returns>
    public async Task<ReviewDto?> GetReviewByIdAsync(int reviewId)
    {
        var review = await unitOfWork.Reviews.GetByIdAsync(reviewId);
        if (review == null)
        {
            throw new NotFoundException("Review not found.");
        }
        return mapper.Map<ReviewDto>(review);
    }

    /// <summary>
    /// Creates a new review.
    /// </summary>
    /// <param name="reviewDto">The CreateReviewDto object containing the review data to create.</param>
    /// <returns>A ReviewDto object representing the newly created review.</returns>
    public async Task<ReviewDto?> CreateReviewAsync(CreateReviewDto reviewDto)
    {
        if (!await unitOfWork.Products.ExistsAsync(reviewDto.ProductId))
        {
            throw new NotFoundException("Product not found.");
        }
        
        var review = mapper.Map<Review>(reviewDto);
        await unitOfWork.Reviews.AddAsync(review);
        return mapper.Map<ReviewDto>(review);
    }

    /// <summary>
    /// Retrieves a collection of reviews associated with a specific product.
    /// </summary>
    /// <param name="productId">The ID of the product to retrieve reviews for.</param>
    /// <returns>An IEnumerable of ReviewDto objects representing the product's reviews.</returns>
    public async Task<IEnumerable<ReviewDto>> GetReviewsByProductAsync(int productId)
    {
        if (!await unitOfWork.Products.ExistsAsync(productId))
        {
            throw new NotFoundException("Product not found.");
        }
        
        var reviews = await unitOfWork.Reviews.GetReviewsByProductAsync(productId);
        return mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }

    /// <summary>
    /// Deletes an existing review.
    /// </summary>
    /// <param name="reviewId">The ID of the review to delete.</param>
    /// <returns>True if the review was successfully deleted, false otherwise.</returns>
    public async Task<bool> DeleteReviewAsync(int reviewId)
    {
        var review = await unitOfWork.Reviews.GetByIdAsync(reviewId);
        if (review == null)
        {
            throw new NotFoundException("Review not found.");
        }
        await unitOfWork.Reviews.DeleteAsync(review);
        return true;
    }

    /// <summary>
    /// Updates an existing review.
    /// </summary>
    /// <param name="reviewId">The ID of the review to update.</param>
    /// <param name="reviewDto">The CreateReviewDto object containing the updated review data.</param>
    /// <returns>True if the review was successfully updated, false otherwise.</returns>
    public async Task<bool> UpdateReviewAsync(int reviewId, CreateReviewDto reviewDto)
    {
        var review = await unitOfWork.Reviews.GetByIdAsync(reviewId);
        if (review == null)
        {
            throw new NotFoundException("Review not found.");
        }
        
        mapper.Map(reviewDto, review);
        await unitOfWork.Reviews.UpdateAsync(review);
        return true;
    }

    /// <summary>
    /// Checks if a review exists based on its ID.
    /// </summary>
    /// <param name="reviewId">The ID of the review to check for existence.</param>
    /// <returns>True if the review exists, false otherwise.</returns>
    public async Task<bool> ReviewExistsAsync(int reviewId)
    {
        return await unitOfWork.Reviews.ExistsAsync(reviewId);
    }
}