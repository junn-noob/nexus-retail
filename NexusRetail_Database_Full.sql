-- ====================================================================================
-- NEXUS RETAIL POS ENTERPRISE - DATABASE INITIALIZATION SCRIPT
-- SCRIPT CƠ SỞ DỮ LIỆU ĐẦY ĐỦ: TỰ ĐỘNG KHỞI TẠO ĐỂ CHẠY TRÊN MÁY MỚI
-- HỆ THỐNG BAO GỒM 2 CƠ SỞ DỮ LIỆU:
--   1. NexusRetailDb        : 12 bảng nghiệp vụ chuẩn + Chatbot log + Triggers + Views + SPs + Seed Data
--   2. NexusRetailIdentityDb: Quản lý người dùng, phân quyền RBAC (Admin, Staff, Customer) & JWT Tokens
-- TƯƠNG THÍCH HOÀN TOÀN:
--   - SQL Server 2008 / 2012 / 2014 / 2016 / 2017 / 2019 / 2022 / 2025 / LocalDB / Azure SQL
--   - Chuẩn cú pháp T-SQL tương thích ngược 100% (Không lỗi cú pháp CREATE OR ALTER)
--   - Đã xử lý triệt để cảnh báo Clustered Index Key Length (< 900 bytes)
--   - .NET 10.0 (Core API, Identity API, WinForms POS & WebMvc)
-- ====================================================================================

SET NOCOUNT ON;
SET DATEFORMAT ymd;
GO

-- ####################################################################################
-- PHẦN 1: CƠ SỞ DỮ LIỆU NGHIỆP VỤ CỐT LÕI (NexusRetailDb)
-- ####################################################################################

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'NexusRetailDb')
BEGIN
    CREATE DATABASE NexusRetailDb;
    PRINT N'✔ Đã tạo mới cơ sở dữ liệu: NexusRetailDb';
END
ELSE
BEGIN
    PRINT N'✔ Cơ sở dữ liệu NexusRetailDb đã sẵn sàng.';
END
GO

USE NexusRetailDb;
GO

-- ------------------------------------------------------------------------------------
-- XÓA CÁC ĐỐI TƯỢNG CŨ NẾU CẦN KHỞI TẠO LẠI (THỨ TỰ PHỤ THUỘC CHUẨN)
-- ------------------------------------------------------------------------------------
-- 1. Xóa Views trước
IF OBJECT_ID('dbo.vw_ChiTietHoaDonBan', 'V') IS NOT NULL DROP VIEW dbo.vw_ChiTietHoaDonBan;
IF OBJECT_ID('dbo.vw_DanhSachSanPham', 'V') IS NOT NULL DROP VIEW dbo.vw_DanhSachSanPham;
GO

