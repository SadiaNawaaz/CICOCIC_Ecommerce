
using Microsoft.JSInterop;
using OfficeOpenXml;
using System.Reflection;
using System.Text;

namespace Ecommerce.Admin.Services;



public class CsvExportService
    {
    private readonly IJSRuntime _jsRuntime;

    public CsvExportService(IJSRuntime jsRuntime)
        {
        _jsRuntime = jsRuntime;
        }

    public async Task ExportDataAsCsv<T>(IEnumerable<T> data, string fileName, Dictionary<string, string> columnMappings)
        {
        var csvData = GenerateCsv(data, columnMappings);
        var bytes = Encoding.UTF8.GetBytes(csvData);

        await _jsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(bytes));
        }

    private string GenerateCsv<T>(IEnumerable<T> data, Dictionary<string, string> columnMappings)
        {
        if (data == null || !data.Any())
            {
            return string.Empty;
            }

        var csv = new StringBuilder();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // Filter only the properties that are defined in the columnMappings
        var mappedProperties = properties.Where(p => columnMappings.ContainsKey(p.Name)).ToArray();

        // Add header row with user-friendly names
        csv.AppendLine(string.Join(",", mappedProperties.Select(p => columnMappings[p.Name])));

        // Add data rows
        foreach (var item in data)
            {
            var values = mappedProperties.Select(p =>
            {
                var value = p.GetValue(item, null);
                return value != null ? EscapeCsv(value.ToString()) : string.Empty;
            });
            csv.AppendLine(string.Join(",", values));
            }

        return csv.ToString();
        }

    private string EscapeCsv(string value)
        {
        if (value.Contains(",") || value.Contains("\""))
            {
            value = $"\"{value.Replace("\"", "\"\"")}\"";
            }
        return value;
        }


    public async Task ExportDataAsExcel<T>(IEnumerable<T> data, string fileName, Dictionary<string, string> columnMappings)
        {
        try
            {
            using (var package = new ExcelPackage())
                {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                // Filter only the properties that are defined in the columnMappings
                var mappedProperties = properties.Where(p => columnMappings.ContainsKey(p.Name)).ToArray();

                // Add headers with user-friendly names
                for (int i = 0; i < mappedProperties.Length; i++)
                    {
                    worksheet.Cells[1, i + 1].Value = columnMappings[mappedProperties[i].Name];
                    }

                // Add data rows
                int row = 2;
                foreach (var item in data)
                    {
                    for (int col = 0; col < mappedProperties.Length; col++)
                        {
                        var value = mappedProperties[col].GetValue(item, null);
                        worksheet.Cells[row, col + 1].Value = value?.ToString() ?? string.Empty;
                        }
                    row++;
                    }

                // Trigger download
                var bytes = package.GetAsByteArray();
                var base64 = Convert.ToBase64String(bytes);
                await _jsRuntime.InvokeVoidAsync("saveAsFile", fileName, base64);
                }
            }
        catch (Exception ex)
            {
            // Handle exception or log it
            Console.WriteLine($"Error exporting Excel: {ex.Message}");
            }
        }

    }


