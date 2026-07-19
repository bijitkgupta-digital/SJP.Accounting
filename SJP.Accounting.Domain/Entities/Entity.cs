using System.ComponentModel.DataAnnotations.Schema;

namespace SJP.Accounting.Domain.Entities;

[Table("Entity")]
public class Entity
{
    public int EntityId { get; set; }

    public string EntityType { get; set; } = string.Empty;

    public string EntityName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<TransactionMaster> PaidTransactions { get; set; }

    public ICollection<TransactionMaster> ReceivedTransactions { get; set; }
}