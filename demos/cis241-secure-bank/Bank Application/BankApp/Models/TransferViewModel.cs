using Microsoft.AspNetCore.Mvc.Rendering;

namespace BankApp.Models;

public class TransferViewModel
{
    public string FromAccountNumber { get; set; } = "";
    public string ToAccountNumber { get; set; } = "";
    public decimal Amount { get; set; }

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    // Populated from the user's accounts for the dropdowns
    public IEnumerable<SelectListItem> AccountOptions { get; set; }
        = Enumerable.Empty<SelectListItem>();
}

