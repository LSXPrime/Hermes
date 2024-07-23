using Hermes.Domain.Entities;
using Hermes.Domain.Interfaces;
using Hermes.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Hermes.Infrastructure.Repositories;

public class UserRepository(HermesDbContext context) : GenericRepository<User>(context), IUserRepository
{
    /// <summary>
    /// Retrieves a User entity based on the provided ID, including related entities.
    /// </summary>
    /// <param name="id">The ID of the User entity to retrieve.</param>
    /// <returns>
    /// The User entity matching the provided ID, including their posts, followers, and following, or null if no user is found.
    /// </returns>
    public new async Task<User?> GetByIdAsync(int id)
    {
        return await Context.Users
            .Include(p => p.Address)
            .Include(p => p.Reviews)
            .Include(p => p.Cart)
            .Include(p => p.Orders)
            .Include(p => p.Products)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    
    /// <summary>
    /// Retrieves a User entity based on the provided username, including related entities.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <returns>
    /// The User entity matching the provided username, including their posts, followers, and following, or null if no user is found.
    /// </returns>
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await Context.Users
            .Include(p => p.Address)
            .Include(p => p.Reviews)
            .Include(p => p.Cart)
            .Include(p => p.Orders)
            .Include(p => p.Products)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    /// <summary>
    /// Retrieves a User entity based on the provided email, including related entities.
    /// </summary>
    /// <param name="email">The email to search for.</param>
    /// <returns>
    /// The User entity matching the provided email, including their posts, followers, and following, or null if no user is found.
    /// </returns>
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await Context.Users
            .Include(p => p.Address)
            .Include(p => p.Reviews)
            .Include(p => p.Cart)
            .Include(p => p.Orders)
            .Include(p => p.Products)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>
    /// Checks if a user with the provided username exists in the database.
    /// </summary>
    /// <param name="username">The username to check for.</param>
    /// <returns>
    /// True if a user with the provided username exists, false otherwise.
    /// </returns>
    public async Task<bool> UserExistsAsync(string username)
    {
        return await Context.Users.AnyAsync(u => u.Username == username);
    }
    
    /// <summary>
    /// Checks if a user with the provided ID exists in the database.
    /// </summary>
    /// <param name="id">The ID of the user to check for.</param>
    /// <returns>
    /// True if a user with the provided ID exists, false otherwise.
    /// </returns>
    public async Task<bool> UserExistsAsync(int id)
    {
        return await Context.Users.AnyAsync(u => u.Id == id);
    }
}