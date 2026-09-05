using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS.Models;

public partial class InvoiceItem
{
    [Key]
    public int Id { get; set; }

    public int InvoiceId { get; set; }

    public int FoodItemId { get; set; }

    public int FoodItemVariantId { get; set; }

    // Snapshot of product name at time of sale
    [Required]
    [StringLength(200)]
    public string ItemName { get; set; } = null!;

    // Snapshot of size/quantity option
    [Required]
    [StringLength(100)]
    public string VariantName { get; set; } = null!;

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalPrice { get; set; }

    // Navigation
    [ForeignKey(nameof(InvoiceId))]
    public virtual Invoice Invoice { get; set; } = null!;

    [ForeignKey(nameof(FoodItemId))]
    public virtual FoodItem FoodItem { get; set; } = null!;

    [ForeignKey(nameof(FoodItemVariantId))]
    public virtual FoodItemVariant FoodItemVariant { get; set; } = null!;
}