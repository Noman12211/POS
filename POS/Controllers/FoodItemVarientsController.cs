using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.Models;

namespace POS.Controllers
{
    public class FoodItemVariantController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FoodItemVariantController(ApplicationDbContext context)
        {
            _context = context;
        }
         
        public async Task<IActionResult> Index()
        {
            var variants = await _context.FoodItemVariants
                .Include(v => v.FoodItem)
                .OrderBy(v => v.FoodItem.Name)
                .ThenBy(v => v.VariantName)
                .ToListAsync();

            return View(variants);
        }

        // GET: FoodItemVariant/Create
        public async Task<IActionResult> Create(int? foodItemId)
        {
            var foodItems = await _context.FoodItems
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync();

            ViewBag.FoodItems = foodItems;

            var model = new FoodItemVariant
            {
                IsActive = true
            };

            if (foodItemId.HasValue)
            {
                var foodItem = await _context.FoodItems
                    .FirstOrDefaultAsync(x => x.Id == foodItemId.Value);

                if (foodItem == null)
                {
                    return NotFound();
                }

                model.FoodItemId = foodItem.Id;
            }

            return View(model);
        }

        // POST: FoodItemVariant/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FoodItemVariant model)
        {
            if (string.IsNullOrWhiteSpace(model.VariantName))
            {
                ModelState.AddModelError(
                    nameof(model.VariantName),
                    "Variant name is required.");
            }

            model.FoodItem = await _context.FoodItems
                .FirstOrDefaultAsync(x => x.Id == model.FoodItemId);

            if (model.FoodItem == null)
            {
                ModelState.AddModelError(
                    nameof(model.FoodItemId),
                    "Please select a valid product.");
            }

            if (!model.IsCustomPrice && model.Price < 0)
            {
                ModelState.AddModelError(
                    nameof(model.Price),
                    "Price must be 0 or greater.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.FoodItems = await _context.FoodItems
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.Name)
                    .ToListAsync();

                return View(model);
            }

            var variant = new FoodItemVariant
            {
                FoodItemId = model.FoodItemId,
                VariantName = model.VariantName.Trim(),
                Price = model.IsCustomPrice ? 0 : model.Price,
                IsCustomPrice = model.IsCustomPrice,
                IsActive = model.IsActive
            };

            _context.FoodItemVariants.Add(variant);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Variant created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: FoodItemVariant/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var variant = await _context.FoodItemVariants
                .Include(v => v.FoodItem)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (variant == null)
            {
                return NotFound();
            }

            ViewBag.FoodItems = await _context.FoodItems
                .OrderBy(x => x.Name)
                .ToListAsync();

            return View(variant);
        }

        // POST: FoodItemVariant/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            FoodItemVariant model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(model.VariantName))
            {
                ModelState.AddModelError(
                    nameof(model.VariantName),
                    "Variant name is required.");
            }

            var foodItem = await _context.FoodItems
                .FirstOrDefaultAsync(x => x.Id == model.FoodItemId);

            if (foodItem == null)
            {
                ModelState.AddModelError(
                    nameof(model.FoodItemId),
                    "Please select a valid product.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.FoodItems = await _context.FoodItems
                    .OrderBy(x => x.Name)
                    .ToListAsync();

                return View(model);
            }

            var variant = await _context.FoodItemVariants
                .FirstOrDefaultAsync(x => x.Id == id);

            if (variant == null)
            {
                return NotFound();
            }

            variant.FoodItemId = model.FoodItemId;
            variant.VariantName = model.VariantName.Trim();
            variant.Price = model.IsCustomPrice ? 0 : model.Price;
            variant.IsCustomPrice = model.IsCustomPrice;
            variant.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Variant updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // POST: FoodItemVariant/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var variant = await _context.FoodItemVariants
                .FirstOrDefaultAsync(x => x.Id == id);

            if (variant == null)
            {
                return NotFound();
            }

            _context.FoodItemVariants.Remove(variant);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Variant deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}