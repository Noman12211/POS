using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.Models;

namespace POS.Controllers
{
    public class FoodItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FoodItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _context.FoodItems
                .Where(x => x.IsActive)
                .ToListAsync();

            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _context.FoodItems
                
                .FirstOrDefaultAsync(x => x.Id == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FoodItem foodItem)
        {
            if (!ModelState.IsValid)
            {
                return View(foodItem);
            }

            foodItem.CreatedDate = DateTime.Now;

            _context.FoodItems.Add(foodItem);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}