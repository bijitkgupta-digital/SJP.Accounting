namespace SJP.Accounting.Domain.ViewModels;

public class DashboardViewModel
{
    public decimal TotalContribution { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal TotalAssetPurchase { get; set; }
    public decimal NetProfitLoss { get; set; }
    public decimal FundBalance { get; set; }
}