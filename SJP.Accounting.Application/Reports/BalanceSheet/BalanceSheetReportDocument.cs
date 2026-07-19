using SJP.Accounting.Domain.ViewModels;

namespace SJP.Accounting.Application.Reports.BalanceSheet
{
    public sealed class BalanceSheetReportDocument : ReportDocument
    {
        public DashboardViewModel Dashboard { get; set; } = new();

        public List<SettlementStatusViewModel> PartnerEquity { get; set; } = [];

    }
}