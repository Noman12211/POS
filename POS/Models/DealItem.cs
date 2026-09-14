namespace POS.Models;
public class DealItem
{
    public int Id { get; set; }

    public int DealId { get; set; }

    public int FoodItemVariantId { get; set; }

    public int Quantity { get; set; } = 1;

    // Navigation
    public Deal Deal { get; set; } = null!;

    public FoodItemVariant FoodItemVariant { get; set; } = null!;
}
