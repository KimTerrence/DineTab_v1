using Microsoft.Maui.Graphics;

namespace DineTab_v1.Models
{
    public class NotificationItem
    {
        public string OrderNumber { get; set; }
        public string Status { get; set; }

        // Dynamic message based on Status
        public string Message
        {
            get
            {
                return Status?.ToLower() switch
                {
                    "complete" => $"Order {OrderNumber} is Completed",
                    "preparing" => $"Order {OrderNumber} is being Prepared",
                    "paid" => $"Order {OrderNumber} has been Paid",
                    "pending" => $"Order {OrderNumber} is Pending",
                    "ready" => $"Order {OrderNumber} is Ready",
                    "canceled" => $"Order {OrderNumber} is Canceled",
                    _ => $"Order {OrderNumber} status: {Status}"
                };
            }
        }

        // Status color based on Status value
        public Color StatusColor
        {
            get
            {
                return Status?.ToLower() switch
                {
                    "complete" => Colors.LightBlue,
                    "preparing" => Colors.LightGray,
                    "paid" => Colors.LightGreen,
                    "pending" => Colors.LightPink,
                    "ready" => Colors.LightGreen,
                    "canceled" => Colors.LightPink,
                    _ => Colors.Black
                };
            }
        }
    }
}
