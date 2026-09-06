using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace POS.Models;

public partial class FoodItem
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Name { get; set; } = null!;
    [StringLength(500)]
    public string? ImagePath { get; set; }

    // Product can have multiple sizes/quantities
    public virtual ICollection<FoodItemVariant> Variants { get; set; }
        = new List<FoodItemVariant>();

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [InverseProperty("FoodItem")]
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
}
