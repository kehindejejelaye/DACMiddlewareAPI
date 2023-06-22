using DACMiddlewareAPI.Entities;

namespace DACMiddlewareAPI.Interfaces;

public interface IUserRepository
{
    Task CreateUserAsync(User user);
    Task AttachUsers(AssignedUser obj);
    Task<User> GetUser(int id);
    Task<User> GetUserByEmail(string email);
    Task<Client> GetClient(string appId);
    Task<bool> SaveChangesAsync();
    Task DeleteUser(User user);
    Task<List<User>> GetAttachedUsers(int ownerId);
    Task<AssignedUser> GetAttachedUserId(int ownerId, int userAssigned);
    Task CreateBankAccountAsync(BankAccount account);
    Task DeleteAttachedUserId(AssignedUser assignedUser);
    //Task UpdateUser(User user);
}
