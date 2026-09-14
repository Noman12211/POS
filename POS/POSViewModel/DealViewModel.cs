using System.ComponentModel.DataAnnotations;

namespace POS.POSViewModels
{
    // ----- View models -----

    public class DealFormViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal DealPrice { get; set; }
        public bool IsActive { get; set; } = true;
        [StringLength(500)]
        public string? ImagePath { get; set; }

        [Display(Name = "Deal Image")]
        public IFormFile? ImageFile { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<DealItemInputModel> Items { get; set; } = new();
    }

    public class DealItemInputModel
    {
        public int FoodItemVariantId { get; set; }
        public int Quantity { get; set; } = 1;
    }

}