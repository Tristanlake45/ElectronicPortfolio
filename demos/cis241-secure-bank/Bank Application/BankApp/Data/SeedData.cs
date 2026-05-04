using BankApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Data;

public static class SeedData
{
    public static void Initialize(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        // ✅ Make sure DB and tables exist (NO migrations)
        context.Database.EnsureCreated();

        // If users already exist, assume DB is seeded
        if (context.Users.Any())
        {
            return;
        }

        // --- Seed demo users with hashed passwords ---
        var user = new User { Username = "user", Role = "Customer" };
        user.Password = passwordHasher.HashPassword(user, "Password1!");

        var admin = new User { Username = "admin", Role = "Admin" };
        admin.Password = passwordHasher.HashPassword(admin, "AdminPass1!");

        context.Users.AddRange(user, admin);
        context.SaveChanges();

        // --- Seed accounts for those users ---
        var accounts = new[]
        {
            new Account { OwnerUserId = user.Id, AccountNumber = "CHK-1001", Balance = 1500.50m },
            new Account { OwnerUserId = user.Id, AccountNumber = "SAV-2001", Balance = 3200.00m },
            new Account { OwnerUserId = admin.Id, AccountNumber = "ADM-9999", Balance = 99999.99m }
        };

        context.Accounts.AddRange(accounts);
        context.SaveChanges();
    }
}


