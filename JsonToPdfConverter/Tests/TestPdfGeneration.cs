using System;
using System.IO;
using JsonToPdfConverter.Services;
using JsonToPdfConverter.Models;
using System.Collections.Generic;

namespace JsonToPdfConverter.Tests
{
    public class TestPdfGeneration
    {
        public static void RunTest()
        {
            Console.WriteLine("Running PDF generation test...");

            // Create test data
            var testData = new DataContext
            {
                ReportTitle = "Test Report",
                CompanyName = "Test Company",
                Summary = "This is a test summary for the PDF generation.",
                SalesActivity = new List<SalesData>
                {
                    new SalesData { Month = "Jan", Revenue = 1000000, UnitsSold = 500 },
                    new SalesData { Month = "Feb", Revenue = 1200000, UnitsSold = 600 },
                    new SalesData { Month = "Mar", Revenue = 1100000, UnitsSold = 550 }
                }
            };

            // Test chart generation
            var chartGenerator = new ChartGenerator();
            byte[] chartBytes = chartGenerator.GeneratePieChart(testData.SalesActivity, "Test Chart", "Month", "Revenue");
            
            File.WriteAllBytes("test-chart.png", chartBytes);
            Console.WriteLine($"Chart generated: {chartBytes.Length} bytes");

            Console.WriteLine("Test completed successfully!");
        }
    }
}
