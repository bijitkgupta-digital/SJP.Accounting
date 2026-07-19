namespace SJP.Accounting.Application.Contracts
{
    public interface ISJPAccountingReport
    {
        string ReportCode { get; }
        string ReportName { get; }

        Task<IReadOnlyDictionary<string, Stream>> ExportAsync(CancellationToken cancellationToken);
    }
}