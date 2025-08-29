using SkiaSharp;

public class PdfService
{
    public async Task<string> CreateSalesReportAsync(
        IEnumerable<(string ItemName, int Qty, decimal Price)> items,
        int totalSold,
        int totalOrders,
        decimal totalRevenue)
    {
        var filePath = Path.Combine(FileSystem.AppDataDirectory, "SalesReport.pdf");

        using var stream = File.OpenWrite(filePath);
        using var document = SKDocument.CreatePdf(stream);
        using var canvas = document.BeginPage(595, 842); // A4 size

        var titlePaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 22,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold),
            IsAntialias = true
        };

        var headerPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 14,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold),
            IsAntialias = true
        };

        var bodyPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 12,
            Typeface = SKTypeface.FromFamilyName("Arial"),
            IsAntialias = true
        };

        var boldBodyPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 12,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold),
            IsAntialias = true
        };

        float margin = 40;
        float y = 60;
        float tableWidth = 500;
        float rowHeight = 22;

        // Center Title
        var title = "Sales Report";
        float titleWidth = titlePaint.MeasureText(title);
        canvas.DrawText(title, (595 - titleWidth) / 2, y, titlePaint);
        y += 50;

        // Table column positions
        float colItem = margin;
        float colQty = 350;
        float colPrice = 450;

        // Draw header background
        var headerBg = new SKPaint { Color = SKColors.LightGray };
        canvas.DrawRect(margin - 5, y - 15, tableWidth, rowHeight, headerBg);

        // Headers
        canvas.DrawText("Item", colItem, y, headerPaint);
        canvas.DrawText("Quantity", colQty, y, headerPaint);
        canvas.DrawText("Price", colPrice, y, headerPaint);

        y += rowHeight;

        // Table rows with alternating background
        bool shade = false;
        foreach (var item in items)
        {
            if (shade)
            {
                canvas.DrawRect(margin - 5, y - 15, tableWidth, rowHeight,
                    new SKPaint { Color = new SKColor(240, 240, 240) });
            }

            canvas.DrawText(item.ItemName, colItem, y, bodyPaint);

            // Right align Qty
            var qtyText = item.Qty.ToString();
            canvas.DrawText(qtyText, colQty + 60 - bodyPaint.MeasureText(qtyText), y, bodyPaint);

            // Right align Price
            var priceText = $"₱{item.Price:N2}";
            canvas.DrawText(priceText, colPrice + 80 - bodyPaint.MeasureText(priceText), y, bodyPaint);

            y += rowHeight;
            shade = !shade;
        }

        y += 30;

        // Totals Section (Bold)
        canvas.DrawText($"Total Items: {totalSold}", margin, y, boldBodyPaint); y += 20;
        canvas.DrawText($"Total Orders: {totalOrders}", margin, y, boldBodyPaint); y += 20;
        canvas.DrawText($"Total Revenue: ₱{totalRevenue:N2}", margin, y, boldBodyPaint);

        document.EndPage();
        document.Close();

        return filePath;
    }
}
