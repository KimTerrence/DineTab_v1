namespace DineTab_v1.Models
{
    public class SoldItemReport
    {
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string Type { get; set; }
        public int TotalItem { get; set; }  
        public decimal TotalPrice { get; set; }
    }
}
