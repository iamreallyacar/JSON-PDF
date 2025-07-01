using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using JsonToPdfConverter.Models;
using Newtonsoft.Json;
using Font = iTextSharp.text.Font;

namespace JsonToPdfConverter.Services
{
    public class PdfRenderer
    {
        private readonly ChartGenerator _chartGenerator;
        private Document? _document;
        private PdfWriter? _writer;
        private DataContext? _dataContext;

        public PdfRenderer()
        {
            _chartGenerator = new ChartGenerator();
        }

        public void GeneratePdf(string jsonContent, string dataJsonContent, string outputPath)
        {
            var report = JsonConvert.DeserializeObject<Report>(jsonContent);
            _dataContext = JsonConvert.DeserializeObject<DataContext>(dataJsonContent) ?? new DataContext();

            if (report == null) throw new ArgumentException("Invalid report JSON");

            _document = new Document(PageSize.A4, 50, 50, 50, 50);
            _writer = PdfWriter.GetInstance(_document, new FileStream(outputPath, FileMode.Create));
            
            _document.Open();

            try
            {
                // Render header
                if (report.Header?.Any() == true)
                {
                    foreach (var component in report.Header)
                    {
                        RenderComponent(component);
                    }
                }

                // Render main layout
                if (report.Layout?.Any() == true)
                {
                    foreach (var component in report.Layout)
                    {
                        RenderComponent(component);
                    }
                }

                // Footer will be handled by page events if needed
            }
            finally
            {
                _document.Close();
            }
        }

        private void RenderComponent(Component component)
        {
            Console.WriteLine($"Rendering component: {component.Renderer}");
            
            switch (component.Renderer)
            {
                case "Title":
                    RenderTitle(component);
                    break;
                case "Paragraph":
                    RenderParagraph(component);
                    break;
                case "Spacer":
                    RenderSpacer(component);
                    break;
                case "Row":
                    RenderRow(component);
                    break;
                case "BulletedList":
                    RenderBulletedList(component);
                    break;
                case "DataTable":
                    RenderDataTable(component);
                    break;
                case "PieChart":
                    RenderPieChart(component);
                    break;
                case "BarChart":
                    RenderBarChart(component);
                    break;
                case "LineChart":
                    RenderLineChart(component);
                    break;
                case "Image":
                    RenderImage(component);
                    break;
                case "PageNumber":
                    // Page numbers are typically handled in header/footer events
                    break;
                default:
                    Console.WriteLine($"Unknown component type: {component.Renderer}");
                    break;
            }
        }

        private void RenderTitle(Component component)
        {
            if (_document == null) return;
            
            string text = GetTextFromDataSource(component.DataSource) ?? component.Options.Text ?? "";
            
            var font = GetFont(component.Options);
            var paragraph = new Paragraph(text, font);
            
            if (component.Options.Color != null)
            {
                var color = ColorTranslator.FromHtml(component.Options.Color);
                font.Color = new BaseColor(color.R, color.G, color.B);
            }

            _document.Add(paragraph);
        }

        private void RenderParagraph(Component component)
        {
            if (_document == null) return;
            
            string text = GetTextFromDataSource(component.DataSource) ?? component.Options.Text ?? "";
            
            var font = GetFont(component.Options);
            var paragraph = new Paragraph(text, font);
            
            if (component.Options.Color != null)
            {
                var color = ColorTranslator.FromHtml(component.Options.Color);
                font.Color = new BaseColor(color.R, color.G, color.B);
            }

            _document.Add(paragraph);
        }

        private void RenderSpacer(Component component)
        {
            if (_document == null) return;
            
            int height = component.Options.Height ?? 10;
            _document.Add(new Paragraph(Chunk.NEWLINE) { SpacingAfter = height });
            
            if (component.Options.ShowLine == true)
            {
                var line = new LineSeparator(1f, 100f, BaseColor.GRAY, Element.ALIGN_CENTER, -1);
                _document.Add(new Chunk(line));
            }
        }

        private void RenderRow(Component component)
        {
            if (_document == null || component.Options.Components == null || !component.Options.Components.Any())
                return;

            // For now, let's render row components as simple sequential elements
            // instead of complex table layout to avoid PDF generation issues
            foreach (var subComponent in component.Options.Components)
            {
                RenderComponent(subComponent);
            }
        }

        private void RenderBulletedList(Component component)
        {
            if (_document == null) return;
            
            var listData = GetListFromDataSource(component.DataSource);
            
            if (listData?.Any() == true)
            {
                var list = new List(List.UNORDERED);
                var font = GetFont(component.Options);
                
                if (component.Options.Color != null)
                {
                    var color = ColorTranslator.FromHtml(component.Options.Color);
                    font.Color = new BaseColor(color.R, color.G, color.B);
                }

                foreach (var item in listData)
                {
                    list.Add(new ListItem(item, font));
                }

                _document.Add(list);
            }
        }

