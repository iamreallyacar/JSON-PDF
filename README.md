# JSON-PDF

A C# console application that converts JSON data into PDF reports with charts and visualizations.

## Features

- Convert JSON data to formatted PDF reports
- Generate various chart types (bar, line, pie, etc.)
- Customizable report layouts and styling
- Support for multiple data sources

## Prerequisites

- .NET 9.0 SDK or later
- Windows OS (required for Windows Forms components)
- Git

## Setup Instructions

### 1. Clone the Repository

```powershell
git pull
```

### 2. Navigate to Project Directory

```powershell
cd JsonToPdfConverter
```

### 3. Restore Dependencies

```powershell
dotnet restore
```

### 4. Build the Project

```powershell
dotnet build
```

### 5. Run the Application

```powershell
dotnet run
```

## Usage

The application expects the following JSON files in the project directory:

- `sample-report.json` - Report configuration and layout
- `sample-data.json` - Data to be visualized
- `sample-charts-showcase.json` - Chart configurations

After running the application, it will generate:

- `complete-report.pdf` - The final PDF report with charts

## Project Structure

- `Models/` - Data models and report structure definitions
- `Services/` - Core services for PDF generation and chart creation
- `Tests/` - Unit tests for PDF generation functionality

## Dependencies

- **iTextSharp** (5.5.13.3) - PDF generation library
- **Newtonsoft.Json** (13.0.3) - JSON parsing and serialization
- **System.Drawing.Common** (9.0.6) - Chart and graphics generation
