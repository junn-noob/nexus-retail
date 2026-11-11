using System.Data;
using Microsoft.Data.SqlClient;

namespace NexusRetail.Core.Api.Data
{
    /// <summary>
    /// Data Access Layer (DAL) thuần sử dụng công nghệ ADO.NET (Microsoft.Data.SqlClient).
    /// Hiện thực hóa truy vấn tham số hóa qua SQL Text, Stored Procedure và SqlDataReader.
    /// </summary>
    public class AdoNetDataProvider : IAdoNetDataProvider
    {
        private readonly string _connectionString;

        public AdoNetDataProvider(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("RetailDb")
                ?? configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NexusRetailDb;Integrated Security=True;TrustServerCertificate=True";
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        private static SqlCommand CreateCommand(
            SqlConnection conn,
            string commandText,
            CommandType commandType,
            SqlParameter[]? parameters)
        {
            var cmd = new SqlCommand(commandText, conn)
            {
                CommandType = commandType
            };

            if (parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }

            return cmd;
        }

        #region 1. TRUY VẤN TRỰC TIẾP TRÊN C# (SQL TEXT)
        /// <summary>
        /// Thực thi câu lệnh SQL Text trực tiếp trả về bảng dữ liệu DataTable sử dụng SqlDataAdapter.
        /// </summary>
        public DataTable ExecuteQueryText(string sqlQuery, SqlParameter[]? parameters = null)
        {
            var dt = new DataTable();
            using var conn = CreateConnection();
            using var cmd = CreateCommand(conn, sqlQuery, CommandType.Text, parameters);
            using var adapter = new SqlDataAdapter(cmd);
            conn.Open();
            adapter.Fill(dt);
            return dt;
        }

        /// <summary>
        /// Thực thi lệnh Thêm, Sửa, Xóa (CRUD) trực tiếp bằng SQL Text với tham số hóa chống SQL Injection.
        /// </summary>
        public int ExecuteNonQueryText(string sqlCommand, SqlParameter[]? parameters = null)
        {
            using var conn = CreateConnection();
            using var cmd = CreateCommand(conn, sqlCommand, CommandType.Text, parameters);
            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Thực thi truy vấn trả về một giá trị đơn (Scalar) bằng SQL Text.
        /// </summary>
        public object? ExecuteScalarText(string sqlQuery, SqlParameter[]? parameters = null)
        {
            using var conn = CreateConnection();
            using var cmd = CreateCommand(conn, sqlQuery, CommandType.Text, parameters);
            conn.Open();
            return cmd.ExecuteScalar();
        }

        /// <summary>
        /// Đọc dữ liệu trực tiếp dạng luồng tiến một chiều bằng SqlDataReader.
        /// </summary>
        public List<T> ExecuteReaderText<T>(string sqlQuery, Func<SqlDataReader, T> mapper, SqlParameter[]? parameters = null)
        {
            var list = new List<T>();
            using var conn = CreateConnection();
            using var cmd = CreateCommand(conn, sqlQuery, CommandType.Text, parameters);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(mapper(reader));
            }
            return list;
        }
        #endregion

        #region 2. TRUY VẤN BẰNG THỦ TỤC LƯU TRỮ (STORED PROCEDURE)
        /// <summary>
        /// Thực thi Stored Procedure trả về bảng dữ liệu DataTable.
        /// </summary>
        public DataTable ExecuteStoredProcedure(string spName, SqlParameter[]? parameters = null)
        {
            var dt = new DataTable();
            using var conn = CreateConnection();
            using var cmd = CreateCommand(conn, spName, CommandType.StoredProcedure, parameters);
            using var adapter = new SqlDataAdapter(cmd);
            conn.Open();
            adapter.Fill(dt);
            return dt;
        }

        /// <summary>
        /// Gọi Stored Procedure: Top sản phẩm bán chạy nhất theo tháng/năm.
        /// </summary>
        public DataTable SpBaoCaoSanPhamBanChay(int thang, int nam)
        {
            return ExecuteStoredProcedure("sp_BaoCao_SP_BanChayNhat_Thang", new[]
            {
                new SqlParameter("@Thang", SqlDbType.Int) { Value = thang },
                new SqlParameter("@Nam", SqlDbType.Int) { Value = nam }
            });
        }

        /// <summary>
        /// Gọi Stored Procedure: Hóa đơn nhập lớn nhất theo quý/năm.
        /// </summary>
        public DataTable SpBaoCaoHdnLonNhatQuy(int quy, int nam)
        {
            return ExecuteStoredProcedure("sp_BaoCao_HDNhap_LonNhat_Quy", new[]
            {
                new SqlParameter("@Quy", SqlDbType.Int) { Value = quy },
                new SqlParameter("@Nam", SqlDbType.Int) { Value = nam }
            });
        }

        /// <summary>
        /// Gọi Stored Procedure: Báo cáo tổng kết doanh số năm.
        /// </summary>
        public DataTable SpBaoCaoDoanhSoNam(int nam)
        {
            return ExecuteStoredProcedure("sp_BaoCao_DoanhSo_Nam", new[]
            {
                new SqlParameter("@Nam", SqlDbType.Int) { Value = nam }
            });
        }

        /// <summary>
        /// Gọi Stored Procedure: Top 5 nhân viên doanh số thấp nhất trong tháng.
        /// </summary>
        public DataTable SpBaoCao5NvBanItNhat(int thang, int nam)
        {
            return ExecuteStoredProcedure("sp_BaoCao_5NV_BanItNhat_Thang", new[]
            {
                new SqlParameter("@Thang", SqlDbType.Int) { Value = thang },
                new SqlParameter("@Nam", SqlDbType.Int) { Value = nam }
            });
        }
        #endregion
    }
}
