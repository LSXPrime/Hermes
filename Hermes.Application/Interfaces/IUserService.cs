using Hermes.Application.DTOs;

namespace Hermes.Application.Interfaces;

/// <summary>
/// Defines the interface for a service that manages User operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves a specific user by their ID.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve.</param>
    /// <returns>A UserDto object representing the specified user, or null if no user with the given ID is found.</returns>
    Task<UserDto?> GetUserByIdAsync(int userId);

    /// <summary>
    /// Retrieves a collection of all users.
    /// </summary>
    /// <returns>An IEnumerable of UserDto objects representing all users.</returns>
    Task<IEnumerable<UserDto>> GetAllUsersAsync();

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="userId">The ID of the user to update.</param>
    /// <param name="userDto">The UserDto object containing the updated user data.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task UpdateUserAsync(int userId, UserDto userDto);
}