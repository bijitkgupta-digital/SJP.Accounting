using System.ComponentModel.DataAnnotations.Schema;

namespace SJP.Accounting.Domain.Entities;

[Table("TransactionMaster")]
public class TransactionMaster
{
    public Guid TransactionId { get; set; }

    public string TransactionHash { get; set; } = string.Empty;

    public DateTime TransactionDate { get; set; }

    public int? ProjectId { get; set; }

    public int? CategoryId { get; set; }

    public int TransactionTypeId { get; set; }

    public decimal Amount { get; set; }

    public int? PaidByEntityId { get; set; }

    public int? ReceivedByEntityId { get; set; }

    public string? Narration { get; set; }

    public string? GoogleDriveLink { get; set; }

    public DateTime ImportedOn { get; set; }

    public Project? Project { get; set; }

    public Category? Category { get; set; }

    public TransactionType TransactionType { get; set; } = null!;

    public Entity? PaidByEntity { get; set; }

    public Entity? ReceivedByEntity { get; set; }

    public ICollection<Asset> Assets { get; set; }
        = new List<Asset>();
}