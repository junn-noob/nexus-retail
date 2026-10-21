using ClosedXML.Excel;

namespace NexusRetail.Core.Api.Services.Export
{
    public class ExcelExportService : IExcelExportService
    {
        public byte[] GenerateExcelReport(string tieuDe, string[] cotTieuDe, List<object[]> duLieuHang, string sheetName = "BaoCao")
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(sheetName);

            // 1. Tiêu đề chính lớn
            worksheet.Cell(1, 1).Value = tieuDe;
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Cell(1, 1).Style.Font.FontColor = XLColor.DarkBlue;
            worksheet.Range(1, 1, 1, Math.Max(cotTieuDe.Length, 1)).Merge();
            worksheet.Row(1).Height = 30;
            worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(1, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // 2. Ngày xuất báo cáo
            worksheet.Cell(2, 1).Value = $"Thời gian xuất báo cáo: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            worksheet.Cell(2, 1).Style.Font.Italic = true;
            worksheet.Range(2, 1, 2, Math.Max(cotTieuDe.Length, 1)).Merge();

            // 3. Header cột
            for (int i = 0; i < cotTieuDe.Length; i++)
            {
                var cell = worksheet.Cell(4, i + 1);
                cell.Value = cotTieuDe[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E3A8A"); // Navy blue
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }
            worksheet.Row(4).Height = 25;

            // 4. Dữ liệu từng dòng
            for (int r = 0; r < duLieuHang.Count; r++)
            {
                var rowNumber = r + 5;
                for (int c = 0; c < cotTieuDe.Length; c++)
                {
                    var cell = worksheet.Cell(rowNumber, c + 1);
                    var val = duLieuHang[r][c];

                    if (val is decimal || val is double || val is int || val is long)
                    {
                        cell.Value = Convert.ToDouble(val);
                        if (cotTieuDe[c].Contains("Tiền") || cotTieuDe[c].Contains("Thu") || cotTieuDe[c].Contains("Giá"))
                        {
                            cell.Style.NumberFormat.Format = "#,##0 ₫";
                        }
                    }
                    else
                    {
                        cell.Value = val?.ToString() ?? "";
                    }

                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.OutsideBorderColor = XLColor.LightGray;
                }
            }

            worksheet.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
