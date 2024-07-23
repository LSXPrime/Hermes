using Hermes.API.Attributes;
using Hermes.Application.DTOs;
using Hermes.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hermes.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBaseEx
{
    [AuthorizeMiddleware(["User", "Seller", "Admin"])]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userDto = await userService.GetUserByIdAsync(CurrentUserId);
        return Ok(userDto);
    }

    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateCurrentUser([FromBody] UserDto userDto)
    {
        if (userDto.Id != CurrentUserId && CurrentUserRole != "Admin")
            return Forbid();
        
        await userService.UpdateUserAsync(CurrentUserId, userDto);
        return NoContent();
    }

    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetUserById(int userId)
    {
        var userDto = await userService.GetUserByIdAsync(userId);
        if (userDto == null)
            return NotFound();

        return Ok(userDto);
    }
    
    [HttpGet]
    [AuthorizeMiddleware(["Admin"])]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await userService.GetAllUsersAsync();
        return Ok(users);
    }
}