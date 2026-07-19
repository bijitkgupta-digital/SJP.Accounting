using SJP.Accounting.Application.Reports;

namespace SJP.Accounting.Application.Contracts;

public interface IReportExporter<TDocument> where TDocument : ReportDocument
{
    string ExportType { get; }

    Task<Stream> ExportAsync(TDocument document, CancellationToken cancellationToken);
}