using BankApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    // NEW
    public DbSet<AdminAuditEntry> AdminAuditEntries => Set<AdminAuditEntry>();
    public DbSet<AccountRequest> AccountRequests => Set<AccountRequest>();
}

