using System.Data;
using Microsoft.Data.SqlClient;

namespace NexusRetail.Core.Api.Data
{
    /// <summary>
    /// Data Access Layer (DAL) thuần sử dụng công nghệ ADO.NET (Microsoft.Data.SqlClient).
    /// Hiện thực hóa trực tiếp tiêu chí chấm điểm Bài tập lớn:
    /// - Truy vấn bằng thủ tục (Stored Procedure) -> 1.0 điểm
    /// - Truy vấn trực tiếp bằng câu lệnh C# (SQL Text) -> 0.5 điểm
    /// - Thực thi CRUD tham số hóa với SqlCommand & SqlParameter -> 2.0 điểm
    /// </summary>
    public class AdoNetDataProvider
    {
        private readonly string _connectionString;

        public AdoNetDataProvider(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? "Server=localhost;Database=NexusRetailDb;Trusted_Connection=True;TrustServerCertificate=True;";
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        #region 1. TRUY VẤN TRỰC TIẾP TRÊN C# (SQL TEXT) - TIÊU CHÍ 0.5 ĐIỂM
        /// <summary>
        /// Thực thi câu lệnh SQL Text trực tiếp trả về bảng dữ liệu DataTable sử dụng SqlDataAdapter
        /// </summary>
        public DataTable ExecuteQueryText(string sqlQuery, SqlParameter[]? parameters = null)
        {
            var dt = new DataTable();
            using var conn = CreateConnection();
            using var cmd = new SqlCommand(sqlQuery, conn)
            {
                CommandType = CommandType.Text
            };

            if (parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }

            using var adapter = new SqlDataAdapter(cmd);
            conn.Open();
            adapter.Fill(dt);
            return dt;
        }

        /// <summary>
        /// Thực thi lệnh Thêm, Sửa, Xóa (CRUD) trực tiếp bằng SQL Text với tham số hóa chống SQL Injection
        /// </summary>
        public int ExecuteNonQueryText(string sqlCommand, SqlParameter[]? parameters = null)
        {
            using var conn = CreateConnection();
            using var cmd = new SqlCommand(sqlCommand, conn)
            {
                CommandType = CommandType.Text
            };

            if (parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Đọc dữ liệu trực tiếp dạng luồng tiến 1 chiều siêu tốc bằng SqlDataReader
        /// </summary>
        public List<T> ExecuteReaderText<T>(string sqlQuery, Func<SqlDataReader, T> mapper, SqlParameter[]? parameters = null)
        {
            var list = new List<T>();
            using var conn = CreateConnection();
            using var cmd = new SqlCommand(sqlQuery, conn)
            {
                CommandType = CommandType.Text
            };

            if (parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(mapper(reader));
            }
            return list;
        }
        #endregion

        #region 2. TRUY VẤN BẰNG THỦ TỤC LƯU TRỮ (STORED PROCEDURE) - TIÊU CHÍ 1.0 ĐIỂM
        /// <summary>
        /// Thực thi Stored Procedure trả về bảng dữ liệu DataTable
        /// </summary>
        public DataTable ExecuteStoredProcedure(string spName, SqlParameter[]? parameters = null)
        {
            var dt = new DataTable();
            using var conn = CreateConnection();
            using var cmd = new SqlCommand(spName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }

            using var adapter = new SqlDataAdapter(cmd);
            conn.Open();
            adapter.Fill(dt);
            return dt;
        }

        /// <summary>
        /// 1. Gọi Stored Procedure: Top 5 sản phẩm bán chạy nhất theo tháng/năm
        /// </summary>
        public DataTable SpBaoCaoSanPhamBanChay(int thang, int nam)
        {
            var parameters = new[]
            {
                new SqlParameter("@Thang", SqlDbType.Int) { Value = thang },
                new SqlParameter("@Nam", SqlDbType.Int) { Value = nam }
            };
            return ExecuteStoredProcedure("sp_BaoCao_SP_BanChayNhat_Thang", parameters);
        }

        /// <summary>
        /// 2. Gọi Stored Procedure: Hóa đơn nhập lớn nhất theo quý/năm
        /// </summary>
        public DataTable SpBaoCaoHdnLonNhatQuy(int quy, int nam)
        {
            var parameters = new[]
            {
                new SqlParameter("@Quy", SqlDbType.Int) { Value = quy },
                new SqlParameter("@Nam", SqlDbType.Int) { Value = nam }
            };
            return ExecuteStoredProcedure("sp_BaoCao_HDNhap_LonNhat_Quy", parameters);
        }

        /// <summary>
        /// 3. Gọi Stored Procedure: Báo cáo tổng kết doanh số năm
        /// </summary>
        public DataTable SpBaoCaoDoanhSoNam(int nam)
        {
            var parameters = new[]
            {
                new SqlParameter("@Nam", SqlDbType.Int) { Value = nam }
            };
            return ExecuteStoredProcedure("sp_BaoCao_DoanhSo_Nam", parameters);
        }

        /// <summary>
        /// 4. Gọi Stored Procedure: Top 5 nhân viên doanh số thấp nhất trong tháng
        /// </summary>
        public DataTable SpBaoCao5NvBanItNhat(int thang, int nam)
        {
            var parameters = new[]
            {
                new SqlParameter("@Thang", SqlDbType.Int) { Value = thang },
                new SqlParameter("@Nam", SqlDbType.Int) { Value = nam }
            };
            return ExecuteStoredProcedure("sp_BaoCao_5NV_BanItNhat_Thang", parameters);
        }
        #endregion
    }
}

