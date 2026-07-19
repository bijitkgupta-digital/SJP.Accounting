namespace SJP.Accounting.Domain.ViewModels;

public class ProjectProfitabilityViewModel
{
    public string ProjectName { get; set; } = string.Empty;

    public decimal Income { get; set; }

    public decimal Expense { get; set; }

    public decimal ProfitLoss { get; set; }
}