using System.ComponentModel.DataAnnotations;

namespace POS.POSViewModels
{
    public class FoodItemCreateViewModel
    {
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(200)]
        [Display(Name = "Product Name")]
        public string Name { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Price must be 0 or greater.")]
        [Display(Name = "Base Price")]
        public decimal Price { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public List<FoodItemVariantViewModel>? Variants { get; set; }
            = new List<FoodItemVariantViewModel>();
    }

    public class FoodItemVariantViewModel
    {
        [StringLength(100)]
        [Display(Name = "Variant Name")]
        public string VariantName { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Price must be 0 or greater.")]
        public decimal Price { get; set; }

        [Display(Name = "Custom Price")]
        public bool IsCustomPrice { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}