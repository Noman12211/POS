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
                .OrderBy(x => x.Name)
                .ToListAsync();

            ViewBag.FoodItems = foodItems;

            return View(new InvoiceCreateViewModel());
        }


        [HttpPost]
        public async Task<IActionResult> Save([FromBody] InvoiceSaveRequest request)
        {
            if (request.Items == null || request.Items.Count == 0)
            {
                return BadRequest(
                    "Invoice must contain at least one item.");
            }


            var foodItemIds = request.Items
                .Select(x => x.FoodItemId)
                .Distinct()
                .ToList(); 


            var foodItems = await _context.FoodItems
                .Where(x =>
                    x.IsActive &&
                    foodItemIds.Contains(x.Id))
                .ToListAsync();


            if (foodItems.Count() != foodItemIds.Count())
            {
                return BadRequest(
                    "One or more food items are invalid.");
            }


            var invoicedto = new Invoice
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


                var foodItem = foodItems
                    .First(x =>
                        x.Id == requestItem.FoodItemId);


                var totalPrice =
                    foodItem.Price *
                    requestItem.Quantity;


                invoicedto.InvoiceItems.Add(
                    new InvoiceItem
                    {
                        FoodItemId =
                            foodItem.Id,

                        ItemName =
                            foodItem.Name,

                        Quantity =
                            requestItem.Quantity,

                        UnitPrice =
                            foodItem.Price,

                        TotalPrice =
                            totalPrice
                    });


                grandTotal += totalPrice;
            }


            invoicedto.GrandTotal =
                grandTotal;

             
            _context.Invoices.Add(invoicedto);

            await _context.SaveChangesAsync();


            return Json(new
            {
                success = true,
                invoiceNumber =
                    invoicedto.InvoiceNumber,

                invoiceId =
                    invoicedto.Id
            });
        }


        private async Task<string>GenerateInvoiceNumber()
        {
            var lastInvoice = await _context.Invoices?
                    .OrderByDescending(x => x.Id).FirstOrDefaultAsync();


            var nextNumber = lastInvoice == null ? 1 : lastInvoice.Id + 1;


            return $"INV-{nextNumber:D6}";
        }
    }


    public class InvoiceSaveRequest
    {
        public List<InvoiceSaveItem> Items { get; set; } = new List<InvoiceSaveItem>();
    }


    public class InvoiceSaveItem
    {
        public int FoodItemId { get; set; }

        public int Quantity { get; set; }
    }
}