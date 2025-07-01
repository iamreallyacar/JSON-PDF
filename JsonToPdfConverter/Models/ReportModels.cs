namespace JsonToPdfConverter.Models
{
    public class Report
    {
        public int Id { get; set; }
        public List<Component> Header { get; set; } = new();
        public List<Component> Layout { get; set; } = new();
        public List<Component> Footer { get; set; } = new();
    }

    public class Component
    {
        public string Renderer { get; set; } = "";
        public string? DataSource { get; set; }
        public ComponentOptions Options { get; set; } = new();
    }

    public class ComponentOptions
    {
        public int? FontSize { get; set; }
        public bool? Bold { get; set; }
        public string? Color { get; set; }
        public string? Text { get; set; }
        public int? Height { get; set; }
        public bool? ShowLine { get; set; }
        public int? Width { get; set; }
        public string? Alignment { get; set; }
        
        // Row components
        public List<int>? ColumnWidths { get; set; }
        public List<Component>? Components { get; set; }
        
        // DataTable components
        public List<TableColumn>? Columns { get; set; }
        
        // Chart components
        public string? Title { get; set; }
        public string? LabelSource { get; set; }
        public string? ValueSource { get; set; }
    }

    public class TableColumn
    {
        public string Header { get; set; } = "";
        public string Source { get; set; } = "";
    }

    public class DataContext
    {
        public string ReportTitle { get; set; } = "";
        public string CompanyLogo { get; set; } = "";
        public string CompanyName { get; set; } = "";
        public string Summary { get; set; } = "";
        public string ExecutiveSummary { get; set; } = "";
        public string MarketAnalysis { get; set; } = "";
        public string FinancialHighlights { get; set; } = "";
        public List<string> ProductFeatures { get; set; } = new();
        public List<string> TechnicalAchievements { get; set; } = new();
        public List<string> CustomerSuccess { get; set; } = new();
        public string FutureOutlook { get; set; } = "";
        public List<SalesData> SalesActivity { get; set; } = new();
    }

    public class SalesData
    {
        public string Month { get; set; } = "";
        public decimal Revenue { get; set; }
        public int UnitsSold { get; set; }
    }
}