-- 2. Xóa Stored Procedures trước
IF OBJECT_ID('dbo.sp_BaoCao_5NV_BanItNhat_Thang', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_BaoCao_5NV_BanItNhat_Thang;
IF OBJECT_ID('dbo.sp_BaoCao_DoanhSo_Nam', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_BaoCao_DoanhSo_Nam;
IF OBJECT_ID('dbo.sp_BaoCao_HDNhap_LonNhat_Quy', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_BaoCao_HDNhap_LonNhat_Quy;
IF OBJECT_ID('dbo.sp_BaoCao_SP_BanChayNhat_Thang', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_BaoCao_SP_BanChayNhat_Thang;
GO

-- 3. Xóa Bảng theo thứ tự khóa ngoại ngược (Tránh xung đột Foreign Key)
IF OBJECT_ID('dbo.Chatbot_TuVanLog', 'U') IS NOT NULL DROP TABLE dbo.Chatbot_TuVanLog;
IF OBJECT_ID('dbo.ChiTietHDB', 'U') IS NOT NULL DROP TABLE dbo.ChiTietHDB;
IF OBJECT_ID('dbo.HoaDonBan', 'U') IS NOT NULL DROP TABLE dbo.HoaDonBan;
IF OBJECT_ID('dbo.ChiTietHDN', 'U') IS NOT NULL DROP TABLE dbo.ChiTietHDN;
IF OBJECT_ID('dbo.HoaDonNhap', 'U') IS NOT NULL DROP TABLE dbo.HoaDonNhap;
IF OBJECT_ID('dbo.SanPham', 'U') IS NOT NULL DROP TABLE dbo.SanPham;
IF OBJECT_ID('dbo.NhaCungCap', 'U') IS NOT NULL DROP TABLE dbo.NhaCungCap;
IF OBJECT_ID('dbo.KhachHang', 'U') IS NOT NULL DROP TABLE dbo.KhachHang;
IF OBJECT_ID('dbo.NhanVien', 'U') IS NOT NULL DROP TABLE dbo.NhanVien;
IF OBJECT_ID('dbo.Que', 'U') IS NOT NULL DROP TABLE dbo.Que;
IF OBJECT_ID('dbo.ManHinh', 'U') IS NOT NULL DROP TABLE dbo.ManHinh;
IF OBJECT_ID('dbo.NhanHieu', 'U') IS NOT NULL DROP TABLE dbo.NhanHieu;
IF OBJECT_ID('dbo.Loai', 'U') IS NOT NULL DROP TABLE dbo.Loai;
GO

-- ------------------------------------------------------------------------------------
-- 1. BẢNG LOẠI (Phân loại: Flagship, Cận cao cấp, Tầm trung, Phổ thông, Gaming, Gập)
-- ------------------------------------------------------------------------------------
CREATE TABLE Loai (
    MaLoai NVARCHAR(20) NOT NULL,
    TenLoai NVARCHAR(100) NOT NULL,
    CONSTRAINT PK_Loai PRIMARY KEY (MaLoai)
);
GO

-- ------------------------------------------------------------------------------------
-- 2. BẢNG NHÃN HIỆU (Apple, Samsung, Xiaomi, OPPO, Vivo...)
-- ------------------------------------------------------------------------------------
CREATE TABLE NhanHieu (
    MaNhanHieu NVARCHAR(20) NOT NULL,
    TenNhanHieu NVARCHAR(100) NOT NULL,
    CONSTRAINT PK_NhanHieu PRIMARY KEY (MaNhanHieu)
);
GO

-- ------------------------------------------------------------------------------------
-- 3. BẢNG MÀN HÌNH (Công nghệ màn hình: OLED, Dynamic AMOLED, LTPO...)
-- ------------------------------------------------------------------------------------
CREATE TABLE ManHinh (
    MaManHinh NVARCHAR(20) NOT NULL,
    TenManHinh NVARCHAR(100) NOT NULL,
    CONSTRAINT PK_ManHinh PRIMARY KEY (MaManHinh)
);
GO

-- ------------------------------------------------------------------------------------
-- 4. BẢNG QUÊ (Quê quán / Tỉnh thành)
-- ------------------------------------------------------------------------------------
CREATE TABLE Que (
    MaQue NVARCHAR(20) NOT NULL,
    TenQue NVARCHAR(100) NOT NULL,
    CONSTRAINT PK_Que PRIMARY KEY (MaQue)
);
GO

-- ------------------------------------------------------------------------------------
-- 5. BẢNG NHÂN VIÊN (Nhân sự bán hàng, thu ngân, quản trị kho)
-- ------------------------------------------------------------------------------------
CREATE TABLE NhanVien (
    MaNV NVARCHAR(20) NOT NULL,
    TenNV NVARCHAR(100) NOT NULL,
    GioiTinh NVARCHAR(10) NULL,
    NgaySinh DATE NULL,
    DienThoai NVARCHAR(20) NULL,
    DiaChi NVARCHAR(200) NULL,
    MaQue NVARCHAR(20) NULL,
    CONSTRAINT PK_NhanVien PRIMARY KEY (MaNV),
    CONSTRAINT FK_NhanVien_Que FOREIGN KEY (MaQue) REFERENCES Que(MaQue) ON UPDATE CASCADE ON DELETE SET NULL
);
GO

-- ------------------------------------------------------------------------------------
-- 6. BẢNG KHÁCH HÀNG (Tra cứu bảo hành và lịch sử hóa đơn qua Số điện thoại)
-- ------------------------------------------------------------------------------------
CREATE TABLE KhachHang (
    MaKhach NVARCHAR(20) NOT NULL,
    TenKhach NVARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(200) NULL,
    DienThoai NVARCHAR(20) NOT NULL,
    CONSTRAINT PK_KhachHang PRIMARY KEY (MaKhach)
);
GO

-- ------------------------------------------------------------------------------------
-- 7. BẢNG NHÀ CUNG CẤP (Đơn vị phân phối thiết bị chính hãng)
-- ------------------------------------------------------------------------------------
CREATE TABLE NhaCungCap (
    MaNCC NVARCHAR(20) NOT NULL,
    TenNCC NVARCHAR(150) NOT NULL,
    DiaChi NVARCHAR(200) NULL,
    DienThoai NVARCHAR(20) NULL,
    CONSTRAINT PK_NhaCungCap PRIMARY KEY (MaNCC)
);
GO

-- ------------------------------------------------------------------------------------
-- 8. BẢNG SẢN PHẨM (Khớp 100% thuộc tính: GiaNhap, GiaBan, SoLuong, ThoiGianBaoHanh, Anh...)
-- ------------------------------------------------------------------------------------
CREATE TABLE SanPham (
    MaSP NVARCHAR(20) NOT NULL,
    TenSP NVARCHAR(150) NOT NULL,
    MaLoai NVARCHAR(20) NOT NULL,
    MaNhanHieu NVARCHAR(20) NOT NULL,
    MaManHinh NVARCHAR(20) NOT NULL,
    GiaNhap DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (GiaNhap >= 0),
    GiaBan DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (GiaBan >= 0),
    SoLuong INT NOT NULL DEFAULT 0 CHECK (SoLuong >= 0),
    ThoiGianBaoHanh INT NOT NULL DEFAULT 12,
    AmThanh NVARCHAR(100) NULL,
    ChupAnh NVARCHAR(200) NULL,
    Anh NVARCHAR(250) NULL,
    GhiChu NVARCHAR(MAX) NULL,
    CONSTRAINT PK_SanPham PRIMARY KEY (MaSP),
    CONSTRAINT FK_SanPham_Loai FOREIGN KEY (MaLoai) REFERENCES Loai(MaLoai) ON UPDATE CASCADE,
    CONSTRAINT FK_SanPham_NhanHieu FOREIGN KEY (MaNhanHieu) REFERENCES NhanHieu(MaNhanHieu) ON UPDATE CASCADE,
    CONSTRAINT FK_SanPham_ManHinh FOREIGN KEY (MaManHinh) REFERENCES ManHinh(MaManHinh) ON UPDATE CASCADE
);
GO

-- ------------------------------------------------------------------------------------
-- 9. BẢNG HÓA ĐƠN NHẬP (Phiếu nhập kho từ Nhà cung cấp)
-- ------------------------------------------------------------------------------------
CREATE TABLE HoaDonNhap (
    MaHDN NVARCHAR(20) NOT NULL,
    MaNV NVARCHAR(20) NOT NULL,
    NgayNhap DATETIME NOT NULL DEFAULT GETDATE(),
    MaNCC NVARCHAR(20) NOT NULL,
    TongTien DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (TongTien >= 0),
    CONSTRAINT PK_HoaDonNhap PRIMARY KEY (MaHDN),
    CONSTRAINT FK_HoaDonNhap_NhanVien FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV) ON UPDATE CASCADE,
    CONSTRAINT FK_HoaDonNhap_NhaCungCap FOREIGN KEY (MaNCC) REFERENCES NhaCungCap(MaNCC) ON UPDATE CASCADE
);
GO

-- ------------------------------------------------------------------------------------
-- 10. BẢNG CHI TIẾT HÓA ĐƠN NHẬP (KhuyenMai tính theo tỷ lệ % chiết khấu 0-100)
-- ------------------------------------------------------------------------------------
CREATE TABLE ChiTietHDN (
    MaHDN NVARCHAR(20) NOT NULL,
    MaSP NVARCHAR(20) NOT NULL,
    SoLuong INT NOT NULL CHECK (SoLuong > 0),
    DonGia DECIMAL(18,2) NOT NULL CHECK (DonGia >= 0),
    KhuyenMai DECIMAL(5,2) NOT NULL DEFAULT 0 CHECK (KhuyenMai >= 0 AND KhuyenMai <= 100),
    ThanhTien DECIMAL(18,2) NOT NULL CHECK (ThanhTien >= 0),
    CONSTRAINT PK_ChiTietHDN PRIMARY KEY (MaHDN, MaSP),
    CONSTRAINT FK_ChiTietHDN_HoaDonNhap FOREIGN KEY (MaHDN) REFERENCES HoaDonNhap(MaHDN) ON DELETE CASCADE,
    CONSTRAINT FK_ChiTietHDN_SanPham FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP) ON UPDATE CASCADE
);
GO

-- ------------------------------------------------------------------------------------
-- 11. BẢNG HÓA ĐƠN BÁN (Hóa đơn bán lẻ quầy POS & Đặt hàng Online)
-- ------------------------------------------------------------------------------------
CREATE TABLE HoaDonBan (
    MaHDB NVARCHAR(20) NOT NULL,
    MaNV NVARCHAR(20) NOT NULL,
    NgayBan DATETIME NOT NULL DEFAULT GETDATE(),
    MaKhach NVARCHAR(20) NOT NULL,
    TongTien DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (TongTien >= 0),
    CONSTRAINT PK_HoaDonBan PRIMARY KEY (MaHDB),
    CONSTRAINT FK_HoaDonBan_NhanVien FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV) ON UPDATE CASCADE,
    CONSTRAINT FK_HoaDonBan_KhachHang FOREIGN KEY (MaKhach) REFERENCES KhachHang(MaKhach) ON UPDATE CASCADE
);
GO

-- ------------------------------------------------------------------------------------
-- 12. BẢNG CHI TIẾT HÓA ĐƠN BÁN (KhuyenMai tính theo tỷ lệ % giảm giá 0-100)
-- ------------------------------------------------------------------------------------
CREATE TABLE ChiTietHDB (
    MaHDB NVARCHAR(20) NOT NULL,
    MaSP NVARCHAR(20) NOT NULL,
    SoLuong INT NOT NULL CHECK (SoLuong > 0),
    KhuyenMai DECIMAL(5,2) NOT NULL DEFAULT 0 CHECK (KhuyenMai >= 0 AND KhuyenMai <= 100),
    ThanhTien DECIMAL(18,2) NOT NULL CHECK (ThanhTien >= 0),
    CONSTRAINT PK_ChiTietHDB PRIMARY KEY (MaHDB, MaSP),
    CONSTRAINT FK_ChiTietHDB_HoaDonBan FOREIGN KEY (MaHDB) REFERENCES HoaDonBan(MaHDB) ON DELETE CASCADE,
    CONSTRAINT FK_ChiTietHDB_SanPham FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP) ON UPDATE CASCADE
);
GO

