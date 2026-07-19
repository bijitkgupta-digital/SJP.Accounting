namespace SJP.Accounting.Domain.Models;

public class ReportDefinition
{
    public string ReportName { get; set; } = string.Empty;

    public string Frequency { get; set; } = string.Empty;

    public int RunOnlyOnDay { get; set; }

    public string OutputFolder { get; set; } = string.Empty;

    public List<string> OutputFormats { get; set; }
        = new();

    public List<SheetDefinition> Sheets { get; set; }
        = new();
}