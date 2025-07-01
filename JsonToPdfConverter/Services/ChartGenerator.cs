using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using JsonToPdfConverter.Models;

namespace JsonToPdfConverter.Services
{
    public class ChartGenerator
    {
        public byte[] GeneratePieChart(List<SalesData> data, string title, string labelSource, string valueSource)
        {
            int width = 400;
            int height = 300;
            
            using var bitmap = new Bitmap(width, height);
            using var graphics = Graphics.FromImage(bitmap);
            
            graphics.Clear(Color.White);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            // Calculate total for percentages
            decimal total = data.Sum(d => d.Revenue);
            
            // Chart area
            Rectangle chartRect = new Rectangle(50, 50, 200, 200);
            
            // Draw pie slices
            float startAngle = 0;
            Color[] colors = { Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Purple, Color.Yellow };
            
            for (int i = 0; i < data.Count; i++)
            {
                float sweepAngle = (float)(data[i].Revenue / total * 360);
                using var brush = new SolidBrush(colors[i % colors.Length]);
                graphics.FillPie(brush, chartRect, startAngle, sweepAngle);
                startAngle += sweepAngle;
            }
            
            // Title is handled by PDF Title component, not embedded in chart
            
            // Draw legend
            int legendY = 50;
            using var legendFont = new Font("Arial", 10);
            for (int i = 0; i < data.Count; i++)
            {
                using var legendBrush = new SolidBrush(colors[i % colors.Length]);
                graphics.FillRectangle(legendBrush, 270, legendY, 15, 15);
                
                using var textBrush = new SolidBrush(Color.Black);
                string legendText = $"{data[i].Month}: ${data[i].Revenue:N0}";
                graphics.DrawString(legendText, legendFont, textBrush, new PointF(290, legendY));
                legendY += 25;
            }
            
            using var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            return stream.ToArray();
        }
        
        public byte[] GenerateBarChart(List<SalesData> data, string title)
        {
            int width = 500;
            int height = 300;
            
            using var bitmap = new Bitmap(width, height);
            using var graphics = Graphics.FromImage(bitmap);
            
            graphics.Clear(Color.White);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            // Chart area
            Rectangle chartRect = new Rectangle(80, 50, 350, 200);
            
            // Find max value for scaling
            decimal maxValue = data.Max(d => d.Revenue);
            
            // Draw bars
            int barWidth = chartRect.Width / data.Count - 10;
            for (int i = 0; i < data.Count; i++)
            {
                int barHeight = (int)(data[i].Revenue / maxValue * chartRect.Height);
                int x = chartRect.X + i * (barWidth + 10);
                int y = chartRect.Bottom - barHeight;
                
                using var brush = new SolidBrush(Color.SteelBlue);
                graphics.FillRectangle(brush, x, y, barWidth, barHeight);
                
                // Draw value labels
                using var font = new Font("Arial", 8);
                using var textBrush = new SolidBrush(Color.Black);
                string valueText = $"${data[i].Revenue:N0}";
                SizeF textSize = graphics.MeasureString(valueText, font);
                graphics.DrawString(valueText, font, textBrush, 
                    x + (barWidth - textSize.Width) / 2, y - 20);
                
                // Draw month labels
                graphics.DrawString(data[i].Month, font, textBrush, 
                    x + (barWidth - graphics.MeasureString(data[i].Month, font).Width) / 2, 
                    chartRect.Bottom + 5);
            }
            
            // Title is handled by PDF Title component, not embedded in chart
            
            using var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            return stream.ToArray();
        }
        
        public byte[] GenerateLineChart(List<SalesData> data, string title)
        {
            int width = 500;
            int height = 300;
            
            using var bitmap = new Bitmap(width, height);
            using var graphics = Graphics.FromImage(bitmap);
            
            graphics.Clear(Color.White);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            // Chart area
            Rectangle chartRect = new Rectangle(80, 50, 350, 200);
            
            // Find max value for scaling
            decimal maxValue = data.Max(d => d.Revenue);
            decimal minValue = data.Min(d => d.Revenue);
            
            // Draw axes
            using var axisPen = new Pen(Color.Gray, 1);
            graphics.DrawLine(axisPen, chartRect.Left, chartRect.Bottom, chartRect.Right, chartRect.Bottom); // X-axis
            graphics.DrawLine(axisPen, chartRect.Left, chartRect.Top, chartRect.Left, chartRect.Bottom); // Y-axis
            
            // Calculate points
            Point[] points = new Point[data.Count];
            int xStep = chartRect.Width / (data.Count - 1);
            
            for (int i = 0; i < data.Count; i++)
            {
                int x = chartRect.Left + i * xStep;
                int y = chartRect.Bottom - (int)((data[i].Revenue - minValue) / (maxValue - minValue) * chartRect.Height);
                points[i] = new Point(x, y);
            }
            
            // Draw line
            using var linePen = new Pen(Color.Blue, 3);
            if (points.Length > 1)
            {
                graphics.DrawLines(linePen, points);
            }
            
            // Draw data points
            using var pointBrush = new SolidBrush(Color.Red);
            for (int i = 0; i < points.Length; i++)
            {
                graphics.FillEllipse(pointBrush, points[i].X - 4, points[i].Y - 4, 8, 8);
                
                // Draw value labels
                using var font = new Font("Arial", 8);
                using var textBrush = new SolidBrush(Color.Black);
                string valueText = $"${data[i].Revenue:N0}";
                SizeF textSize = graphics.MeasureString(valueText, font);
                graphics.DrawString(valueText, font, textBrush, 
                    points[i].X - textSize.Width / 2, points[i].Y - 20);
                
                // Draw month labels
                graphics.DrawString(data[i].Month, font, textBrush, 
                    points[i].X - graphics.MeasureString(data[i].Month, font).Width / 2, 
                    chartRect.Bottom + 5);
            }
            
            // Title is handled by PDF Title component, not embedded in chart
            
            // Draw Y-axis labels
            using var labelFont = new Font("Arial", 8);
            using var labelBrush = new SolidBrush(Color.Black);
            for (int i = 0; i <= 5; i++)
            {
                decimal value = minValue + (maxValue - minValue) * i / 5;
                int y = chartRect.Bottom - (int)(chartRect.Height * i / 5);
                string labelText = $"${value:N0}";
                graphics.DrawString(labelText, labelFont, labelBrush, new PointF(10, y - 6));
                
                // Draw grid lines
                using var gridPen = new Pen(Color.LightGray, 1);
                graphics.DrawLine(gridPen, chartRect.Left, y, chartRect.Right, y);
            }
            
            using var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            return stream.ToArray();
        }
    }
}
