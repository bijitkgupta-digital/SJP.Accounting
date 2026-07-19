using System.ComponentModel.DataAnnotations.Schema;

namespace SJP.Accounting.Domain.Entities;

[Table("Project")]
public class Project
{
    public int ProjectId { get; set; }

    public string ProjectName { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public ICollection<TransactionMaster> Transactions { get; set; }
        = new List<TransactionMaster>();
}