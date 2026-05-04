using BankApp.Data;
using BankApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Services;

public class AuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public User? ValidateUser(string username, string password)
    {
        var user = _context.Users
            .SingleOrDefault(u => u.Username == username);

        if (user is null)
            return null;

        // NEW: locked user cannot log in
        if (user.IsLocked)
            return null;

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
        return result == PasswordVerificationResult.Success ? user : null;
    }

    public User? GetUserById(int id) =>
        _context.Users.Find(id);
}
