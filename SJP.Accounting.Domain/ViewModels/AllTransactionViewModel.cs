namespace SJP.Accounting.Domain.ViewModels;

public class AllTransactionViewModel
{
    public Guid TransactionId { get; set; }

    public DateTime TransactionDate { get; set; }

    public string? ProjectName { get; set; }

    public string? CategoryName { get; set; }

    public string TransactionTypeName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string? PaidBy { get; set; }

    public string? ReceivedBy { get; set; }

    public string? Narration { get; set; }

    public string? GoogleDriveLink { get; set; }

    public DateTime ImportedOn { get; set; }
}