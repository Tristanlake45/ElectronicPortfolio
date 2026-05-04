namespace BankApp.Models;

public class AccountRequest
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string RequestedAccountType { get; set; } = "";
    public string? Notes { get; set; }

    // "Pending", "Approved", "Rejected"
    public string Status { get; set; } = "Pending";

    public DateTime RequestedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}
