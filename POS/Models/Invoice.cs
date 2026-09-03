using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace POS.Models;

[Index("InvoiceNumber", Name = "IX_Invoices_InvoiceNumber", IsUnique = true)]
public partial class Invoice
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string InvoiceNumber { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime InvoiceDate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal GrandTotal { get; set; }

    [InverseProperty("Invoice")]
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
}
