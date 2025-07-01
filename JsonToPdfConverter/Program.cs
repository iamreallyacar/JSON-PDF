using System;
using System.IO;
using JsonToPdfConverter.Services;

namespace JsonToPdfConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("JSON to PDF Converter - Complete Report with All Chart Types");
                Console.WriteLine("=============================================================");

                // Get current directory
                string currentDirectory = Directory.GetCurrentDirectory();
                
                // File paths
                string reportJsonPath = Path.Combine(currentDirectory, "sample-report.json");
                string dataJsonPath = Path.Combine(currentDirectory, "sample-data.json");
                string outputPdfPath = Path.Combine(currentDirectory, "complete-report.pdf");

                // Check if input files exist
                if (!File.Exists(reportJsonPath))
                {
                    Console.WriteLine($"Report JSON file not found: {reportJsonPath}");
                    return;
                }

                if (!File.Exists(dataJsonPath))
                {
                    Console.WriteLine($"Data JSON file not found: {dataJsonPath}");
                    return;
                }

                // Read JSON files
                string reportJson = File.ReadAllText(reportJsonPath);
                string dataJson = File.ReadAllText(dataJsonPath);

                Console.WriteLine($"\nReading report definition from: sample-report.json");
                Console.WriteLine($"Reading data from: sample-data.json");

                // Generate PDF
                var pdfRenderer = new PdfRenderer();
                pdfRenderer.GeneratePdf(reportJson, dataJson, outputPdfPath);

                Console.WriteLine($"\n✅ Complete PDF generated successfully: complete-report.pdf");
                Console.WriteLine("📊 Report includes:");
                Console.WriteLine("   • Business summary and analysis");
                Console.WriteLine("   • Product features and achievements");
                Console.WriteLine("   • Customer success metrics");
                Console.WriteLine("   • Sales data table");
                Console.WriteLine("   • Pie Chart - Revenue distribution");
                Console.WriteLine("   • Bar Chart - Revenue comparison");
                Console.WriteLine("   • Line Chart - Revenue trend");
                
                // Try to open the PDF file
                if (File.Exists(outputPdfPath))
                {
                    Console.WriteLine("\n🚀 Opening PDF file...");
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(outputPdfPath) 
                    { 
                        UseShellExecute = true 
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
