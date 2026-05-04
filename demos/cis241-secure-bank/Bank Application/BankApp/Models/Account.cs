namespace BankApp.Models;

public class Account
{
    public int Id { get; set; }
    public int OwnerUserId { get; set; }
    public string AccountNumber { get; set; } = "";
    public decimal Balance { get; set; }
}
