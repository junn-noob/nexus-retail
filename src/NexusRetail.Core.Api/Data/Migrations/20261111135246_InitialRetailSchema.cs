using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusRetail.Core.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialRetailSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    MaKhach = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenKhach = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHang", x => x.MaKhach);
                });

            migrationBuilder.CreateTable(
                name: "Loai",
                columns: table => new
                {
                    MaLoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenLoai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loai", x => x.MaLoai);
                });

            migrationBuilder.CreateTable(
                name: "ManHinh",
                columns: table => new
                {
                    MaManHinh = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenManHinh = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManHinh", x => x.MaManHinh);
                });

            migrationBuilder.CreateTable(
                name: "NhaCungCap",
                columns: table => new
                {
                    MaNCC = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenNCC = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaCungCap", x => x.MaNCC);
                });

            migrationBuilder.CreateTable(
                name: "NhanHieu",
                columns: table => new
                {
                    MaNhanHieu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenNhanHieu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanHieu", x => x.MaNhanHieu);
                });

            migrationBuilder.CreateTable(
                name: "Que",
                columns: table => new
                {
                    MaQue = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenQue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Que", x => x.MaQue);
                });

            migrationBuilder.CreateTable(
                name: "SanPham",
                columns: table => new
                {
                    MaSP = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenSP = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MaLoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaNhanHieu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaManHinh = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GiaNhap = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GiaBan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    ThoiGianBaoHanh = table.Column<int>(type: "int", nullable: false, defaultValue: 12),
                    AmThanh = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChupAnh = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Anh = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPham", x => x.MaSP);
                    table.ForeignKey(
                        name: "FK_SanPham_Loai",
                        column: x => x.MaLoai,
                        principalTable: "Loai",
                        principalColumn: "MaLoai");
                    table.ForeignKey(
                        name: "FK_SanPham_ManHinh",
                        column: x => x.MaManHinh,
                        principalTable: "ManHinh",
                        principalColumn: "MaManHinh");
                    table.ForeignKey(
                        name: "FK_SanPham_NhanHieu",
                        column: x => x.MaNhanHieu,
                        principalTable: "NhanHieu",
                        principalColumn: "MaNhanHieu");
                });

            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    MaNV = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenNV = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    NgaySinh = table.Column<DateOnly>(type: "date", nullable: true),
                    DienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MaQue = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVien", x => x.MaNV);
                    table.ForeignKey(
                        name: "FK_NhanVien_Que",
                        column: x => x.MaQue,
                        principalTable: "Que",
                        principalColumn: "MaQue",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Chatbot_TuVanLog",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NhuCauKhach = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NganSachToiDa = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaSP_GoiY = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PhanHoiChatbot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiGian = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chatbot_TuVanLog", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_Chatbot_SanPham",
                        column: x => x.MaSP_GoiY,
                        principalTable: "SanPham",
                        principalColumn: "MaSP",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HoaDonBan",
                columns: table => new
                {
                    MaHDB = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaNV = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayBan = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    MaKhach = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDonBan", x => x.MaHDB);
                    table.ForeignKey(
                        name: "FK_HoaDonBan_KhachHang",
                        column: x => x.MaKhach,
                        principalTable: "KhachHang",
                        principalColumn: "MaKhach");
                    table.ForeignKey(
                        name: "FK_HoaDonBan_NhanVien",
                        column: x => x.MaNV,
                        principalTable: "NhanVien",
                        principalColumn: "MaNV");
                });

            migrationBuilder.CreateTable(
                name: "HoaDonNhap",
                columns: table => new
                {
                    MaHDN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaNV = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayNhap = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    MaNCC = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDonNhap", x => x.MaHDN);
                    table.ForeignKey(
                        name: "FK_HoaDonNhap_NhaCungCap",
                        column: x => x.MaNCC,
                        principalTable: "NhaCungCap",
                        principalColumn: "MaNCC");
                    table.ForeignKey(
                        name: "FK_HoaDonNhap_NhanVien",
                        column: x => x.MaNV,
                        principalTable: "NhanVien",
                        principalColumn: "MaNV");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietHDB",
                columns: table => new
                {
                    MaHDB = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaSP = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    KhuyenMai = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietHDB", x => new { x.MaHDB, x.MaSP });
                    table.ForeignKey(
                        name: "FK_ChiTietHDB_HoaDonBan",
                        column: x => x.MaHDB,
                        principalTable: "HoaDonBan",
                        principalColumn: "MaHDB",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietHDB_SanPham",
                        column: x => x.MaSP,
                        principalTable: "SanPham",
                        principalColumn: "MaSP");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietHDN",
                columns: table => new
                {
                    MaHDN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaSP = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    KhuyenMai = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietHDN", x => new { x.MaHDN, x.MaSP });
                    table.ForeignKey(
                        name: "FK_ChiTietHDN_HoaDonNhap",
                        column: x => x.MaHDN,
                        principalTable: "HoaDonNhap",
                        principalColumn: "MaHDN",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietHDN_SanPham",
                        column: x => x.MaSP,
                        principalTable: "SanPham",
                        principalColumn: "MaSP");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chatbot_TuVanLog_MaSP_GoiY",
                table: "Chatbot_TuVanLog",
                column: "MaSP_GoiY");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHDB_MaSP",
                table: "ChiTietHDB",
                column: "MaSP");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHDN_MaSP",
                table: "ChiTietHDN",
                column: "MaSP");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonBan_MaKhach",
                table: "HoaDonBan",
                column: "MaKhach");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonBan_MaNV",
                table: "HoaDonBan",
                column: "MaNV");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonNhap_MaNCC",
                table: "HoaDonNhap",
                column: "MaNCC");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonNhap_MaNV",
                table: "HoaDonNhap",
                column: "MaNV");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_MaQue",
                table: "NhanVien",
                column: "MaQue");

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_MaLoai",
                table: "SanPham",
                column: "MaLoai");

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_MaManHinh",
                table: "SanPham",
                column: "MaManHinh");

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_MaNhanHieu",
                table: "SanPham",
                column: "MaNhanHieu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chatbot_TuVanLog");

            migrationBuilder.DropTable(
                name: "ChiTietHDB");

            migrationBuilder.DropTable(
                name: "ChiTietHDN");

            migrationBuilder.DropTable(
                name: "HoaDonBan");

            migrationBuilder.DropTable(
                name: "HoaDonNhap");

            migrationBuilder.DropTable(
                name: "SanPham");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "NhaCungCap");

            migrationBuilder.DropTable(
                name: "NhanVien");

            migrationBuilder.DropTable(
                name: "Loai");

            migrationBuilder.DropTable(
                name: "ManHinh");

            migrationBuilder.DropTable(
                name: "NhanHieu");

            migrationBuilder.DropTable(
                name: "Que");
        }
    }
}
