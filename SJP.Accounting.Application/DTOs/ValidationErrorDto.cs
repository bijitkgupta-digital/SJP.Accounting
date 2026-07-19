namespace SJP.Accounting.Application.DTOs;

public class ValidationErrorDto
{
    public int RowNumber { get; set; }
    public string ColumnName { get; set; } = string.Empty;
    public string FieldValue { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}