namespace SJP.Accounting.Application.DTOs;

public class ImportResultDto
{
    public int TotalRows { get; set; }

    public int ImportedRows { get; set; }

    public int DuplicateRows { get; set; }

    public int ErrorRows { get; set; }

    public List<ValidationErrorDto> ImportErrors { get; set; } = new();

    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;
}