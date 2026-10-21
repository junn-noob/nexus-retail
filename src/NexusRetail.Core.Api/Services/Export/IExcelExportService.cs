namespace NexusRetail.Core.Api.Services.Export
{
    public interface IExcelExportService
    {
        byte[] GenerateExcelReport(string tieuDe, string[] cotTieuDe, List<object[]> duLieuHang, string sheetName = "BaoCao");
    }
}
