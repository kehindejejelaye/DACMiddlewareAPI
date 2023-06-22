using DACMiddlewareAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace DACMiddlewareAPI.Context;

public class MiddlewareContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<BankAccount> BankAccounts { get; set; }

    public DbSet<AssignedUser> AssignedUsers { get; set; }

    public MiddlewareContext(DbContextOptions<MiddlewareContext> options) : base(options)
    { }
}
