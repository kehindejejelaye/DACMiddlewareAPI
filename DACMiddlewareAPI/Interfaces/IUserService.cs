using DACMiddlewareAPI.Entities;
using DACMiddlewareAPI.Models;

namespace DACMiddlewareAPI.Interfaces;

public interface IUserService
{
    Task<ResponseDto<bool>> CreateUser(UserForCreationDto user);
    Task<ResponseDto<bool>> UpdateUser(int userId, UserForUpdateDto updatedUser);
    Task<ResponseDto<UserDto>> GetUser(int userId);
    Task<ResponseDto<UserDto>> GetUserByEmail(UserLoginDto userL);
    Task<Client> GetClient(string id);
    Task<ResponseDto<bool>> DeleteUser(int userId);
    Task<ResponseDto<bool>> AssignUser(int userId, AttachUserDto obj);
    Task<ResponseDto<List<UserDto>>> GetAssignedUsers(int userId);
    Task<ResponseDto<bool>> DeleteAssignedUser(int userId, int assignedUser);
    Task<ResponseDto<bool>> CreateBankAccount(int userId);
}
