public class SettlementStatusViewModel
{
    public string Partner { get; set; } = string.Empty;

    public decimal OwnershipPercentage { get; set; }

    public decimal TotalContributionPool { get; set; }

    public decimal ActualContribution { get; set; }

    public decimal ContributionPercentage { get; set; }

    public decimal ExpectedContribution { get; set; }

    public decimal ContributionVariance { get; set; }

    public decimal TotalProfitLoss { get; set; }

    public decimal ExpectedProfitShare { get; set; }

    public decimal CapitalPosition { get; set; }

    public string FundingStatus { get; set; } = string.Empty;

    public string SettlementDirection { get; set; } = string.Empty;

    public decimal SettlementAmount { get; set; }
}