namespace BankApp.Models;

public class AccountRequestViewModel
{
    public string RequestedAccountType { get; set; } = "";
    public string? Notes { get; set; }

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
}
