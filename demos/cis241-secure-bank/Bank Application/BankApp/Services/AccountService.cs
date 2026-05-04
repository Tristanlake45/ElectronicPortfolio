using BankApp.Data;
using BankApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Services;

public class AccountService
{
    private readonly ApplicationDbContext _context;

    public AccountService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Account> GetAccountsForUser(int userId)
    {
        return _context.Accounts
            .Where(a => a.OwnerUserId == userId)
            .AsNoTracking()
            .ToList();
    }

    public Account? GetAccountByNumber(string accountNumber)
    {
        return _context.Accounts
            .SingleOrDefault(a => a.AccountNumber == accountNumber);
    }

    public IEnumerable<Transaction> GetTransactionsForAccount(int accountId)
    {
        return _context.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.Timestamp)
            .AsNoTracking()
            .ToList();
    }

    public bool Transfer(string fromAcc, string toAcc, decimal amount, out string message)
    {
        if (amount <= 0)
        {
            message = "Amount must be positive.";
            return false;
        }

        if (fromAcc == toAcc)
        {
            message = "Cannot transfer to the same account.";
            return false;
        }

        var from = _context.Accounts.SingleOrDefault(a => a.AccountNumber == fromAcc);
        var to   = _context.Accounts.SingleOrDefault(a => a.AccountNumber == toAcc);

        if (from is null || to is null)
        {
            message = "Invalid account.";
            return false;
        }

        if (from.Balance < amount)
        {
            message = "Insufficient funds.";
            return false;
        }

        using var tx = _context.Database.BeginTransaction();

        from.Balance -= amount;
        to.Balance   += amount;

        var now = DateTime.UtcNow;

        _context.Transactions.Add(new Transaction
        {
            AccountId = from.Id,
            Timestamp = now,
            Description = $"Transfer to {to.AccountNumber}",
            Amount = -amount
        });

        _context.Transactions.Add(new Transaction
        {
            AccountId = to.Id,
            Timestamp = now,
            Description = $"Transfer from {from.AccountNumber}",
            Amount = amount
        });

        _context.SaveChanges();
        tx.Commit();

        message = "Transfer completed.";
        return true;
    }

    // ----- Account request helpers -----

    public AccountRequest? GetAccountRequestById(int id)
    {
        return _context.AccountRequests
            .Include(r => r.User)
            .SingleOrDefault(r => r.Id == id);
    }

    public (bool Success, string Message) CreateAccountForRequest(AccountRequest request, string accountNumber, decimal initialBalance)
    {
        if (request.Status != "Pending")
        {
            return (false, "Request is not pending.");
        }

        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            return (false, "Account number is required.");
        }

        if (initialBalance < 0)
        {
            return (false, "Initial balance cannot be negative.");
        }

        var exists = _context.Accounts.Any(a => a.AccountNumber == accountNumber);
        if (exists)
        {
            return (false, "An account with that number already exists.");
        }

        var account = new Account
        {
            OwnerUserId = request.UserId,
            AccountNumber = accountNumber,
            Balance = initialBalance
        };

        _context.Accounts.Add(account);

        request.Status = "Approved";
        request.ProcessedAt = DateTime.UtcNow;

        _context.SaveChanges();

        return (true, $"Account {accountNumber} created for {request.User.Username}.");
    }
}


