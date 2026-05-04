using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BankApp.Models;
using BankApp.Services;

namespace BankApp.Controllers;

[Authorize(Policy = "AdminOnly")]
public class AdminController : Controller
{
    private readonly AdminService _adminService;
    private readonly AccountService _accountService;

    public AdminController(AdminService adminService, AccountService accountService)
    {
        _adminService = adminService;
        _accountService = accountService;
    }

    // -------- Admin Dashboard (logs + pending requests) --------
    public IActionResult Index()
    {
        var log = _adminService.GetRecentAuditLog();
        var pendingRequests = _adminService.GetPendingAccountRequests();

        var vm = new AdminDashboardViewModel
        {
            AuditLines = log,
            PendingRequests = pendingRequests
        };

        return View(vm);
    }


    // -------- Approve account request --------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ApproveRequest(int requestId, string accountNumber, decimal initialBalance)
    {
        var adminName = User.Identity?.Name ?? "unknown-admin";

        var request = _accountService.GetAccountRequestById(requestId);
        if (request is null || request.Status != "Pending")
        {
            TempData["Message"] = "Request not found or already processed.";
            return RedirectToAction("Index");
        }

        var (success, message) = _accountService.CreateAccountForRequest(
            request,
            accountNumber,
            initialBalance);

        if (success)
        {
            // Log the admin action here (not inside AccountService)
            _adminService.LogAdminAction(
                adminName,
                $"Approved account request {request.Id} and created account {accountNumber} for {request.User.Username}");
        }

        TempData["Message"] = message;
        return RedirectToAction("Index");
    }

    [HttpGet]
public IActionResult ManageUsers()
{
    var users = _adminService.GetAllUsers();
    return View(users);
}

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult LockUser(int userId)
{
    var adminName = User.Identity?.Name ?? "unknown-admin";
    _adminService.LockUser(userId, adminName, out var message);
    TempData["Message"] = message;
    return RedirectToAction("ManageUsers");
}

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult UnlockUser(int userId)
{
    var adminName = User.Identity?.Name ?? "unknown-admin";
    _adminService.UnlockUser(userId, adminName, out var message);
    TempData["Message"] = message;
    return RedirectToAction("ManageUsers");
}

}



