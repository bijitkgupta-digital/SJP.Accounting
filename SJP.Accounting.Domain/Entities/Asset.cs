using System.ComponentModel.DataAnnotations.Schema;

namespace SJP.Accounting.Domain.Entities;

[Table("Asset")]
public class Asset
{
    public int AssetId { get; set; }

    public Guid TransactionId { get; set; }

    public string AssetName { get; set; } = string.Empty;

    public decimal PurchaseValue { get; set; }

    public DateTime PurchaseDate { get; set; }

    public string AssetStatus { get; set; } = string.Empty;

    public TransactionMaster Transaction { get; set; } = null!;
}