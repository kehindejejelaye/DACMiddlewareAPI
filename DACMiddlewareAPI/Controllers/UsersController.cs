using DACMiddlewareAPI.Entities;
using DACMiddlewareAPI.Interfaces;
using DACMiddlewareAPI.Models;
using DACMiddlewareAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DACMiddlewareAPI.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUserService _userService;

    public UsersController(ILogger<UsersController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    private async Task<Client> ValidateClient(string key, string id)
    {
        var client = await _userService.GetClient(id);

        return client;
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> AuthenticateUser([FromBody] UserLoginDto user, [FromHeader] string APP_KEY, [FromHeader] string APP_ID)
    {
        var client = await ValidateClient(APP_KEY, APP_ID);

        if (client == null) return Unauthorized();

        if (client.AppKeyHash != APP_KEY) return Forbid("Invalid Credentials");

        if (user == null)
        {
            return BadRequest("User object cannot be empty");
        }

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        var userServiceResult = await _userService.GetUserByEmail(user);

        if (userServiceResult.StatusCode == 400) return BadRequest(userServiceResult);
        else if (userServiceResult.StatusCode == 404) return NotFound(userServiceResult);
        else if (userServiceResult.StatusCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, userServiceResult);

        return Ok(userServiceResult);
    }

    [Authorize(Policy = "AuthUser")]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserForCreationDto user, [FromHeader] string APP_KEY, [FromHeader] string APP_ID)
    {
        var client = await ValidateClient(APP_KEY, APP_ID);

        if (client == null) return Unauthorized();

        if (client.AppKeyHash != APP_KEY) return Forbid("Invalid Credentials");

        if (user == null)
        {
            return BadRequest("User object cannot be empty");
        }

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        var userServiceResult = await _userService.CreateUser(user);

        if (userServiceResult.StatusCode == 404) return NotFound(userServiceResult);
        else if (userServiceResult.StatusCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, userServiceResult);

        return Ok(userServiceResult);
    }

    [Authorize(Policy = "AuthUser")]
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(int userId, UserForUpdateDto updatedUser, [FromHeader] string APP_KEY, [FromHeader] string APP_ID)
    {
        var client = await ValidateClient(APP_KEY, APP_ID);

        if (client == null) return Unauthorized();

        if (client.AppKeyHash != APP_KEY) return Forbid("Invalid Credentials");

        if (updatedUser == null)
        {
            return BadRequest("User object cannot be empty");
        }

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        var userServiceResult = await _userService.UpdateUser(userId, updatedUser);

        if (userServiceResult.StatusCode == 404) return NotFound(userServiceResult);
        else if (userServiceResult.StatusCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, userServiceResult);

        return Ok(userServiceResult);
    }

    [Authorize(Policy = "AuthUser")]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(int userId, [FromHeader] string APP_KEY, [FromHeader] string APP_ID)
    {
        var client = await ValidateClient(APP_KEY, APP_ID);

        if (client == null) return Unauthorized();

        if (client.AppKeyHash != APP_KEY) return Forbid("Invalid Credentials");

        var userServiceResult = await _userService.GetUser(userId);

        if (userServiceResult.StatusCode == 404) return NotFound(userServiceResult);
        else if (userServiceResult.StatusCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, userServiceResult);

        return Ok(userServiceResult);
    }

    [Authorize(Policy = "AuthUser")]
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(int userId, [FromHeader] string APP_KEY, [FromHeader] string APP_ID)
    {
        var client = await ValidateClient(APP_KEY, APP_ID);

        if (client == null) return Unauthorized();

        if (client.AppKeyHash != APP_KEY) return Forbid("Invalid Credentials");

        var userServiceResult = await _userService.DeleteUser(userId);

        if (userServiceResult.StatusCode == 404) return NotFound(userServiceResult);
        else if (userServiceResult.StatusCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, userServiceResult);

        return Ok(userServiceResult);

    }

    [Authorize(Policy = "AuthUser")]
    [HttpPost("{userId}/assignuser")]
    public async Task<IActionResult> AttachUser(int userId, AttachUserDto obj, [FromHeader] string APP_KEY, [FromHeader] string APP_ID)
    {
        var client = await ValidateClient(APP_KEY, APP_ID);

        if (client == null) return Unauthorized();

        if (client.AppKeyHash != APP_KEY) return Forbid("Invalid Credentials");

        if (obj == null)
        {
            return BadRequest("Object object cannot be empty");
        }

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        var userServiceResult = await _userService.AssignUser(userId, obj);

        if (userServiceResult.StatusCode == 404) return NotFound(userServiceResult);
        else if (userServiceResult.StatusCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, userServiceResult);

        return Ok(userServiceResult);
    }

    [Authorize(Policy = "AuthUser")]
    [HttpGet("{userId}/assignuser")]
    public async Task<IActionResult> GetAssignedUsers(int userId, [FromHeader] string APP_KEY, [FromHeader] string APP_ID)
    {
        var client = await ValidateClient(APP_KEY, APP_ID);

        if (client == null) return Unauthorized();

        if (client.AppKeyHash != APP_KEY) return Forbid("Invalid Credentials");

        var userServiceResult = await _userService.GetAssignedUsers(userId);

        if (userServiceResult.StatusCode == 404) return NotFound(userServiceResult);
        else if (userServiceResult.StatusCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, userServiceResult);

        return Ok(userServiceResult);
    }

    [Authorize(Policy = "AuthUser")]
    [HttpDelete("{userId}/assignuser/{assignUserId}")]
    public async Task<IActionResult> DeleteUser(int userId, int assignUserId, [FromHeader] string APP_KEY, [FromHeader] string APP_ID)
    {
        var client = await ValidateClient(APP_KEY, APP_ID);

        if (client == null) return Unauthorized();

        if (client.AppKeyHash != APP_KEY) return Forbid("Invalid Credentials");
        var userServiceResult = await _userService.DeleteAssignedUser(userId, assignUserId);

        if (userServiceResult.StatusCode == 404) return NotFound(userServiceResult);
        else if (userServiceResult.StatusCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, userServiceResult);

        return Ok(userServiceResult);

    }

    [Authorize(Policy = "AuthUser")]
    [HttpPost("{userId}/bankaccount")]
    public async Task<IActionResult> CreateBankAccount(int userId, [FromHeader] string APP_KEY, [FromHeader] string APP_ID)
    {
        var client = await ValidateClient(APP_KEY, APP_ID);

        if (client == null) return Unauthorized();

        if (client.AppKeyHash != APP_KEY) return Forbid("Invalid Credentials");

        var user = await _userService.GetUser(userId);

        if (user == null)
        {
            return NotFound("User not found!");
        }

        var userServiceResult = await _userService.CreateBankAccount(userId);

        if (userServiceResult.StatusCode == 500) return StatusCode(StatusCodes.Status500InternalServerError, userServiceResult);

        return Ok(userServiceResult);
    }
}