using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BankApp.Models;
using BankApp.Services;
using BankApp.Data;

namespace BankApp.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly AccountService _accountService;
    private readonly ApplicationDbContext _context;

    public AccountController(AccountService accountService, ApplicationDbContext context)
    {
        _accountService = accountService;
        _context = context;
    }

    private int GetUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.Parse(id!);
    }

    // ---------- GET /Account ----------
    public IActionResult Index()
    {
        var userId = GetUserId();
        var accounts = _accountService.GetAccountsForUser(userId).ToList();
        return View(accounts);
    }

    // ---------- helper to build dropdown options ----------
    private void PopulateAccountOptions(TransferViewModel model)
    {
        var userId = GetUserId();
        var accounts = _accountService.GetAccountsForUser(userId).ToList();

        model.AccountOptions = accounts.Select(a => new SelectListItem
        {
            Value = a.AccountNumber,
            Text = $"{a.AccountNumber} — {a.Balance:C}"
        }).ToList();
    }

    // ---------- GET /Account/Transfer ----------
    [HttpGet]
    public IActionResult Transfer(string? fromAccount)
    {
        var model = new TransferViewModel();

        PopulateAccountOptions(model);

        // Default the "from" field either to the clicked account or the first one
        if (!string.IsNullOrEmpty(fromAccount))
        {
            model.FromAccountNumber = fromAccount;
        }
        else
        {
            model.FromAccountNumber = model.AccountOptions.FirstOrDefault()?.Value ?? "";
        }

        return View(model);
    }

    // ---------- POST /Account/Transfer ----------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Transfer(TransferViewModel model)
    {
        if (model.Amount <= 0)
        {
            model.ErrorMessage = "Amount must be positive.";
            PopulateAccountOptions(model);
            return View(model);
        }

        if (_accountService.Transfer(
                model.FromAccountNumber,
                model.ToAccountNumber,
                model.Amount,
                out var msg))
        {
            TempData["Success"] = msg;
            return RedirectToAction("Index");
        }

        model.ErrorMessage = msg;
        PopulateAccountOptions(model);
        return View(model);
    }

    // ---------- GET /Account/RequestAccount ----------
    [HttpGet]
    public IActionResult RequestAccount()
    {
        return View(new AccountRequestViewModel());
    }

    // ---------- POST /Account/RequestAccount ----------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RequestAccount(AccountRequestViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.RequestedAccountType))
        {
            model.ErrorMessage = "Please select or enter an account type.";
            return View(model);
        }

        var userId = GetUserId();
        var user = _context.Users.Find(userId);
        if (user is null)
        {
            model.ErrorMessage = "User not found.";
            return View(model);
        }

        var request = new AccountRequest
        {
            UserId = userId,
            RequestedAccountType = model.RequestedAccountType,
            Notes = model.Notes,
            Status = "Pending",
            RequestedAt = DateTime.UtcNow
        };

        _context.AccountRequests.Add(request);
        _context.SaveChanges();

        // Show a message on Accounts page and go back
        TempData["Message"] = "Your request has been submitted to an administrator.";
        return RedirectToAction("Index", "Account");
    }
}



