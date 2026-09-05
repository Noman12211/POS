using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS.Models;

public partial class FoodItemVariant
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int FoodItemId { get; set; }

    [Required]
    [StringLength(100)]
    public string VariantName { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    // True when cashier can enter the price manually
    public bool IsCustomPrice { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation
    [ForeignKey(nameof(FoodItemId))]
    public virtual FoodItem? FoodItem { get; set; } = null!;

    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; }
        = new List<InvoiceItem>();
}