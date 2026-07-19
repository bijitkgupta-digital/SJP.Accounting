namespace SJP.Accounting.Domain.ViewModels;

public class PartnerCapitalPositionViewModel
{
    public string Partner { get; set; } = string.Empty;

    public decimal Investment { get; set; }

    public decimal ExpenseFunding { get; set; }

    public decimal AssetFunding { get; set; }

    public decimal Withdrawal { get; set; }

    public decimal SettlementPaid { get; set; }

    public decimal SettlementReceived { get; set; }

    public decimal Contribution { get; set; }
}