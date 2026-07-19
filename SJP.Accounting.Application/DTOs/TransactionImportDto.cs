using System.Security.Cryptography;
using System.Text;

namespace SJP.Accounting.Application.DTOs;

public class TransactionImportDto
{
    public DateTime TransactionDate { get; set; }

    public string ProjectName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public string TransactionType { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string PaidByType { get; set; } = string.Empty;

    public string PaidBy { get; set; } = string.Empty;

    public string ReceivedByType { get; set; } = string.Empty;

    public string ReceivedBy { get; set; } = string.Empty;

    public string? Narration { get; set; }

    public string? GoogleDriveLink { get; set; }

    public string HashValue
    {
        get
        {
            var rawValue =
                $"{TransactionDate:yyyy-MM-dd}|{ProjectName}|{CategoryName}|{TransactionType}|{Amount}|{PaidBy}|{PaidByType}|{ReceivedByType}|{ReceivedBy}|{Narration}|{GoogleDriveLink}";

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawValue));
            return Convert.ToHexString(hashBytes);
        }
    }
}