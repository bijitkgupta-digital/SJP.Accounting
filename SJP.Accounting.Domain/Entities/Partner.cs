using System.ComponentModel.DataAnnotations.Schema;

namespace SJP.Accounting.Domain.Entities;

[Table("Partner")]
public class Partner
{
    public int PartnerId { get; set; }

    public string PartnerName { get; set; } = string.Empty;

    public decimal ProfitSharePercentage { get; set; }

    public bool IsActive { get; set; }
}