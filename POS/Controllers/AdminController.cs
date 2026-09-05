using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.Models;

namespace POS.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Dashboard()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var totalProduct = await _context.FoodItems
                .CountAsync();

            var todayInvoices = await _context.Invoices
                .Where(x => x.InvoiceDate >= today &&
                            x.InvoiceDate < tomorrow)
                .ToListAsync();

            var todayInvoice = todayInvoices.Count;

            var todaySale = todayInvoices
                .Sum(x => x.GrandTotal);

            var viewModel = new POSViewModels.DashboardViewModel
            {
                productsCount = totalProduct,
                TodaysInvoice = todayInvoice,
                TodaySale = todaySale
            };

            return View(viewModel);
        }
    }
}