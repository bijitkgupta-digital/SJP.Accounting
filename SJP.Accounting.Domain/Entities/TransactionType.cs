using System.ComponentModel.DataAnnotations.Schema;

namespace SJP.Accounting.Domain.Entities;

[Table("TransactionType")]
public class TransactionType
{
    public int TransactionTypeId { get; set; }

    public string TransactionTypeName { get; set; } = string.Empty;

    public ICollection<TransactionMaster> Transactions { get; set; }
        = new List<TransactionMaster>();
}