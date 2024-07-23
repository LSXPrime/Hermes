using AutoMapper;
using Hermes.Application.DTOs;
using Hermes.Application.Exceptions;
using Hermes.Application.Interfaces;
using Hermes.Domain.Interfaces;

namespace Hermes.Application.Services;

public class UserService(IUnitOfWork unitOfWork, IMapper mapper) : IUserService
{
    /// <summary>
    /// Retrieves a specific user by their ID.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve.</param>
    /// <returns>A UserDto object representing the specified user, or null if no user with the given ID is found.</returns>
    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found.");

        return mapper.Map<UserDto>(user);
    }

    /// <summary>
    /// Retrieves a collection of all users.
    /// </summary>
    /// <returns>An IEnumerable of UserDto objects representing all users.</returns>
    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await unitOfWork.Users.ExecuteQueryAsync(unitOfWork.Users.GetAllAsync());
        return mapper.Map<IEnumerable<UserDto>>(users);
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="userId">The ID of the user to update.</param>
    /// <param name="userDto">The UserDto object containing the updated user data.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task UpdateUserAsync(int userId, UserDto userDto)
    {
        var user = await unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found.");
        
        mapper.Map(userDto, user);
        await unitOfWork.Users.UpdateAsync(user);
    }
}