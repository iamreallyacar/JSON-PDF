using System;
using System.IO;
using JsonToPdfConverter.Services;
using JsonToPdfConverter.Models;
using System.Collections.Generic;

namespace JsonToPdfConverter
{
    public class ChartTest
    {
        public static void TestAllCharts()
        {
            Console.WriteLine("🧪 Testing Chart Generation...");
            
            var testData = new List<SalesData>
            {
                new SalesData { Month = "Jan", Revenue = 1000000, UnitsSold = 500 },
                new SalesData { Month = "Feb", Revenue = 2000000, UnitsSold = 800 },
                new SalesData { Month = "Mar", Revenue = 3000000, UnitsSold = 1200 }
            };

            var chartGenerator = new ChartGenerator();
            
            try
            {
                // Test Pie Chart
                Console.WriteLine("📊 Generating Pie Chart...");
                byte[] pieChartBytes = chartGenerator.GeneratePieChart(testData, "Test Pie Chart", "Month", "Revenue");
                File.WriteAllBytes("test-pie-chart.png", pieChartBytes);
                Console.WriteLine($"✅ Pie Chart: {pieChartBytes.Length} bytes");

                // Test Bar Chart
                Console.WriteLine("📊 Generating Bar Chart...");
                byte[] barChartBytes = chartGenerator.GenerateBarChart(testData, "Test Bar Chart");
                File.WriteAllBytes("test-bar-chart.png", barChartBytes);
                Console.WriteLine($"✅ Bar Chart: {barChartBytes.Length} bytes");

                // Test Line Chart
                Console.WriteLine("📊 Generating Line Chart...");
                byte[] lineChartBytes = chartGenerator.GenerateLineChart(testData, "Test Line Chart");
                File.WriteAllBytes("test-line-chart.png", lineChartBytes);
                Console.WriteLine($"✅ Line Chart: {lineChartBytes.Length} bytes");

                Console.WriteLine("\n🎉 All chart types generated successfully!");
                Console.WriteLine("Check the following files:");
                Console.WriteLine("   • test-pie-chart.png");
                Console.WriteLine("   • test-bar-chart.png");
                Console.WriteLine("   • test-line-chart.png");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error generating charts: {ex.Message}");
            }
        }
    }
}
