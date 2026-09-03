using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace POS.Models;

public partial class InvoiceItem
{
    [Key]
    public int Id { get; set; }

    public int InvoiceId { get; set; }

    public int FoodItemId { get; set; }

    [StringLength(200)]
    public string ItemName { get; set; } = null!;

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalPrice { get; set; }

    [ForeignKey("FoodItemId")]
    [InverseProperty("InvoiceItems")]
    public virtual FoodItem FoodItem { get; set; } = null!;

    [ForeignKey("InvoiceId")]
    [InverseProperty("InvoiceItems")]
    public virtual Invoice Invoice { get; set; } = null!;
}
