using Hermes.Domain.Entities;

namespace Hermes.Domain.Interfaces;

/// <summary>
/// Defines the repository interface for managing Review entities.
/// </summary>
public interface IReviewRepository : IGenericRepository<Review>
{
    /// <summary>
    /// Retrieves a collection of reviews associated with a specific product.
    /// </summary>
    /// <param name="productId">The ID of the product to retrieve reviews for.</param>
    /// <returns>An IEnumerable of Review objects representing the product's reviews.</returns>
    Task<IEnumerable<Review>> GetReviewsByProductAsync(int productId);

    /// <summary>
    /// Retrieves a collection of reviews written by a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve reviews for.</param>
    /// <returns>An IEnumerable of Review objects representing the user's reviews.</returns>
    Task<IEnumerable<Review>> GetReviewsByUserAsync(int userId);
}