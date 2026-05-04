using BankApp.Data;
using BankApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Services;

public class AdminService
{
    private readonly ApplicationDbContext _context;

    public AdminService(ApplicationDbContext context)
    {
        _context = context;
    }

    public void LogAdminAction(string adminUsername, string action)
    {
        var entry = new AdminAuditEntry
        {
            AdminUsername = adminUsername,
            Action = action,
            Timestamp = DateTime.UtcNow
        };

        _context.AdminAuditEntries.Add(entry);
        _context.SaveChanges();
    }

    public IReadOnlyList<string> GetRecentAuditLog(int count = 20)
    {
        return _context.AdminAuditEntries
            .OrderByDescending(e => e.Timestamp)
            .Take(count)
            .Select(e => $"[{e.Timestamp:u}] {e.AdminUsername}: {e.Action}")
            .ToList();
    }

    // HELPER: truly lock a user
    public bool LockUser(int userId, string adminName, out string message)
    {
        var user = _context.Users.SingleOrDefault(u => u.Id == userId);
        if (user is null)
        {
            message = "User not found.";
            return false;
        }

        if (user.IsLocked)
        {
            message = "User is already locked.";
            return false;
        }

        user.IsLocked = true;
        _context.SaveChanges();

        LogAdminAction(adminName, $"Locked user {user.Username} (Id={user.Id})");
        message = $"User {user.Username} has been locked.";
        return true;
    }

    // Helper to surface pending account requests for Admin page
    public IReadOnlyList<AccountRequest> GetPendingAccountRequests()
    {
        return _context.AccountRequests
            .Include(r => r.User)
            .Where(r => r.Status == "Pending")
            .OrderBy(r => r.RequestedAt)
            .ToList();
    }

    public IReadOnlyList<User> GetAllUsers()
{
    return _context.Users
        .OrderBy(u => u.Username)
        .ToList();
}

public bool UnlockUser(int userId, string adminName, out string message)
{
    var user = _context.Users.SingleOrDefault(u => u.Id == userId);
    if (user is null)
    {
        message = "User not found.";
        return false;
    }

    if (!user.IsLocked)
    {
        message = "User is not locked.";
        return false;
    }

    user.IsLocked = false;
    _context.SaveChanges();

    LogAdminAction(adminName, $"Unlocked user {user.Username} (Id={user.Id})");
    message = $"User {user.Username} has been unlocked.";
    return true;
}

}
