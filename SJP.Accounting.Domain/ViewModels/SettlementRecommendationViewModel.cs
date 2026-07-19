namespace SJP.Accounting.Domain.ViewModels;

public class SettlementRecommendationViewModel
{
    public decimal SettlementAmount { get; set; }

    public string PayingPartner { get; set; } = string.Empty;

    public string ReceivingPartner { get; set; } = string.Empty;
}