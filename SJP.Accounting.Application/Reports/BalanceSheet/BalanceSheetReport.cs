using SJP.Accounting.Application.Contracts;
using SJP.Accounting.Application.Reports.BalanceSheet;
using SJP.Accounting.Domain.Contracts;

public sealed class BalanceSheetReport : SJPAccountingReport<BalanceSheetReportDocument>
{
    private readonly IReportRepository _reportRepository;

    public override string ReportCode => "BalanceSheet";

    public override string ReportName => "Balance Sheet";

    public BalanceSheetReport(
        IReportRepository reportRepository,
        IEnumerable<IReportExporter<
            BalanceSheetReportDocument>>
            exporters)
        : base(exporters)
    {
        _reportRepository = reportRepository;
    }

    public override async Task<BalanceSheetReportDocument> GenerateAsync(CancellationToken cancellationToken)
    {
        return new BalanceSheetReportDocument
        {
            ReportCode = ReportCode,
            ReportName = ReportName,
            Dashboard = await _reportRepository.GetDashboardAsync(cancellationToken),
            PartnerEquity = await _reportRepository.GetSettlementStatusAsync(cancellationToken)
        };
    }
}