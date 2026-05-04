using System.Collections.Generic;

namespace BankApp.Models;

public class AdminDashboardViewModel
{
    public IReadOnlyList<string> AuditLines { get; set; } = new List<string>();
    public IReadOnlyList<AccountRequest> PendingRequests { get; set; } = new List<AccountRequest>();
}