        private void RenderDataTable(Component component)
        {
            if (_document == null || component.Options.Columns == null || !component.Options.Columns.Any())
                return;

            var tableData = GetTableDataFromDataSource(component.DataSource);
            if (tableData == null || !tableData.Any())
                return;

            var table = new PdfPTable(component.Options.Columns.Count);
            table.WidthPercentage = 100;

            // Add headers
            foreach (var column in component.Options.Columns)
            {
                var headerCell = new PdfPCell(new Phrase(column.Header, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)));
                headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(headerCell);
            }

            // Add data rows
            foreach (var row in tableData)
            {
                foreach (var column in component.Options.Columns)
                {
                    string cellValue = GetPropertyValue(row, column.Source) ?? "";
                    table.AddCell(new PdfPCell(new Phrase(cellValue, FontFactory.GetFont(FontFactory.HELVETICA, 9))));
                }
            }

            _document.Add(table);
        }

        private void RenderPieChart(Component component)
        {
            if (_document == null) return;
            
            var chartData = GetTableDataFromDataSource(component.DataSource);
            if (chartData == null || !chartData.Any())
                return;

            string title = component.Options.Title ?? "Chart";
            string labelSource = component.Options.LabelSource ?? "";
            string valueSource = component.Options.ValueSource ?? "";

            var chartBytes = _chartGenerator.GeneratePieChart(chartData, title, labelSource, valueSource);
            
            var image = iTextSharp.text.Image.GetInstance(chartBytes);
            image.ScalePercent(75f);
            _document.Add(image);
        }

        private void RenderBarChart(Component component)
        {
            if (_document == null) return;
            
            var chartData = GetTableDataFromDataSource(component.DataSource);
            if (chartData == null || !chartData.Any())
                return;

            string title = component.Options.Title ?? "Bar Chart";

            var chartBytes = _chartGenerator.GenerateBarChart(chartData, title);
            
            var image = iTextSharp.text.Image.GetInstance(chartBytes);
            image.ScalePercent(75f);
            _document.Add(image);
        }

        private void RenderLineChart(Component component)
        {
            if (_document == null) return;
            
            var chartData = GetTableDataFromDataSource(component.DataSource);
            if (chartData == null || !chartData.Any())
                return;

            string title = component.Options.Title ?? "Line Chart";

            var chartBytes = _chartGenerator.GenerateLineChart(chartData, title);
            
            var image = iTextSharp.text.Image.GetInstance(chartBytes);
            image.ScalePercent(75f);
            _document.Add(image);
        }

        private void RenderImage(Component component)
        {
            if (_document == null) return;
            
            string imagePath = GetTextFromDataSource(component.DataSource) ?? "";
            
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                var image = iTextSharp.text.Image.GetInstance(imagePath);
                
                if (component.Options.Width.HasValue)
                {
                    image.ScaleToFit(component.Options.Width.Value, image.Height);
                }
                
                _document.Add(image);
            }
        }

        private Font GetFont(ComponentOptions options)
        {
            int fontSize = options.FontSize ?? 12;
            int style = Font.NORMAL;
            
            if (options.Bold == true)
            {
                style = Font.BOLD;
            }

            var font = FontFactory.GetFont(FontFactory.HELVETICA, fontSize, style);
            
            if (options.Color != null)
            {
                var color = ColorTranslator.FromHtml(options.Color);
                font.Color = new BaseColor(color.R, color.G, color.B);
            }

            return font;
        }

        private string? GetTextFromDataSource(string? dataSource)
        {
            if (string.IsNullOrEmpty(dataSource) || _dataContext == null)
                return null;

            return dataSource switch
            {
                "reportTitle" => _dataContext.ReportTitle,
                "companyLogo" => _dataContext.CompanyLogo,
                "companyName" => _dataContext.CompanyName,
                "summary" => _dataContext.Summary,
                "executiveSummary" => _dataContext.ExecutiveSummary,
                "marketAnalysis" => _dataContext.MarketAnalysis,
                "financialHighlights" => _dataContext.FinancialHighlights,
                "futureOutlook" => _dataContext.FutureOutlook,
                _ => null
            };
        }

        private List<string>? GetListFromDataSource(string? dataSource)
        {
            if (string.IsNullOrEmpty(dataSource) || _dataContext == null)
                return null;

            return dataSource switch
            {
                "productFeatures" => _dataContext.ProductFeatures,
                "technicalAchievements" => _dataContext.TechnicalAchievements,
                "customerSuccess" => _dataContext.CustomerSuccess,
                _ => null
            };
        }

        private List<SalesData>? GetTableDataFromDataSource(string? dataSource)
        {
            if (string.IsNullOrEmpty(dataSource) || _dataContext == null)
                return null;

            return dataSource switch
            {
                "salesActivity" => _dataContext.SalesActivity,
                _ => null
            };
        }

        private string? GetPropertyValue(object obj, string propertyName)
        {
            if (obj is SalesData salesData)
            {
                return propertyName switch
                {
                    "Month" => salesData.Month,
                    "Revenue" => salesData.Revenue.ToString("C"),
                    "UnitsSold" => salesData.UnitsSold.ToString(),
                    _ => null
                };
            }
            return null;
        }
    }
}
