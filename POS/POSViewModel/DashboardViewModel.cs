using System.ComponentModel.DataAnnotations;

namespace POS.POSViewModels
{
    public class DashboardViewModel
    {
        public int productsCount { get; set; } 
        public int TodaysInvoice { get; set; } = 0;
        public decimal TodaySale { get; set; } = 0;
    }
     
}