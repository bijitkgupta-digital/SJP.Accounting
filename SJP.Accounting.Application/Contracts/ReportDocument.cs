namespace SJP.Accounting.Application.Reports;

public abstract class ReportDocument
{
    public string ReportCode { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public DateTime GeneratedOn { get; set; } = DateTime.UtcNow;
}