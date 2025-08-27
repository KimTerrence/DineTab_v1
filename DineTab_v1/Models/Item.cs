namespace DineTab_v1.Models
{
    //model for  order item
    public class Item
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }

        public string Availability { get; set; } = "Available";
        public string Spicy { get; set; } = "No";
        public byte[]? Image { get; set; }
        public ImageSource ItemImageSource
        {
            get
            {
                if (Image == null || Image.Length == 0)
                    return "icon.png"; // fallback image from Resources

                try
                {
                    return ImageSource.FromStream(() => new MemoryStream(Image));
                }
                catch
                {
                    return "icon.png"; // fallback if decode fails
                }
            }
        }
        public string CategoryName { get; set; } = string.Empty;
    }
}
