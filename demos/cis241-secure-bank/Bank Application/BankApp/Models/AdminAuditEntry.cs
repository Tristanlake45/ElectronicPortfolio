namespace BankApp.Models;

public class AdminAuditEntry
{
    public int Id { get; set; }

    public DateTime Timestamp { get; set; }

    public string AdminUsername { get; set; } = "";

    public string Action { get; set; } = "";
}
