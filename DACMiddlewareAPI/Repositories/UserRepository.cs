using DACMiddlewareAPI.Context;
using DACMiddlewareAPI.Entities;
using DACMiddlewareAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DACMiddlewareAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MiddlewareContext _middlewareContext;

        public UserRepository(MiddlewareContext middlewareContext)
        {
            _middlewareContext = middlewareContext;
        }


        // CreateUser
        public async Task CreateUserAsync(User user) => await _middlewareContext.Users.AddAsync(user);

        // UpdateUser
        // public async Task UpdateUser(User user) => await _middlewareContext.SaveChangesAsync();

        // GetUser
        public async Task<User> GetUser(int id) => await _middlewareContext.Users.Where(user => user.Id == id).FirstOrDefaultAsync();  
        public async Task<User> GetUserByEmail(string email) => await _middlewareContext.Users.Where(user => user.Email == email).FirstOrDefaultAsync();

        // GetClient
        public async Task<Client> GetClient(string appId) => await _middlewareContext.Clients.FirstAsync(client => client.AppId == appId);

        // DeleteUser
        public async Task DeleteUser(User user) => _middlewareContext.Users.Remove(user);

        // attachUser
        public async Task AttachUsers(AssignedUser obj) => await _middlewareContext.AssignedUsers.AddAsync(obj);

        // displayAssignedUsers
        public async Task<List<User>> GetAttachedUsers(int ownerId)
        {
            var assignedUserIds = await _middlewareContext.AssignedUsers
                .Where(au => au.OwnerId == ownerId)
                .Select(au => au.UserAssigned)
                .ToListAsync();

            var assignedUsers = await _middlewareContext.Users
                .Where(u => assignedUserIds.Contains(u.Id))
                .ToListAsync();

            return assignedUsers;
        }

        // GetAssignedUser
        public async Task<AssignedUser> GetAttachedUserId(int ownerId, int userAssigned)
        {
            var assignedUser = await _middlewareContext.AssignedUsers
            .FirstOrDefaultAsync(au => au.OwnerId == ownerId && au.UserAssigned == userAssigned);

            return assignedUser;
        }

        // detachAssignedUser
        public async Task DeleteAttachedUserId(AssignedUser assignedUser) => _middlewareContext.AssignedUsers.Remove(assignedUser);

        // createBankAccount
        public async Task CreateBankAccountAsync(BankAccount account) => await _middlewareContext.BankAccounts.AddAsync(account);

        // SaveChanges
        public async Task<bool> SaveChangesAsync() => (await _middlewareContext.SaveChangesAsync() >= 1);
    }
}
