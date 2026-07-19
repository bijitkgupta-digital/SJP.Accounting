using SJP.Accounting.Application.Contracts;
using SJP.Accounting.Domain.Contracts;

namespace SJP.Accounting.Application.Reports.PartnerSettlement;

public sealed class PartnerSettlementReport : SJPAccountingReport<PartnerSettlementReportDocument>
{
    private readonly IReportRepository _reportRepository;
    public override string ReportCode => "PartnerSettlement";
    public override string ReportName => "Partner Account Settlement Report";
    public PartnerSettlementReport(IReportRepository reportRepository
        , IEnumerable<IReportExporter<PartnerSettlementReportDocument>> reportExporters) : base(reportExporters)
    {
        _reportRepository = reportRepository;
    }

    public override async Task<PartnerSettlementReportDocument> GenerateAsync(CancellationToken cancellationToken)
    {
        var document =
            new PartnerSettlementReportDocument
            {
                ReportCode = ReportCode,
                ReportName = ReportName
            };
        document.Dashboard = await _reportRepository.GetDashboardAsync(cancellationToken) ?? new();
        document.ProjectProfitability = await _reportRepository.GetProjectProfitabilityAsync(cancellationToken);
        document.CapitalPositions = await _reportRepository.GetPartnerCapitalPositionAsync(cancellationToken);
        document.SettlementStatus = await _reportRepository.GetSettlementStatusAsync(cancellationToken);
        document.SettlementRecommendation = await _reportRepository.GetSettlementRecommendationAsync(cancellationToken);
        document.Transactions = await _reportRepository.GetAllTransactionsAsync(cancellationToken);

        return document;
    }
}