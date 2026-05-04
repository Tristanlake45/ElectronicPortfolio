namespace BankApp.Models;

public class Transaction
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Description { get; set; } = "";
    public decimal Amount { get; set; } // +credit / -debit
}
