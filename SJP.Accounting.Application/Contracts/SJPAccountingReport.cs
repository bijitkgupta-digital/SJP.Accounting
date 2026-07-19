using SJP.Accounting.Application.Reports;

namespace SJP.Accounting.Application.Contracts;

public abstract class SJPAccountingReport<TDocument> : ISJPAccountingReport where TDocument : ReportDocument
{
    protected readonly IEnumerable<IReportExporter<TDocument>> _reportExporters;

    public abstract string ReportCode { get; }

    public abstract string ReportName { get; }

    protected SJPAccountingReport(IEnumerable<IReportExporter<TDocument>> reportExporters)
    {
        _reportExporters = reportExporters;
    }
    public abstract Task<TDocument> GenerateAsync(CancellationToken cancellationToken);
    public async Task<IReadOnlyDictionary<string, Stream>> ExportAsync(CancellationToken cancellationToken)
    {
        if (!_reportExporters.Any())
        {
            throw new InvalidOperationException($"No exporters registered for report '{ReportCode}'.");
        }

        var document = await GenerateAsync(cancellationToken);
        var result = new Dictionary<string, Stream>(StringComparer.OrdinalIgnoreCase);
        foreach (var exporter in _reportExporters)
        {
            result[exporter.ExportType] = await exporter.ExportAsync(document, cancellationToken);
        }

        return result;
    }
}