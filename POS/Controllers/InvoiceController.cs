using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.Models;
using POS.POSViewModels;

namespace POS.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InvoiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var invoices = await _context.Invoices
                .Include(x => x.InvoiceItems)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return View(invoices);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _context.Invoices
                .Include(x => x.InvoiceItems)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var foodItems = await _context.FoodItems
                .Where(x => x.IsActive)
                .Include(x => x.Variants)
                .OrderBy(x => x.Name)
                .ToListAsync();

            ViewBag.FoodItems = foodItems;

            return View(new InvoiceCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Save(
            [FromBody] InvoiceSaveRequest request)
        {
            if (request.Items == null || request.Items.Count == 0)
            {
                return BadRequest(
                    "Invoice must contain at least one item.");
            }

            // Get all selected variants
            var variantIds = request.Items
                .Select(x => x.FoodItemVariantId)
                .Distinct()
                .ToList();

            var variants = await _context.FoodItemVariants
                .Include(x => x.FoodItem)
                .Where(x =>
                    x.IsActive &&
                    x.FoodItem.IsActive &&
                    variantIds.Contains(x.Id))
                .ToListAsync();

            // Check that all requested variants exist
            if (variants.Count != variantIds.Count)
            {
                return BadRequest(
                    "One or more food item variants are invalid.");
            }

            var invoice = new Invoice
            {
                InvoiceNumber = await GenerateInvoiceNumber(),
                InvoiceDate = DateTime.Now
            };

            decimal grandTotal = 0;

            foreach (var requestItem in request.Items)
            {
                if (requestItem.Quantity <= 0)
                {
                    return BadRequest(
                        "Quantity must be greater than zero.");
                }

                var variant = variants.First(
                    x => x.Id == requestItem.FoodItemVariantId);

                decimal unitPrice = variant.Price;

                // Custom price
                if (variant.IsCustomPrice)
                {
                    if (!requestItem.CustomPrice.HasValue ||
                        requestItem.CustomPrice.Value <= 0)
                    {
                        return BadRequest(
                            "Please enter a valid custom price.");
                    }

                    unitPrice = requestItem.CustomPrice.Value;
                }

                var totalPrice =
                    unitPrice * requestItem.Quantity;

                invoice.InvoiceItems.Add(
                    new InvoiceItem
                    {
                        FoodItemId = variant.FoodItemId,

                        FoodItemVariantId = variant.Id,

                        ItemName = variant.FoodItem.Name,

                        VariantName = variant.VariantName,

                        Quantity = requestItem.Quantity,

                        UnitPrice = unitPrice,

                        TotalPrice = totalPrice
                    });

                grandTotal += totalPrice;
            }

            invoice.GrandTotal = grandTotal;

            _context.Invoices.Add(invoice);

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                invoiceNumber = invoice.InvoiceNumber,
                invoiceId = invoice.Id
            });
        }

        private async Task<string> GenerateInvoiceNumber()
        {
            var lastInvoice = await _context.Invoices
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            var nextNumber = lastInvoice == null
                ? 1
                : lastInvoice.Id + 1;

            return $"INV-{nextNumber:D6}";
        }
    }

    public class InvoiceSaveRequest
    {
        public List<InvoiceSaveItem> Items { get; set; }
            = new List<InvoiceSaveItem>();
    }

    public class InvoiceSaveItem
    {
        public int FoodItemVariantId { get; set; }

        public int Quantity { get; set; }

        // Used only when the selected variant allows custom pricing
        public decimal? CustomPrice { get; set; }
    }
}