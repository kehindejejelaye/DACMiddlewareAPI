using AutoMapper;
using Azure;
using DACMiddlewareAPI.Context;
using DACMiddlewareAPI.Entities;
using DACMiddlewareAPI.Interfaces;
using DACMiddlewareAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DACMiddlewareAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    private readonly ILogger<UserService> _logger;
    private readonly IMapper _mapper;
    public UserService(ILogger<UserService> logger, IUserRepository userRepository, IMapper mapper)
    {
        _logger = logger;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ResponseDto<bool>> CreateUser(UserForCreationDto user)
    {
        var responseDto = new ResponseDto<bool>();

        try
        {
            var _user = _mapper.Map<User>(user);
            await _userRepository.CreateUserAsync(_user);
            var response = await _userRepository.SaveChangesAsync();

            return new ResponseDto<bool>
            {
                DisplayMessage = "Successfully created user",
                StatusCode = StatusCodes.Status200OK,
                Result = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("Unable to create user {user}", user);
            return new ResponseDto<bool>
            {
                DisplayMessage = "Error",
                StatusCode = StatusCodes.Status500InternalServerError,
                ErrorMessages = new List<string>() { "Unable to create user" }
            };
        }
    }

    public async Task<ResponseDto<bool>> UpdateUser(int userId, UserForUpdateDto updatedUser)
    {
        var responseDto = new ResponseDto<bool>();

        try
        {
            var existingUser = await _userRepository.GetUser(userId);
            if (existingUser == null)
            {
                responseDto.DisplayMessage = "User not found";
                responseDto.StatusCode = StatusCodes.Status404NotFound;
                return responseDto;
            }
            updatedUser.Id = userId;
            // Update the user properties
            var _user = _mapper.Map(updatedUser, existingUser);

            var response = await _userRepository.SaveChangesAsync();

            return new ResponseDto<bool>
            {
                DisplayMessage = "Successfully updated user",
                StatusCode = StatusCodes.Status200OK,
                Result = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to update user with ID: {userId}", userId);
            return new ResponseDto<bool>
            {
                DisplayMessage = "Error",
                StatusCode = StatusCodes.Status500InternalServerError,
                ErrorMessages = new List<string>() { "Unable to update user" }
            };
        }
    }

    public async Task<Client> GetClient(string id) => await _userRepository.GetClient(id);

    public async Task<ResponseDto<UserDto>> GetUser(int userId)
    {
        var responseDto = new ResponseDto<UserDto>();

        try
        {
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                responseDto.DisplayMessage = "User not found";
                responseDto.StatusCode = StatusCodes.Status404NotFound;
                return responseDto;
            }

            var userDto = _mapper.Map<UserDto>(user);

            return new ResponseDto<UserDto>
            {
                DisplayMessage = "Successfully retrieved user",
                StatusCode = StatusCodes.Status200OK,
                Result = userDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to retrieve user with ID: {userId}", userId);
            return new ResponseDto<UserDto>
            {
                DisplayMessage = "Error",
                StatusCode = StatusCodes.Status500InternalServerError,
                ErrorMessages = new List<string>() { "Unable to retrieve user" }
            };
        }
    }

    public async Task<ResponseDto<UserDto>> GetUserByEmail(UserLoginDto userL)
    {
        var responseDto = new ResponseDto<UserDto>();

        try
        {
            var user = await _userRepository.GetUserByEmail(userL.Email);

            if (user == null)
            {
                responseDto.DisplayMessage = "User not found";
                responseDto.StatusCode = StatusCodes.Status404NotFound;
                return responseDto;
            }

            if (user.Password != userL.Password)
            {
                responseDto.DisplayMessage = "Invalid Credentials";
                responseDto.StatusCode = StatusCodes.Status400BadRequest;
                return responseDto;
            }
            var userDto = _mapper.Map<UserDto>(user);

            return new ResponseDto<UserDto>
            {
                DisplayMessage = "Successfully retrieved user",
                StatusCode = StatusCodes.Status200OK,
                Result = userDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to retrieve user with email: {email}", userL.Email);
            return new ResponseDto<UserDto>
            {
                DisplayMessage = "Error",
                StatusCode = StatusCodes.Status500InternalServerError,
                ErrorMessages = new List<string>() { "Unable to retrieve user" }
            };
        }
    }

    public async Task<ResponseDto<bool>> DeleteUser(int userId)
    {
        var responseDto = new ResponseDto<bool>();

        try
        {
            var existingUser = await _userRepository.GetUser(userId);
            if (existingUser == null)
            {
                responseDto.DisplayMessage = "User not found";
                responseDto.StatusCode = StatusCodes.Status404NotFound;
                return responseDto;
            }

            // Soft delete the user
            await _userRepository.DeleteUser(existingUser);

            var response = await _userRepository.SaveChangesAsync();

            return new ResponseDto<bool>
            {
                DisplayMessage = "Successfully deleted user",
                StatusCode = StatusCodes.Status200OK,
                Result = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to delete user with ID: {userId}", userId);
            return new ResponseDto<bool>
            {
                DisplayMessage = "Error",
                StatusCode = StatusCodes.Status500InternalServerError,
                ErrorMessages = new List<string>() { "Unable to delete user" }
            };
        }
    }

    
    public async Task<ResponseDto<bool>> AssignUser(int userId, AttachUserDto obj)
    {
        var responseDto = new ResponseDto<bool>();

        try
        {
            var owner = await _userRepository.GetUser(userId);
            var attachedUser = await _userRepository.GetUser(obj.UserAssigned);

            if (owner == null || attachedUser == null)
            {
                responseDto.DisplayMessage = (owner == null) ? "User does not exist" : "The user you want to attach does not exist";
                responseDto.StatusCode = StatusCodes.Status404NotFound;
                return responseDto;
            }

            obj.OwnerId = owner.Id;
            var attachUserObj = _mapper.Map<AssignedUser>(obj);
            await _userRepository.AttachUsers(attachUserObj);
            var response = await _userRepository.SaveChangesAsync();

            return new ResponseDto<bool>
            {
                DisplayMessage = "Successfully attached user",
                StatusCode = StatusCodes.Status200OK,
                Result = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("Unable to attach user with {attachId} to owner {ownerId}", obj.UserAssigned, obj.OwnerId);

            return new ResponseDto<bool>
            {
                DisplayMessage = "Error",
                StatusCode = StatusCodes.Status500InternalServerError,
                ErrorMessages = new List<string>() { "Unable to attach user" }
            };
        }
    }

    public async Task<ResponseDto<List<UserDto>>> GetAssignedUsers(int userId)
    {
        var responseDto = new ResponseDto<List<UserDto>>();

        try
        {
            var user = await _userRepository.GetUser(userId);

            if (user == null)
            {
                responseDto.DisplayMessage = "User not found";
                responseDto.StatusCode = StatusCodes.Status404NotFound;
                return responseDto;
            }

            var assignedUsers = await _userRepository.GetAttachedUsers(userId);
            var userDto = _mapper.Map<List<UserDto>>(assignedUsers);

            return new ResponseDto<List<UserDto>>
            {
                DisplayMessage = "Successfully retrieved assigned users",
                StatusCode = StatusCodes.Status200OK,
                Result = userDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to retrieve users assigned to user with ID: {userId}", userId);

            return new ResponseDto<List<UserDto>>
            {
                DisplayMessage = "Error",
                StatusCode = StatusCodes.Status500InternalServerError,
                ErrorMessages = new List<string>() { "Unable to retrieve assigned users" }
            };
        }
    }

    public async Task<ResponseDto<bool>> DeleteAssignedUser(int userId, int assignedUser)
    {
        var responseDto = new ResponseDto<bool>();

        try
        {
            var _assignedUser = await _userRepository.GetAttachedUserId(userId, assignedUser);
            if (_assignedUser == null)
            {
                responseDto.DisplayMessage = "Entry not found";
                responseDto.StatusCode = StatusCodes.Status404NotFound;
                return responseDto;
            }

            // Soft delete the user
            await _userRepository.DeleteAttachedUserId(_assignedUser);

            var response = await _userRepository.SaveChangesAsync();

            return new ResponseDto<bool>
            {
                DisplayMessage = "Successfully deleted assigned user",
                StatusCode = StatusCodes.Status200OK,
                Result = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to delete user with ID: {userId}", userId);

            return new ResponseDto<bool>
            {
                DisplayMessage = "Error",
                StatusCode = StatusCodes.Status500InternalServerError,
                ErrorMessages = new List<string>() { "Unable to delete user" }
            };
        }
    }

    public async Task<ResponseDto<bool>> CreateBankAccount(int userId)
    {
        var responseDto = new ResponseDto<bool>();

        try
        {
            var bankAccount = new BankAccount
            {
                UserId = userId
            };
            await _userRepository.CreateBankAccountAsync(bankAccount);

            var response = await _userRepository.SaveChangesAsync();

            return new ResponseDto<bool>
            {
                DisplayMessage = "Successfully created bank account",
                StatusCode = StatusCodes.Status200OK,
                Result = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("Unable to create bank account for user with id: {userId}", userId);
            return new ResponseDto<bool>
            {
                DisplayMessage = "Error",
                StatusCode = StatusCodes.Status500InternalServerError,
                ErrorMessages = new List<string>() { "Unable to create bank account" }
            };
        }
    }

}
