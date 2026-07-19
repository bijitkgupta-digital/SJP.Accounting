using SJP.Accounting.Domain.ViewModels;

namespace SJP.Accounting.Application.Reports.PartnerSettlement;

public sealed class PartnerSettlementReportDocument : ReportDocument
{
    public DashboardViewModel Dashboard { get; set; } = new();
    public List<ProjectProfitabilityViewModel> ProjectProfitability { get; set; } = new();
    public List<PartnerCapitalPositionViewModel> CapitalPositions { get; set; } = new();
    public List<SettlementStatusViewModel> SettlementStatus { get; set; } = new();
    public SettlementRecommendationViewModel? SettlementRecommendation { get; set; }
    public List<AllTransactionViewModel> Transactions { get; set; } = new();
}