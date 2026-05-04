namespace BankApp.Models;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = "";

    // Hashed password
    public string Password { get; set; } = "";

    public string Role { get; set; } = "Customer";

    // used to prevent login when locked
    public bool IsLocked { get; set; } = false;
}

