using System.ComponentModel.DataAnnotations;

namespace Hermes.Domain.Entities;

public class Inventory : BaseEntity 
{
    public int ProductVariantId { get; set; } 
    public ProductVariant ProductVariant { get; set; }

    public int QuantityOnHand { get; set; }  
    public int ReservedQuantity { get; set; }

    public int ReorderThreshold { get; set; }
    public bool IsReorderNeeded { get; set; }
    
    [Timestamp]
    public byte[]? RowVersion { get; set; } 
}