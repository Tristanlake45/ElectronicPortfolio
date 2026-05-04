using BankApp.Data;
using BankApp.Models;
using BankApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankApp.Controllers;

public class AuthController : Controller
{
    private readonly AuthService _authService;
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthController(
        AuthService authService,
        ApplicationDbContext context,
        IPasswordHasher<User> passwordHasher)
    {
        _authService = authService;
        _context = context;
        _passwordHasher = passwordHasher;
    }

    // ---------- GET /Auth/Login ----------
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        var model = new LoginViewModel
        {
            ReturnUrl = returnUrl
        };

        // Optional: surface registration success message
        if (TempData["Message"] is string msg)
        {
            model.InfoMessage = msg;
        }

        return View(model);
    }

    // ---------- POST /Auth/Login ----------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Username) ||
            string.IsNullOrWhiteSpace(model.Password))
        {
            model.ErrorMessage = "Username and password are required.";
            return View(model);
        }

        var user = _authService.ValidateUser(model.Username, model.Password);
        if (user is null)
        {
            // Could be bad password OR locked user
            model.ErrorMessage = "Invalid credentials or account locked.";
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        if (!string.IsNullOrEmpty(model.ReturnUrl) &&
            Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    // ---------- POST /Auth/Logout ----------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    // ---------- GET /Auth/Register ----------
    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    // ---------- POST /Auth/Register ----------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel model)
    {
        if (model.Password != model.ConfirmPassword)
        {
            model.ErrorMessage = "Passwords do not match.";
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.Username) ||
            string.IsNullOrWhiteSpace(model.Password))
        {
            model.ErrorMessage = "Username and password are required.";
            return View(model);
        }

        var existing = _context.Users.SingleOrDefault(u => u.Username == model.Username);
        if (existing != null)
        {
            model.ErrorMessage = "That username is already taken.";
            return View(model);
        }

        var user = new User
        {
            Username = model.Username,
            Role = "Customer",
            IsLocked = false
        };

        user.Password = _passwordHasher.HashPassword(user, model.Password);

        _context.Users.Add(user);
        _context.SaveChanges();

        // Tell the login page to show a success message
        TempData["Message"] = "Registration successful. Please log in.";
        return RedirectToAction("Login");
    }
}

