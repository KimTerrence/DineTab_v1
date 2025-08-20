namespace DineTab_v1.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }

        public string Availability { get; set; } = "Available";
        public string Spicy { get; set; } = "No";
        public byte[]? Image { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
