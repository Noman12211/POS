using System.ComponentModel.DataAnnotations;

namespace POS.POSViewModels
{
    public class InvoiceCreateViewModel
    {
        public List<InvoiceItemViewModel> Items { get; set; }
            = new List<InvoiceItemViewModel>();

        public decimal GrandTotal { get; set; }
    }

    public class InvoiceItemViewModel
    {
        public int FoodItemId { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        [Range(1, 1000)]
        public int Quantity { get; set; }

        public decimal TotalPrice =>
            UnitPrice * Quantity;
    }
}