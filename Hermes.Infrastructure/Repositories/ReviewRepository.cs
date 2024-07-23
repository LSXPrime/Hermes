using Hermes.Domain.Entities;
using Hermes.Domain.Interfaces;
using Hermes.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Hermes.Infrastructure.Repositories;

public class ReviewRepository(HermesDbContext context) : GenericRepository<Review>(context), IReviewRepository
{
    /// <summary>
    /// Retrieves a collection of reviews associated with a specific product.
    /// </summary>
    /// <param name="productId">The ID of the product to retrieve reviews for.</param>
    /// <returns>An IEnumerable of Review objects representing the product's reviews.</returns>
    public async Task<IEnumerable<Review>> GetReviewsByProductAsync(int productId)
    {
        return await Context.Reviews
            .Where(r => r.ProductId == productId)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a collection of reviews written by a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve reviews for.</param>
    /// <returns>An IEnumerable of Review objects representing the user's reviews.</returns>
    public async Task<IEnumerable<Review>> GetReviewsByUserAsync(int userId)
    {
        return await Context.Reviews
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }
}