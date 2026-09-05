using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.Models;
using POS.POSViewModels;

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
            FoodItemCreateViewModel model =new FoodItemCreateViewModel();
            return View(model);
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
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var foodItem = await _context.FoodItems
                .Include(x => x.Variants)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (foodItem == null)
            {
                return NotFound();
            }

            var model = new FoodItemCreateViewModel
            {
                Id = foodItem.Id,
                Name = foodItem.Name,
                IsActive = foodItem.IsActive,

                Variants = foodItem.Variants
                    .Select(v => new FoodItemVariantViewModel
                    {
                        Id = v.Id,
                        VariantName = v.VariantName,
                        Price = v.Price,
                        IsCustomPrice = v.IsCustomPrice,
                        IsActive = v.IsActive
                    })
                    .ToList()
            };

            return View(model);
        }


        // -----------------------------------------
        // EDIT - POST
        // -----------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FoodItemCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var foodItem = await _context.FoodItems
                .Include(x => x.Variants)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (foodItem == null)
            {
                return NotFound();
            }


            // Update product
            foodItem.Name = model.Name;
            foodItem.IsActive = model.IsActive;


            // Existing variant IDs submitted from the form
            var submittedVariantIds = model.Variants
                .Where(x => x.Id > 0)
                .Select(x => x.Id)
                .ToHashSet();


            // Delete variants removed from the page
            var variantsToDelete = foodItem.Variants
                .Where(x => !submittedVariantIds.Contains(x.Id))
                .ToList();

            foreach (var variant in variantsToDelete)
            {
                _context.FoodItemVariants.Remove(variant);
            }


            // Update existing / add new variants
            foreach (var variantModel in model.Variants)
            {
                if (string.IsNullOrWhiteSpace(variantModel.VariantName))
                    continue;


                // Existing variant
                if (variantModel.Id > 0)
                {
                    var variant = foodItem.Variants
                        .FirstOrDefault(x => x.Id == variantModel.Id);

                    if (variant == null)
                        continue;

                    variant.VariantName = variantModel.VariantName;

                    variant.Price = variantModel.IsCustomPrice
                        ? 0
                        : variantModel.Price;

                    variant.IsCustomPrice = variantModel.IsCustomPrice;

                    variant.IsActive = variantModel.IsActive;
                }

                // New variant
                else
                {
                    var newVariant = new FoodItemVariant
                    {
                        FoodItemId = foodItem.Id,
                        VariantName = variantModel.VariantName,

                        Price = variantModel.IsCustomPrice
                            ? 0
                            : variantModel.Price,

                        IsCustomPrice = variantModel.IsCustomPrice,
                        IsActive = variantModel.IsActive
                    };

                    _context.FoodItemVariants.Add(newVariant);
                }
            }


            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}