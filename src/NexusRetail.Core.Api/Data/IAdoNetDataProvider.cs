using System.Data;
using Microsoft.Data.SqlClient;

namespace NexusRetail.Core.Api.Data;

/// <summary>
/// Hợp đồng truy xuất dữ liệu tầng DAL sử dụng ADO.NET thuần (Microsoft.Data.SqlClient).
/// </summary>
public interface IAdoNetDataProvider
{
    SqlConnection CreateConnection();

    DataTable ExecuteQueryText(string sqlQuery, SqlParameter[]? parameters = null);

    int ExecuteNonQueryText(string sqlCommand, SqlParameter[]? parameters = null);

    object? ExecuteScalarText(string sqlQuery, SqlParameter[]? parameters = null);

    List<T> ExecuteReaderText<T>(string sqlQuery, Func<SqlDataReader, T> mapper, SqlParameter[]? parameters = null);

    DataTable ExecuteStoredProcedure(string spName, SqlParameter[]? parameters = null);

    DataTable SpBaoCaoSanPhamBanChay(int thang, int nam);

    DataTable SpBaoCaoHdnLonNhatQuy(int quy, int nam);

    DataTable SpBaoCaoDoanhSoNam(int nam);

    DataTable SpBaoCao5NvBanItNhat(int thang, int nam);
}

