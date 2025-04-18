﻿
using Microsoft.JSInterop;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data;
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








  public async Task ExportDataAsPdf<T>(IEnumerable<T> data, string fileName, Dictionary<string, string> columnMappings)
{
    try
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var mappedProperties = properties.Where(p => columnMappings.ContainsKey(p.Name)).ToArray();


            DataTable dataTable = new DataTable();

            // Add columns to the DataTable using columnMappings
            foreach (var mapping in columnMappings)
                {
                dataTable.Columns.Add(mapping.Value); // Using mapped column names
                }

            // Add rows by fetching values using property names from columnMappings
            foreach (var item in data)
                {
                DataRow row = dataTable.NewRow();
                foreach (var mapping in columnMappings)
                    {
                    var property = typeof(T).GetProperty(mapping.Key);
                    if (property != null)
                        {
                        row[mapping.Value] = property.GetValue(item) ?? DBNull.Value;
                        }
                    }
                dataTable.Rows.Add(row);
                }

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A3);
                    page.Margin(20);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Content().Table(table =>
                    {
                        // Define columns based on DataTable columns
                        table.ColumnsDefinition(columns =>
                        {
                            foreach (DataColumn column in dataTable.Columns)
                                {
                                columns.RelativeColumn();
                                }
                        });
                        
                        // Add header row
                        table.Header(header =>
                        {
                            foreach (DataColumn column in dataTable.Columns)
                                {
                                header.Cell()
                                .Border(1)
                                .Background("#f59c1a")
                                .BorderColor(Colors.Grey.Lighten1)
                                 .Padding(5)
                                 .Text(column.ColumnName)
                                 .FontColor(Colors.White)
                                 .Bold();
                                }
                        });

                        // Add data rows
                        foreach (DataRow row in dataTable.Rows)
                            {
                            foreach (var cell in row.ItemArray)
                                {
                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten1).Padding(5).Text(cell?.ToString() ?? string.Empty);
                                }
                            }
                    });
                });
            });
            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            var base64 = Convert.ToBase64String(stream.ToArray());
       



            // Blazor JS interop for downloading file
           await _jsRuntime.InvokeVoidAsync("saveAsFile", fileName, base64);
            }
    catch (Exception ex)
    {
        Console.WriteLine($"Error exporting PDF: {ex.Message}");
    }
}


    }


