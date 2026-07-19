namespace SJP.Accounting.Domain.Models;

public class ReportConfiguration
{
    public List<ReportDefinition> Reports { get; set; }
        = new();
}