using Hermes.Application.DTOs;

namespace Hermes.Application.Interfaces;

/// <summary>
/// Defines the interface for a service that manages Review operations.
/// </summary>
public interface IReviewService
{
    /// <summary>
    /// Retrieves a specific review by its ID.
    /// </summary>
    /// <param name="reviewId">The ID of the review to retrieve.</param>
    /// <returns>A ReviewDto object representing the specified review, or null if no review with the given ID is found.</returns>
    Task<ReviewDto?> GetReviewByIdAsync(int reviewId);

    /// <summary>
    /// Creates a new review.
    /// </summary>
    /// <param name="reviewDto">The CreateReviewDto object containing the review data to create.</param>
    /// <returns>A ReviewDto object representing the newly created review.</returns>
    Task<ReviewDto?> CreateReviewAsync(CreateReviewDto reviewDto);

    /// <summary>
    /// Retrieves a collection of reviews associated with a specific product.
    /// </summary>
    /// <param name="productId">The ID of the product to retrieve reviews for.</param>
    /// <returns>An IEnumerable of ReviewDto objects representing the product's reviews.</returns>
    Task<IEnumerable<ReviewDto>> GetReviewsByProductAsync(int productId);

    /// <summary>
    /// Deletes an existing review.
    /// </summary>
    /// <param name="reviewId">The ID of the review to delete.</param>
    /// <returns>True if the review was successfully deleted, false otherwise.</returns>
    Task<bool> DeleteReviewAsync(int reviewId);

    /// <summary>
    /// Updates an existing review.
    /// </summary>
    /// <param name="reviewId">The ID of the review to update.</param>
    /// <param name="reviewDto">The CreateReviewDto object containing the updated review data.</param>
    /// <returns>True if the review was successfully updated, false otherwise.</returns>
    Task<bool> UpdateReviewAsync(int reviewId, CreateReviewDto reviewDto);

    /// <summary>
    /// Checks if a review exists based on its ID.
    /// </summary>
    /// <param name="reviewId">The ID of the review to check for existence.</param>
    /// <returns>True if the review exists, false otherwise.</returns>
    Task<bool> ReviewExistsAsync(int reviewId);
}