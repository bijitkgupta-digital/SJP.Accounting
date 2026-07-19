using System.ComponentModel.DataAnnotations.Schema;

namespace SJP.Accounting.Domain.Entities;

[Table("Category")]
public class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public ICollection<TransactionMaster> Transactions { get; set; }
        = new List<TransactionMaster>();
}