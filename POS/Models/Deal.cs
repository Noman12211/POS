namespace POS.Models;
public class Deal
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal DealPrice { get; set; }

    public bool IsActive { get; set; } = true;
    public string? ImagePath { get; set; }


    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ICollection<DealItem> DealItems { get; set; } = new List<DealItem>();
}