-- ------------------------------------------------------------------------------------
-- 13. BẢNG NHẬT KÝ TƯ VẤN CHATBOT AI (Gemini AI Consultant Logs)
-- ------------------------------------------------------------------------------------
CREATE TABLE Chatbot_TuVanLog (
    LogId INT IDENTITY(1,1) NOT NULL,
    SessionId NVARCHAR(100) NULL,
    NhuCauKhach NVARCHAR(500) NULL,
    NganSachToiDa DECIMAL(18,2) NULL,
    MaSP_GoiY NVARCHAR(20) NULL,
    PhanHoiChatbot NVARCHAR(MAX) NULL,
    ThoiGian DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_Chatbot_TuVanLog PRIMARY KEY (LogId),
    CONSTRAINT FK_Chatbot_SanPham FOREIGN KEY (MaSP_GoiY) REFERENCES SanPham(MaSP) ON UPDATE CASCADE ON DELETE SET NULL
);
GO

-- ====================================================================================
-- DATABASE TRIGGERS - QUY TẮC TỰ ĐỘNG HÓA KHO VÀ ĐỊNH GIÁ NIÊM YẾT
-- ====================================================================================

-- ------------------------------------------------------------------------------------
-- Trigger 1: Tự động cộng tồn kho, cập nhật giá nhập mới, định giá bán = GiaNhap * 1.10
-- ------------------------------------------------------------------------------------
IF OBJECT_ID('dbo.trg_ChiTietHDN_Change', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_ChiTietHDN_Change;
GO

CREATE TRIGGER trg_ChiTietHDN_Change
ON ChiTietHDN
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Cộng tồn kho khi nhập mới
    IF EXISTS (SELECT 1 FROM inserted)
    BEGIN
        UPDATE sp
        SET 
            sp.SoLuong = sp.SoLuong + ins.TongNhap,
            sp.GiaNhap = ins.DonGia,
            sp.GiaBan = ROUND(ins.DonGia * 1.10, 0)
        FROM SanPham sp
        INNER JOIN (
            SELECT MaSP, SUM(SoLuong) AS TongNhap, MAX(DonGia) AS DonGia
            FROM inserted
            GROUP BY MaSP
        ) ins ON sp.MaSP = ins.MaSP;
    END;

    -- Trừ tồn kho nếu xóa hoặc sửa chi tiết nhập cũ
    IF EXISTS (SELECT 1 FROM deleted)
    BEGIN
        UPDATE sp
        SET sp.SoLuong = sp.SoLuong - del.TongXoa
        FROM SanPham sp
        INNER JOIN (
            SELECT MaSP, SUM(SoLuong) AS TongXoa
            FROM deleted
            GROUP BY MaSP
        ) del ON sp.MaSP = del.MaSP;
    END;

    -- Cập nhật tổng tiền hóa đơn nhập
    UPDATE hdn
    SET hdn.TongTien = ISNULL((
        SELECT SUM(ct.ThanhTien)
        FROM ChiTietHDN ct
        WHERE ct.MaHDN = hdn.MaHDN
    ), 0)
    FROM HoaDonNhap hdn
    WHERE hdn.MaHDN IN (
        SELECT MaHDN FROM inserted
        UNION
        SELECT MaHDN FROM deleted
    );
END;
GO

-- ------------------------------------------------------------------------------------
-- Trigger 2: Kiểm tra tồn kho trước khi xuất bán, tự động trừ tồn kho, tính tổng tiền bán
-- ------------------------------------------------------------------------------------
IF OBJECT_ID('dbo.trg_ChiTietHDB_Change', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_ChiTietHDB_Change;
GO

CREATE TRIGGER trg_ChiTietHDB_Change
ON ChiTietHDB
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra ràng buộc tồn kho khả dụng trước khi xuất bán
    IF EXISTS (
        SELECT 1
        FROM (
            SELECT ins.MaSP, SUM(ins.SoLuong) AS TongBanMoi
            FROM inserted ins
            GROUP BY ins.MaSP
        ) ins
        INNER JOIN SanPham sp ON ins.MaSP = sp.MaSP
        LEFT JOIN (
            SELECT del.MaSP, SUM(del.SoLuong) AS TongBanCu
            FROM deleted del
            GROUP BY del.MaSP
        ) del ON ins.MaSP = del.MaSP
        WHERE (sp.SoLuong + ISNULL(del.TongBanCu, 0) - ins.TongBanMoi) < 0
    )
    BEGIN
        RAISERROR (N'Lỗi giao dịch: Số lượng sản phẩm trong kho không đủ để xuất bán!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END;

    -- Trừ tồn kho khi xuất bán mới
    IF EXISTS (SELECT 1 FROM inserted)
    BEGIN
        UPDATE sp
        SET sp.SoLuong = sp.SoLuong - ins.TongBan
        FROM SanPham sp
        INNER JOIN (
            SELECT MaSP, SUM(SoLuong) AS TongBan
            FROM inserted
            GROUP BY MaSP
        ) ins ON sp.MaSP = ins.MaSP;
    END;

    -- Hoàn lại tồn kho nếu xóa hoặc giảm số lượng bán
    IF EXISTS (SELECT 1 FROM deleted)
    BEGIN
        UPDATE sp
        SET sp.SoLuong = sp.SoLuong + del.TongHoan
        FROM SanPham sp
        INNER JOIN (
            SELECT MaSP, SUM(SoLuong) AS TongHoan
            FROM deleted
            GROUP BY MaSP
        ) del ON sp.MaSP = del.MaSP;
    END;

    -- Cập nhật tổng tiền hóa đơn bán
    UPDATE hdb
    SET hdb.TongTien = ISNULL((
        SELECT SUM(ct.ThanhTien)
        FROM ChiTietHDB ct
        WHERE ct.MaHDB = hdb.MaHDB
    ), 0)
    FROM HoaDonBan hdb
    WHERE hdb.MaHDB IN (
        SELECT MaHDB FROM inserted
        UNION
        SELECT MaHDB FROM deleted
    );
END;
GO

-- ====================================================================================
-- VIEWS BỔ TRỢ TRUY VẤN DỮ LIỆU
-- ====================================================================================

IF OBJECT_ID('dbo.vw_DanhSachSanPham', 'V') IS NOT NULL
    DROP VIEW dbo.vw_DanhSachSanPham;
GO

CREATE VIEW vw_DanhSachSanPham
AS
SELECT 
    sp.MaSP,
    sp.TenSP,
    sp.MaLoai,
    l.TenLoai,
    sp.MaNhanHieu,
    nh.TenNhanHieu,
    sp.MaManHinh,
    mh.TenManHinh,
    sp.GiaNhap,
    sp.GiaBan,
    sp.SoLuong,
    sp.ThoiGianBaoHanh,
    sp.AmThanh,
    sp.ChupAnh,
    sp.Anh,
    sp.GhiChu
FROM SanPham sp
INNER JOIN Loai l ON sp.MaLoai = l.MaLoai
INNER JOIN NhanHieu nh ON sp.MaNhanHieu = nh.MaNhanHieu
INNER JOIN ManHinh mh ON sp.MaManHinh = mh.MaManHinh;
GO

IF OBJECT_ID('dbo.vw_ChiTietHoaDonBan', 'V') IS NOT NULL
    DROP VIEW dbo.vw_ChiTietHoaDonBan;
GO

CREATE VIEW vw_ChiTietHoaDonBan
AS
SELECT 
    hdb.MaHDB,
    hdb.NgayBan,
    hdb.MaNV,
    nv.TenNV,
    hdb.MaKhach,
    kh.TenKhach,
    kh.DienThoai AS SDT_Khach,
    ctb.MaSP,
    sp.TenSP,
    ctb.SoLuong,
    sp.GiaBan AS DonGiaNiemYet,
    ctb.KhuyenMai,
    ctb.ThanhTien,
    hdb.TongTien AS TongTienHoaDon
FROM HoaDonBan hdb
INNER JOIN ChiTietHDB ctb ON hdb.MaHDB = ctb.MaHDB
INNER JOIN SanPham sp ON ctb.MaSP = sp.MaSP
INNER JOIN NhanVien nv ON hdb.MaNV = nv.MaNV
INNER JOIN KhachHang kh ON hdb.MaKhach = kh.MaKhach;
GO

-- ====================================================================================
-- STORED PROCEDURES CHO BÁO CÁO THỐNG KÊ DOANH THU & NHÂN SỰ
-- ====================================================================================

-- 1. Báo cáo top sản phẩm bán chạy nhất trong tháng
IF OBJECT_ID('dbo.sp_BaoCao_SP_BanChayNhat_Thang', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_BaoCao_SP_BanChayNhat_Thang;
GO

CREATE PROCEDURE sp_BaoCao_SP_BanChayNhat_Thang
    @Thang INT,
    @Nam INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ROW_NUMBER() OVER(ORDER BY SUM(ctb.SoLuong) DESC) AS STT,
        sp.MaSP,
        sp.TenSP,
        nh.TenNhanHieu,
        l.TenLoai,
        sp.GiaBan,
        SUM(ctb.SoLuong) AS TongSoLuongBan,
        SUM(ctb.ThanhTien) AS TongDoanhThu
    FROM ChiTietHDB ctb
    INNER JOIN HoaDonBan hdb ON ctb.MaHDB = hdb.MaHDB
    INNER JOIN SanPham sp ON ctb.MaSP = sp.MaSP
    INNER JOIN NhanHieu nh ON sp.MaNhanHieu = nh.MaNhanHieu
    INNER JOIN Loai l ON sp.MaLoai = l.MaLoai
    WHERE MONTH(hdb.NgayBan) = @Thang AND YEAR(hdb.NgayBan) = @Nam
    GROUP BY sp.MaSP, sp.TenSP, nh.TenNhanHieu, l.TenLoai, sp.GiaBan
    ORDER BY TongSoLuongBan DESC;
END;
GO

-- 2. Báo cáo hóa đơn nhập lớn nhất trong quý
IF OBJECT_ID('dbo.sp_BaoCao_HDNhap_LonNhat_Quy', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_BaoCao_HDNhap_LonNhat_Quy;
GO

CREATE PROCEDURE sp_BaoCao_HDNhap_LonNhat_Quy
    @Quy INT,
    @Nam INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ROW_NUMBER() OVER(ORDER BY hdn.TongTien DESC) AS STT,
        hdn.MaHDN,
        hdn.NgayNhap,
        nv.MaNV,
        nv.TenNV AS NguoiNhap,
        ncc.MaNCC,
        ncc.TenNCC AS NhaCungCap,
        hdn.TongTien
    FROM HoaDonNhap hdn
    INNER JOIN NhanVien nv ON hdn.MaNV = nv.MaNV
    INNER JOIN NhaCungCap ncc ON hdn.MaNCC = ncc.MaNCC
    WHERE DATEPART(QUARTER, hdn.NgayNhap) = @Quy AND YEAR(hdn.NgayNhap) = @Nam
    ORDER BY hdn.TongTien DESC;
END;
GO

-- 3. Báo cáo tổng kết doanh số bán theo năm
IF OBJECT_ID('dbo.sp_BaoCao_DoanhSo_Nam', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_BaoCao_DoanhSo_Nam;
GO

CREATE PROCEDURE sp_BaoCao_DoanhSo_Nam
    @Nam INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ROW_NUMBER() OVER(ORDER BY hdb.NgayBan ASC) AS STT,
        hdb.MaHDB,
        hdb.NgayBan,
        nv.MaNV,
        nv.TenNV AS NhanVienBan,
        kh.MaKhach,
        kh.TenKhach,
        kh.DienThoai AS SDT_Khach,
        hdb.TongTien
    FROM HoaDonBan hdb
    INNER JOIN NhanVien nv ON hdb.MaNV = nv.MaNV
    INNER JOIN KhachHang kh ON hdb.MaKhach = kh.MaKhach
    WHERE YEAR(hdb.NgayBan) = @Nam
    ORDER BY hdb.NgayBan ASC;
END;
GO

-- 4. Báo cáo 5 nhân viên doanh số thấp nhất trong tháng
IF OBJECT_ID('dbo.sp_BaoCao_5NV_BanItNhat_Thang', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_BaoCao_5NV_BanItNhat_Thang;
GO

CREATE PROCEDURE sp_BaoCao_5NV_BanItNhat_Thang
    @Thang INT,
    @Nam INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 5
        ROW_NUMBER() OVER(ORDER BY ISNULL(SUM(hdb.TongTien), 0) ASC, nv.MaNV ASC) AS STT,
        nv.MaNV,
        nv.TenNV,
        nv.DienThoai,
        ISNULL(SUM(hdb.TongTien), 0) AS TongTienBanDuoc,
        COUNT(hdb.MaHDB) AS SoHoaDonLap
    FROM NhanVien nv
    LEFT JOIN HoaDonBan hdb ON nv.MaNV = hdb.MaNV 
         AND MONTH(hdb.NgayBan) = @Thang 
         AND YEAR(hdb.NgayBan) = @Nam
    GROUP BY nv.MaNV, nv.TenNV, nv.DienThoai
    ORDER BY TongTienBanDuoc ASC, nv.MaNV ASC;
END;
GO

-- ====================================================================================
-- DỮ LIỆU KHỞI TẠO MẪU (SEED DATA CHUẨN THỰC TẾ)
-- ====================================================================================

-- 1. LOẠI
INSERT INTO Loai (MaLoai, TenLoai) VALUES 
('L01', N'Flagship cao cấp'),
('L02', N'Cận cao cấp'),
('L03', N'Tầm trung - Pin trâu'),
('L04', N'Phổ thông giá rẻ'),
('L05', N'Gaming Phone'),
('L06', N'Màn hình gập Foldable');

-- 2. NHÃN HIỆU
INSERT INTO NhanHieu (MaNhanHieu, TenNhanHieu) VALUES 
('NH01', N'Apple'),
('NH02', N'Samsung'),
('NH03', N'Xiaomi'),
('NH04', N'OPPO'),
('NH05', N'Vivo');

-- 3. MÀN HÌNH
INSERT INTO ManHinh (MaManHinh, TenManHinh) VALUES 
('MH01', N'Super Retina XDR OLED 120Hz ProMotion'),
('MH02', N'Dynamic AMOLED 2X 120Hz 2600 nits'),
('MH03', N'CrystalRes AMOLED 1.5K 120Hz Dolby Vision'),
('MH04', N'IPS LCD 90Hz FHD+'),
('MH05', N'LTPO AMOLED 1-120Hz 3000 nits');

-- 4. QUÊ
INSERT INTO Que (MaQue, TenQue) VALUES 
('Q01', N'Hà Nội'),
('Q02', N'Hải Phòng'),
('Q03', N'Nam Định'),
('Q04', N'Bắc Ninh'),
('Q05', N'Thái Bình');

-- 5. NHÂN VIÊN
INSERT INTO NhanVien (MaNV, TenNV, GioiTinh, NgaySinh, DienThoai, DiaChi, MaQue) VALUES 
('NV01', N'Nguyễn Văn Hùng', N'Nam', '2002-05-15', '0981234567', N'Cầu Giấy, Hà Nội', 'Q01'),
('NV02', N'Trần Thị Mai', N'Nữ', '2002-08-20', '0978999888', N'Đống Đa, Hà Nội', 'Q01'),
('NV03', N'Lê Thu Trang', N'Nữ', '2003-03-12', '0912345678', N'Thanh Xuân, Hà Nội', 'Q02'),
('NV04', N'Trần Tuấn Anh', N'Nam', '2001-11-28', '0945678901', N'Nam Từ Liêm, Hà Nội', 'Q03'),
('NV05', N'Hoàng Thảo My', N'Nữ', '2004-01-09', '0967890123', N'Hà Đông, Hà Nội', 'Q04'),
('NV06', N'Bùi Văn Hữu Đức', N'Nam', '2002-09-10', '0934567890', N'Ba Đình, Hà Nội', 'Q01');

-- 6. KHÁCH HÀNG
INSERT INTO KhachHang (MaKhach, TenKhach, DiaChi, DienThoai) VALUES 
('KH01', N'Hoàng Minh Tuấn', N'Đống Đa, Hà Nội', '0988776655'),
('KH02', N'Đặng Phương Linh', N'Hai Bà Trưng, Hà Nội', '0977665544'),
('KH03', N'Phạm Quang Minh', N'Hoàng Mai, Hà Nội', '0966554433'),
('KH04', N'Vũ Thị Mai', N'Hà Đông, Hà Nội', '0955443322');

-- 7. NHÀ CUNG CẤP
INSERT INTO NhaCungCap (MaNCC, TenNCC, DiaChi, DienThoai) VALUES 
('NCC01', N'Công ty CP Phân phối Synnex FPT', N'Quận 1, TP. HCM', '02873001234'),
('NCC02', N'Công ty Dầu khí Viễn thông Petrosetco', N'Quận 3, TP. HCM', '02839115566'),
('NCC03', N'Công ty CP Thế Giới Số Digiworld', N'Quận 5, TP. HCM', '02839290059');

-- 8. DANH MỤC 30 SẢN PHẨM SMARTPHONE CHÍNH HÃNG PHONG PHÚ
INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNhanHieu, MaManHinh, GiaNhap, GiaBan, SoLuong, ThoiGianBaoHanh, AmThanh, ChupAnh, Anh, GhiChu) VALUES 
-- APPLE IPHONE
('SP01', N'iPhone 15 Pro Max 256GB', 'L01', 'NH01', 'MH01', 26000000, 28600000, 15, 12, N'Loa kép Stereo, Spatial Audio', N'48MP Chính + 12MP Tele 5x + 12MP Góc siêu rộng', 'sp01.jpg', N'Chip Apple A17 Pro 3nm, khung vỏ Titan chuẩn hàng không vũ trụ, màu Titan Xanh'),
('SP06', N'iPhone 16 Pro Max 256GB', 'L01', 'NH01', 'MH01', 31800000, 34980000, 20, 12, N'Loa kép Stereo chuẩn Dolby Atmos', N'48MP Fusion + 48MP Ultra-wide + 12MP Tele 5x, Camera Control', 'sp06.jpg', N'Màn hình 6.9 inch siêu lớn, nút Camera Control xúc giác, màu Titan Sa Mạc tuyệt đẹp'),
('SP07', N'iPhone 16 Pro 128GB', 'L01', 'NH01', 'MH01', 26000000, 28600000, 14, 12, N'Loa kép Stereo vòm chuẩn Studio', N'48MP Fusion + 48MP Ultra-wide + 12MP Tele 5x', 'sp07.png', N'Chip Apple A18 Pro, viền siêu mỏng, quay video 4K 120fps Dolby Vision, Titan Tự Nhiên'),
('SP08', N'iPhone 16 Plus 128GB', 'L02', 'NH01', 'MH01', 23500000, 25850000, 18, 12, N'Loa kép vòm Stereo sống động', N'48MP Fusion 2x Tele + 12MP Ultra-wide Macro', 'sp08.png', N'Màn hình 6.7 inch Super Retina XDR, pin trâu vượt trội, nút Action Button và Camera Control'),
('SP09', N'iPhone 16 128GB', 'L02', 'NH01', 'MH01', 20800000, 22880000, 22, 12, N'Loa kép Stereo âm thanh không gian', N'48MP Fusion + 12MP Ultra-wide chụp Spatial Photo', 'sp09.png', N'Chip Apple A18 cực mạnh hỗ trợ Apple Intelligence, cụm camera dọc hiện đại, màu Đen'),
('SP10', N'iPhone 15 128GB', 'L02', 'NH01', 'MH01', 17500000, 19250000, 25, 12, N'Loa kép Stereo', N'48MP OIS + 12MP Ultra-wide', 'sp10.jpg', N'Màn hình Dynamic Island thời thượng, cổng sạc Type-C tiện lợi, màu Đen cá tính'),
('SP11', N'iPhone 13 128GB', 'L03', 'NH01', 'MH01', 12000000, 13200000, 30, 12, N'Loa kép Stereo', N'12MP Kép chống rung cảm biến Sensor-shift', 'sp11.jpg', N'Smartphone quốc dân bền bỉ, chip A15 Bionic mượt mà, màu Trắng Ánh Sao Starlight'),

-- SAMSUNG GALAXY
('SP02', N'Samsung Galaxy S24 Ultra 256GB', 'L01', 'NH02', 'MH02', 24000000, 26400000, 16, 12, N'Loa kép AKG, Dolby Atmos', N'200MP OIS + 50MP Periscope 5x + 10MP 3x + 12MP Ultra-wide', 'sp02.jpg', N'Chip Snapdragon 8 Gen 3 for Galaxy, bút S-Pen tích hợp, khung viền Titan và Galaxy AI'),
('SP12', N'Samsung Galaxy S24+ 256GB', 'L01', 'NH02', 'MH02', 20000000, 22000000, 18, 12, N'Loa kép Stereo Dolby Atmos', N'50MP Dual Pixel OIS + 10MP 3x + 12MP Ultra-wide', 'sp12.jpg', N'Màn hình 6.7 inch QHD+ 2600 nits, RAM 12GB đa nhiệm, tích hợp trọn vẹn Galaxy AI'),
('SP13', N'Samsung Galaxy S24 256GB', 'L02', 'NH02', 'MH02', 16000000, 17600000, 15, 12, N'Loa kép Stereo AKG', N'50MP OIS + 10MP Tele 3x + 12MP Ultra-wide', 'sp13.jpg', N'Thiết kế nhỏ gọn 6.2 inch viền mỏng đối xứng, màn hình Dynamic AMOLED 2X 120Hz mượt mà'),
('SP14', N'Samsung Galaxy Z Fold6 5G 256GB', 'L06', 'NH02', 'MH02', 38000000, 41800000, 10, 12, N'Loa kép Stereo vòm 3D', N'50MP OIS + 10MP Tele 3x + 12MP Góc siêu rộng', 'sp14.jpg', N'Màn hình gập 7.6 inch siêu phẳng, bản lề FlexHinge thế hệ mới, đa nhiệm chia 3 màn hình AI'),
('SP15', N'Samsung Galaxy Z Flip6 5G 256GB', 'L06', 'NH02', 'MH02', 24500000, 26950000, 12, 12, N'Loa kép Stereo sống động', N'50MP OIS nâng cấp + 12MP Ultra-wide FlexCam', 'sp15.jpg', N'Gập vỏ sò thời thượng, màn hình ngoài Flex Window 3.4 inch tùy biến linh hoạt, pin 4000mAh'),
('SP16', N'Samsung Galaxy A55 5G 8GB/128GB', 'L03', 'NH02', 'MH02', 8500000, 9350000, 28, 12, N'Loa kép Stereo sống động', N'50MP OIS + 12MP Ultra-wide + 5MP Macro', 'sp16.jpg', N'Khung viền kim loại sang trọng, kính cường lực Gorilla Glass Victus+, kháng nước chuẩn IP67'),
('SP17', N'Samsung Galaxy A35 5G 8GB/128GB', 'L03', 'NH02', 'MH02', 6800000, 7480000, 24, 12, N'Loa kép Dolby Atmos', N'50MP OIS + 8MP Góc rộng + 5MP Macro', 'sp17.jpg', N'Màn hình Super AMOLED 120Hz sắc nét, pin trâu 5000mAh, thiết kế mặt lưng kính màu Vàng chanh'),
('SP18', N'Samsung Galaxy A15 8GB/128GB', 'L04', 'NH02', 'MH04', 4100000, 4510000, 35, 12, N'Loa đơn tối ưu giọng thoại', N'50MP Chính + 5MP Góc rộng + 2MP Macro', 'sp18.jpg', N'Màn hình Super AMOLED 90Hz sáng đẹp trong phân khúc giá rẻ, pin 5000mAh bền bỉ 2 ngày'),

-- XIAOMI
('SP03', N'Xiaomi 14 5G 12GB/256GB', 'L02', 'NH03', 'MH03', 16000000, 17600000, 16, 18, N'Loa kép Stereo Hi-Res Audio', N'50MP Leica Summilux + 50MP Tele 3.2x + 50MP Ultra-wide', 'sp03.jpg', N'Hệ thống ống kính Leica quang học cao cấp, chip Snapdragon 8 Gen 3, sạc siêu tốc 90W'),
('SP19', N'Xiaomi 14 Ultra 5G 16GB/512GB', 'L01', 'NH03', 'MH03', 27000000, 29700000, 8, 18, N'Loa kép Stereo chuẩn Hi-Res & Dolby Atmos', N'Quad 50MP Leica: Cảm biến 1-inch LYT-900 khẩu độ vô cấp', 'sp19.jpg', N'Vua nhiếp ảnh di động, khung viền nhôm nguyên khối, mặt lưng da thuần chay nano kháng bám bẩn'),
('SP20', N'Xiaomi 13T Pro 5G 12GB/512GB', 'L02', 'NH03', 'MH03', 13500000, 14850000, 14, 18, N'Loa kép Dolby Atmos cao cấp', N'50MP Leica OIS + 50MP Tele 2x + 12MP Góc rộng', 'sp20.jpg', N'Chip Dimensity 9200+ 4nm cực mạnh, sạc siêu nhanh HyperCharge 120W nạp đầy trong 19 phút'),
('SP21', N'Redmi Note 13 Pro+ 5G 8GB/256GB', 'L03', 'NH03', 'MH03', 8200000, 9020000, 22, 12, N'Loa kép Stereo Dolby Atmos', N'200MP OIS Samsung HP3 + 8MP Góc rộng + 2MP Macro', 'sp21.jpg', N'Màn hình cong tràn 1.5K 120Hz đẳng cấp, kháng nước chuẩn IP68, sạc nhanh 120W hàng đầu'),
('SP05', N'Redmi Note 13 Pro 4G 8GB/128GB', 'L04', 'NH03', 'MH04', 4500000, 4950000, 30, 12, N'Loa kép Dolby Atmos stereo', N'200MP Samsung ISOCELL HP3 OIS + 8MP + 2MP', 'sp05.jpg', N'Camera 200MP siêu nét trong phân khúc, viền siêu mỏng, pin 5000mAh, sạc nhanh 67W Turbo'),
('SP22', N'POCO X6 Pro 5G 12GB/512GB', 'L05', 'NH03', 'MH03', 8000000, 8800000, 18, 12, N'Loa kép Hi-Res Stereo Audio', N'64MP OIS + 8MP Ultra-wide + 2MP Macro', 'sp22.jpg', N'Cỗ máy chiến game phân khúc 8 triệu, vi xử lý Dimensity 8300-Ultra điểm AnTuTu 1.4 triệu'),
('SP23', N'Redmi 13 8GB/128GB', 'L04', 'NH03', 'MH04', 3600000, 3960000, 40, 12, N'Loa âm lượng lớn 150%', N'108MP Siêu nét + 2MP Macro', 'sp23.jpg', N'Mặt lưng kính bóng bẩy cao cấp, camera 108MP zoom 3x cảm biến, sạc nhanh 33W pin 5030mAh'),

-- OPPO
('SP04', N'OPPO Reno11 5G 256GB', 'L03', 'NH04', 'MH03', 8000000, 8800000, 20, 12, N'Loa âm thanh vòm Stereo', N'50MP OIS Sony LYT600 + 32MP Tele Chân dung + 8MP Góc rộng', 'sp04.jpg', N'Chuyên gia chân dung thế hệ mới, thiết kế vân lụa sóng biển, sạc nhanh 67W SuperVOOC'),
('SP24', N'OPPO Find N3 Flip 5G 12GB/256GB', 'L06', 'NH04', 'MH02', 20000000, 22000000, 10, 12, N'Loa kép Stereo Dolby Atmos', N'50MP OIS + 32MP Tele + 48MP Ultra-wide Hasselblad', 'sp24.jpg', N'Điện thoại gập vỏ sò đầu tiên có 3 camera Hasselblad, màn hình ngoài tỷ lệ dọc tự nhiên'),
('SP25', N'OPPO Reno12 Pro 5G 12GB/512GB', 'L02', 'NH04', 'MH03', 16500000, 18150000, 15, 12, N'Loa kép siêu âm lượng 300%', N'50MP Sony LYT800 OIS + 50MP Tele 2x + 8MP Góc rộng', 'sp25.jpg', N'Thiết kế dòng chảy Bạc Tinh Vân chống va đập chuẩn quân đội, bộ công cụ AI xóa vật thể thông minh'),
('SP26', N'OPPO Reno12 F 5G 8GB/256GB', 'L03', 'NH04', 'MH03', 8300000, 9130000, 25, 12, N'Loa kép công nghệ Holo Audio', N'50MP OIS + 8MP Góc rộng + 2MP Macro', 'sp26.jpg', N'Vòng tròn ánh sao Halo Light đổi màu theo điệu nhạc và thông báo, thiết kế mỏng nhẹ trẻ trung'),
('SP27', N'OPPO A79 5G 8GB/256GB', 'L03', 'NH04', 'MH03', 6500000, 7150000, 20, 12, N'Loa kép Stereo công suất lớn', N'50MP AI + 2MP Đo độ sâu', 'sp27.jpg', N'Hỗ trợ 5G tốc độ cao, mặt lưng vân lông vũ phát sáng, RAM mở rộng lên đến 16GB mượt mà'),
('SP28', N'OPPO A58 6GB/128GB', 'L04', 'NH04', 'MH04', 4200000, 4620000, 30, 12, N'Loa kép âm thanh nổi sống động', N'50MP AI Chân dung', 'sp28.jpg', N'Màn hình lớn 6.72 inch FHD+ Sunlight Display, sạc nhanh an toàn 33W SuperVOOC pin 5000mAh'),

-- VIVO
('SP29', N'Vivo V30e 5G 8GB/256GB', 'L03', 'NH05', 'MH03', 8400000, 9240000, 18, 12, N'Loa kép Stereo Hi-Res Audio', N'50MP Sony IMX882 OIS + 8MP Ultra-wide, Vòng sáng Aura AI', 'sp29.jpg', N'Vòng sáng Aura thế hệ mới tự điều chỉnh nhiệt độ màu, thiết kế đồng hồ cổ điển siêu mỏng'),
('SP30', N'Vivo Y100 8GB/128GB', 'L04', 'NH05', 'MH04', 6200000, 6820000, 25, 12, N'Loa kép âm lượng 300% Audio Booster', N'50MP Camera chính + Cảm biến Flicker chống nháy', 'sp30.jpg', N'Mặt lưng chuyển màu kim cương lấp lánh dưới ánh nắng, sạc siêu tốc 80W nạp 45% trong 15 phút');

-- 9. HÓA ĐƠN NHẬP KHO MẪU
INSERT INTO HoaDonNhap (MaHDN, MaNV, NgayNhap, MaNCC, TongTien) VALUES 
('HDN01', 'NV01', '2026-09-01 08:30:00', 'NCC01', 0),
('HDN02', 'NV02', '2026-09-05 14:15:00', 'NCC03', 0);

-- 10. CHI TIẾT HÓA ĐƠN NHẬP
INSERT INTO ChiTietHDN (MaHDN, MaSP, SoLuong, DonGia, KhuyenMai, ThanhTien) VALUES 
('HDN01', 'SP01', 10, 26000000, 0, 260000000),
('HDN01', 'SP02', 10, 24000000, 0, 240000000),
('HDN02', 'SP03', 15, 16000000, 0, 240000000),
('HDN02', 'SP04', 20,  8000000, 0, 160000000),
('HDN02', 'SP05', 30,  4500000, 0, 135000000);

-- 11. HÓA ĐƠN BÁN HÀNG MẪU
INSERT INTO HoaDonBan (MaHDB, MaNV, NgayBan, MaKhach, TongTien) VALUES 
('HDB01', 'NV03', '2026-09-10 10:20:00', 'KH01', 0),
('HDB02', 'NV04', '2026-09-12 15:45:00', 'KH02', 0),
('HDB03', 'NV03', '2026-09-15 11:30:00', 'KH03', 0);

-- 12. CHI TIẾT HÓA ĐƠN BÁN
INSERT INTO ChiTietHDB (MaHDB, MaSP, SoLuong, KhuyenMai, ThanhTien) VALUES 
('HDB01', 'SP01', 1, 0, 28600000),
('HDB02', 'SP02', 1, 2.0, 25872000),
('HDB02', 'SP05', 2, 0,    9900000),
('HDB03', 'SP03', 2, 5.0, 33440000);

-- 13. DỮ LIỆU NHẬT KÝ TƯ VẤN CHATBOT AI MẪU
INSERT INTO Chatbot_TuVanLog (SessionId, NhuCauKhach, NganSachToiDa, MaSP_GoiY, PhanHoiChatbot) VALUES 
('SES_001', N'Tầm 18 triệu cần máy chụp ảnh đẹp, màn hình 120Hz mượt mà', 18000000, 'SP03', N'Nexus Retail gợi ý mẫu Xiaomi 14 5G ống kính Leica đỉnh cao, giá niêm yết 17.600.000đ, hiện đang sẵn hàng tại shop!'),
('SES_002', N'Tầm 5 triệu cần máy pin trâu cho phụ huynh đọc báo xem youtube', 5000000, 'SP05', N'Nexus Retail gợi ý mẫu Redmi Note 13 Pro 4G pin khủng 5000mAh, màn AMOLED 120Hz siêu nét, giá 4.950.000đ.');
GO

PRINT N'✔ Đã hoàn tất khởi tạo NexusRetailDb (13 bảng, triggers, views, procedures và 30 thiết bị smartphone phong phú)!';
GO

-- ####################################################################################
-- PHẦN 2: CƠ SỞ DỮ LIỆU XÁC THỰC DANH TÍNH (NexusRetailIdentityDb)
-- Dành cho NexusRetail.Identity.Api cấp quyền đăng nhập JWT RBAC (Admin, Staff, Customer)
-- ####################################################################################

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'NexusRetailIdentityDb')
BEGIN
    CREATE DATABASE NexusRetailIdentityDb;
    PRINT N'✔ Đã tạo mới cơ sở dữ liệu: NexusRetailIdentityDb';
END
ELSE
BEGIN
    PRINT N'✔ Cơ sở dữ liệu NexusRetailIdentityDb đã sẵn sàng.';
END
GO

USE NexusRetailIdentityDb;
GO

IF OBJECT_ID('dbo.AspNetUserRoles', 'U') IS NOT NULL DROP TABLE dbo.AspNetUserRoles;
IF OBJECT_ID('dbo.AspNetUserClaims', 'U') IS NOT NULL DROP TABLE dbo.AspNetUserClaims;
IF OBJECT_ID('dbo.AspNetRoleClaims', 'U') IS NOT NULL DROP TABLE dbo.AspNetRoleClaims;
IF OBJECT_ID('dbo.AspNetUserLogins', 'U') IS NOT NULL DROP TABLE dbo.AspNetUserLogins;
IF OBJECT_ID('dbo.AspNetUserTokens', 'U') IS NOT NULL DROP TABLE dbo.AspNetUserTokens;
IF OBJECT_ID('dbo.AspNetRoles', 'U') IS NOT NULL DROP TABLE dbo.AspNetRoles;
IF OBJECT_ID('dbo.AspNetUsers', 'U') IS NOT NULL DROP TABLE dbo.AspNetUsers;
GO

-- 1. BẢNG ASPNETROLES
CREATE TABLE AspNetRoles (
    Id NVARCHAR(128) NOT NULL,
    Name NVARCHAR(256) NULL,
    NormalizedName NVARCHAR(256) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL,
    CONSTRAINT PK_AspNetRoles PRIMARY KEY (Id)
);
GO

-- 2. BẢNG ASPNETUSERS
CREATE TABLE AspNetUsers (
    Id NVARCHAR(128) NOT NULL,
    UserName NVARCHAR(256) NULL,
    NormalizedUserName NVARCHAR(256) NULL,
    Email NVARCHAR(256) NULL,
    NormalizedEmail NVARCHAR(256) NULL,
    EmailConfirmed BIT NOT NULL DEFAULT 1,
    PasswordHash NVARCHAR(MAX) NULL,
    SecurityStamp NVARCHAR(MAX) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL,
    PhoneNumber NVARCHAR(50) NULL,
    PhoneNumberConfirmed BIT NOT NULL DEFAULT 0,
    TwoFactorEnabled BIT NOT NULL DEFAULT 0,
    LockoutEnd DATETIMEOFFSET NULL,
    LockoutEnabled BIT NOT NULL DEFAULT 1,
    AccessFailedCount INT NOT NULL DEFAULT 0,
    CONSTRAINT PK_AspNetUsers PRIMARY KEY (Id)
);
GO

-- 3. BẢNG ASPNETUSERROLES (Khóa chính kép 128 + 128 = 256 ký tự = 512 bytes <= 900 bytes Clustered Index Limit)
CREATE TABLE AspNetUserRoles (
    UserId NVARCHAR(128) NOT NULL,
    RoleId NVARCHAR(128) NOT NULL,
    CONSTRAINT PK_AspNetUserRoles PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_AspNetUserRoles_User FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    CONSTRAINT FK_AspNetUserRoles_Role FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);
GO

-- 4. BẢNG ASPNETUSERCLAIMS
CREATE TABLE AspNetUserClaims (
    Id INT IDENTITY(1,1) NOT NULL,
    UserId NVARCHAR(128) NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    CONSTRAINT PK_AspNetUserClaims PRIMARY KEY (Id),
    CONSTRAINT FK_AspNetUserClaims_User FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);
GO

-- 5. BẢNG ASPNETROLECLAIMS
CREATE TABLE AspNetRoleClaims (
    Id INT IDENTITY(1,1) NOT NULL,
    RoleId NVARCHAR(128) NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    CONSTRAINT PK_AspNetRoleClaims PRIMARY KEY (Id),
    CONSTRAINT FK_AspNetRoleClaims_Role FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);
GO

-- 6. BẢNG ASPNETUSERLOGINS
CREATE TABLE AspNetUserLogins (
    LoginProvider NVARCHAR(128) NOT NULL,
    ProviderKey NVARCHAR(128) NOT NULL,
    ProviderDisplayName NVARCHAR(MAX) NULL,
    UserId NVARCHAR(128) NOT NULL,
    CONSTRAINT PK_AspNetUserLogins PRIMARY KEY (LoginProvider, ProviderKey),
    CONSTRAINT FK_AspNetUserLogins_User FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);
GO

-- 7. BẢNG ASPNETUSERTOKENS (128 + 128 + 128 = 384 ký tự = 768 bytes <= 900 bytes Clustered Index Limit)
CREATE TABLE AspNetUserTokens (
    UserId NVARCHAR(128) NOT NULL,
    LoginProvider NVARCHAR(128) NOT NULL,
    Name NVARCHAR(128) NOT NULL,
    Value NVARCHAR(MAX) NULL,
    CONSTRAINT PK_AspNetUserTokens PRIMARY KEY (UserId, LoginProvider, Name),
    CONSTRAINT FK_AspNetUserTokens_User FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);
GO

-- KHỞI TẠO ROLES CHUẨN RBAC
INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES 
('R01', 'Admin', 'ADMIN', NEWID()),
('R02', 'Staff', 'STAFF', NEWID()),
('R03', 'Customer', 'CUSTOMER', NEWID());

-- KHỞI TẠO TÀI KHOẢN MẪU (MẬT KHẨU ĐÃ BĂM PBKDF2 CHUẨN ASP.NET CORE IDENTITY)
-- 1. admin     / admin123
-- 2. nhanvien  / 123456
-- 3. customer  / 123456
INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber) VALUES 
('87facb0c-1847-4bb6-8dfa-8398caafe5d2', 'admin', 'ADMIN', 'admin@nexusretail.vn', 'ADMIN@NEXUSRETAIL.VN', 1, 'AQAAAAIAAYagAAAAEJG6RITiFscu0Qi0lq/+78FkvT3BcsXPm25GywVgQ36VD3DCWvrFIYRVcBupLr6IUg==', 'TOJ2RVTPOTJWRUKAUZMEJXAD6QYXGZ7U', NEWID(), '0981234567'),
('919f4f75-00d1-4e34-8bb3-a44486f8c535', 'nhanvien', 'NHANVIEN', 'nhanvien@nexusretail.vn', 'NHANVIEN@NEXUSRETAIL.VN', 1, 'AQAAAAIAAYagAAAAEDNYdagvmxStQMb8l4AcWB46sB/TCz4hgSonvuD+f+WlXIGXeC+y5cZUg98jGTFYbw==', 'KGLMM72OTARKYEDQOSBQHNSCJCQNDKCS', NEWID(), '0978999888'),
('e8098fcc-f98b-4799-897f-40b899c5a923', 'customer', 'CUSTOMER', 'customer@nexusretail.vn', 'CUSTOMER@NEXUSRETAIL.VN', 1, 'AQAAAAIAAYagAAAAEMwfMaA0yh7q2plIS77wnczQDtxWukz+EnMmbfLsF1BuypwIUrH7SfE4Rt24WHn/BA==', 'A4X3JUFPRUCDE2EVXEXZZVRX4HV2JYQB', NEWID(), '0966554433');

-- GÁN ROLE CHO TÀI KHOẢN
INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES 
('87facb0c-1847-4bb6-8dfa-8398caafe5d2', 'R01'), -- admin -> Admin
('919f4f75-00d1-4e34-8bb3-a44486f8c535', 'R02'), -- nhanvien -> Staff
('e8098fcc-f98b-4799-897f-40b899c5a923', 'R03'); -- customer -> Customer
GO

PRINT N'================================================================================';
PRINT N'✔ HOÀN TẤT CÀI ĐẶT 100% CƠ SỞ DỮ LIỆU NEXUS RETAIL CHO MÁY MỚI!';
PRINT N'1. NexusRetailDb: 13 bảng + 2 triggers + 2 views + 4 stored procedures + seed';
PRINT N'2. NexusRetailIdentityDb: Quản lý người dùng, roles và pre-hashed credentials';
PRINT N'   - Tài khoản Admin:    admin    / admin123';
PRINT N'   - Tài khoản Nhân viên: nhanvien / 123456';
PRINT N'   - Tài khoản Khách:    customer / 123456';
PRINT N'================================================================================';
GO
