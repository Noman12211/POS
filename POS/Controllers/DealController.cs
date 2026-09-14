using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.Models;
using POS.POSViewModels;

namespace POS.Controllers
{
    public class DealController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DealController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Deal
        public async Task<IActionResult> Index()
        {
            try { 
            var deals = await _context.Deals
                .Include(d => d.DealItems)
                    .ThenInclude(di => di.FoodItemVariant)
                        .ThenInclude(v => v.FoodItem)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return View(deals);
            }
            catch (Exception ex) { throw ex; }
        }

        // GET: Deal/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var deal = await _context.Deals
                .Include(d => d.DealItems)
                    .ThenInclude(di => di.FoodItemVariant)
                        .ThenInclude(v => v.FoodItem)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (deal == null)
            {
                return NotFound();
            }

            return View(deal);
        }

        // GET: Deal/Create
        public async Task<IActionResult> Create()
        {
            await PopulateVariantsViewBag();

            var model = new DealFormViewModel
            {
                IsActive = true,
                Items = new List<DealItemInputModel> { new DealItemInputModel() }
            };

            return View(model);
        }

        // POST: Deal/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DealFormViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Deal name is required.");
            }

            if (model.DealPrice < 0)
            {
                ModelState.AddModelError(nameof(model.DealPrice), "Deal price must be 0 or greater.");
            }

            if (model.StartDate.HasValue && model.EndDate.HasValue
                && model.EndDate.Value < model.StartDate.Value)
            {
                ModelState.AddModelError(nameof(model.EndDate), "End date cannot be before start date.");
            }

            var validItems = (model.Items ?? new List<DealItemInputModel>())
                .Where(i => i.FoodItemVariantId > 0 && i.Quantity > 0)
                .ToList();

            if (validItems.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Add at least one item to the deal.");
            }
            else
            {
                var variantIds = validItems.Select(i => i.FoodItemVariantId).ToList();
                var existingCount = await _context.FoodItemVariants
                    .CountAsync(v => variantIds.Contains(v.Id));

                if (existingCount != variantIds.Distinct().Count())
                {
                    ModelState.AddModelError(string.Empty, "One or more selected items are invalid.");
                }
            }

            if (!ModelState.IsValid)
            {
                await PopulateVariantsViewBag();
                return View(model);
            }

            var deal = new Deal
            {
                Name = model.Name.Trim(),
                Description = model.Description?.Trim(),
                DealPrice = model.DealPrice,
                IsActive = model.IsActive,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                CreatedAt = DateTime.UtcNow,
                DealItems = validItems
                    .GroupBy(i => i.FoodItemVariantId)
                    .Select(g => new DealItem
                    {
                        FoodItemVariantId = g.Key,
                        Quantity = g.Sum(x => x.Quantity)
                    })
                    .ToList()
            };

            _context.Deals.Add(deal);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Deal created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Deal/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var deal = await _context.Deals
                .Include(d => d.DealItems)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (deal == null)
            {
                return NotFound();
            }

            await PopulateVariantsViewBag();

            var model = new DealFormViewModel
            {
                Id = deal.Id,
                Name = deal.Name,
                Description = deal.Description,
                DealPrice = deal.DealPrice,
                IsActive = deal.IsActive,
                StartDate = deal.StartDate,
                EndDate = deal.EndDate,
                Items = deal.DealItems
                    .Select(di => new DealItemInputModel
                    {
                        FoodItemVariantId = di.FoodItemVariantId,
                        Quantity = di.Quantity
                    })
                    .ToList()
            };

            if (model.Items.Count == 0)
            {
                model.Items.Add(new DealItemInputModel());
            }

            return View(model);
        }

        // POST: Deal/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DealFormViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Deal name is required.");
            }

            if (model.DealPrice < 0)
            {
                ModelState.AddModelError(nameof(model.DealPrice), "Deal price must be 0 or greater.");
            }

            if (model.StartDate.HasValue && model.EndDate.HasValue
                && model.EndDate.Value < model.StartDate.Value)
            {
                ModelState.AddModelError(nameof(model.EndDate), "End date cannot be before start date.");
            }

            var validItems = (model.Items ?? new List<DealItemInputModel>())
                .Where(i => i.FoodItemVariantId > 0 && i.Quantity > 0)
                .ToList();

            if (validItems.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Add at least one item to the deal.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateVariantsViewBag();
                return View(model);
            }

            var deal = await _context.Deals
                .Include(d => d.DealItems)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (deal == null)
            {
                return NotFound();
            }

            deal.Name = model.Name.Trim();
            deal.Description = model.Description?.Trim();
            deal.DealPrice = model.DealPrice;
            deal.IsActive = model.IsActive;
            deal.StartDate = model.StartDate;
            deal.EndDate = model.EndDate;
            deal.UpdatedAt = DateTime.UtcNow;

            // Replace deal items wholesale (simplest, avoids diffing logic)
            _context.DealItems.RemoveRange(deal.DealItems);

            deal.DealItems = validItems
                .GroupBy(i => i.FoodItemVariantId)
                .Select(g => new DealItem
                {
                    DealId = deal.Id,
                    FoodItemVariantId = g.Key,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .ToList();

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Deal updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Deal/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var deal = await _context.Deals
                .Include(d => d.DealItems)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (deal == null)
            {
                return NotFound();
            }

            _context.Deals.Remove(deal); // DealItems cascade-delete via FK config

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Deal deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Deal/ToggleActive/5  (handy for a quick enable/disable button)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var deal = await _context.Deals.FirstOrDefaultAsync(d => d.Id == id);

            if (deal == null)
            {
                return NotFound();
            }

            deal.IsActive = !deal.IsActive;
            deal.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateVariantsViewBag()
        {
            ViewBag.FoodItemVariants = await _context.FoodItemVariants
                .Include(v => v.FoodItem)
                .Where(v => v.IsActive)
                .OrderBy(v => v.FoodItem.Name)
                .ThenBy(v => v.VariantName)
                .ToListAsync();
        }
    }

    
}