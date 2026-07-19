namespace SJP.Accounting.Domain.Models;

public class SheetDefinition
{
    public string SheetName { get; set; } = string.Empty;

    public string SourceType { get; set; } = string.Empty;

    public string SourceName { get; set; } = string.Empty;
}