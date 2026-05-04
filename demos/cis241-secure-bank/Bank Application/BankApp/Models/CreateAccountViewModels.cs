namespace BankApp.Models;

public class CreateAccountViewModel
{
    public string Username { get; set; } = "";
    public string AccountNumber { get; set; } = "";
    public decimal InitialBalance { get; set; }

    public string? ErrorMessage { get; set; }
}
