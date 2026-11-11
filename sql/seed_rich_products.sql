USE NexusRetailDb;
GO

SET NOCOUNT ON;

-- 1. BỔ SUNG LOOKUP BẢNG LOẠI
IF NOT EXISTS (SELECT 1 FROM Loai WHERE MaLoai = 'L05')
    INSERT INTO Loai (MaLoai, TenLoai) VALUES ('L05', N'Gaming Phone');

IF NOT EXISTS (SELECT 1 FROM Loai WHERE MaLoai = 'L06')
    INSERT INTO Loai (MaLoai, TenLoai) VALUES ('L06', N'Màn hình gập Foldable');

-- 2. BỔ SUNG LOOKUP BẢNG NHÃN HIỆU
IF NOT EXISTS (SELECT 1 FROM NhanHieu WHERE MaNhanHieu = 'NH05')
    INSERT INTO NhanHieu (MaNhanHieu, TenNhanHieu) VALUES ('NH05', N'Vivo');

-- 3. BỔ SUNG LOOKUP BẢNG MÀN HÌNH
IF NOT EXISTS (SELECT 1 FROM ManHinh WHERE MaManHinh = 'MH05')
    INSERT INTO ManHinh (MaManHinh, TenManHinh) VALUES ('MH05', N'LTPO AMOLED 1-120Hz 3000 nits');

-- 4. BẢNG TẠM CHỨA 30 SẢN PHẨM SMARTPHONE PHONG PHÚ KÈM ẢNH CHÍNH HÃNG
DECLARE @Phones TABLE (
    MaSP NVARCHAR(20),
    TenSP NVARCHAR(150),
    MaLoai NVARCHAR(20),
    MaNhanHieu NVARCHAR(20),
    MaManHinh NVARCHAR(20),
    GiaNhap DECIMAL(18,2),
    GiaBan DECIMAL(18,2),
    SoLuong INT,
    ThoiGianBaoHanh INT,
    AmThanh NVARCHAR(100),
    ChupAnh NVARCHAR(200),
    Anh NVARCHAR(250),
    GhiChu NVARCHAR(MAX)
);

INSERT INTO @Phones VALUES
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

-- 5. TIẾN HÀNH MERGE / CẬP NHẬT VÀO BẢNG SANPHAM
MERGE SanPham AS target
USING @Phones AS source
ON (target.MaSP = source.MaSP)
WHEN MATCHED THEN
    UPDATE SET 
        TenSP = source.TenSP,
        MaLoai = source.MaLoai,
        MaNhanHieu = source.MaNhanHieu,
        MaManHinh = source.MaManHinh,
        GiaNhap = source.GiaNhap,
        GiaBan = source.GiaBan,
        SoLuong = source.SoLuong,
        ThoiGianBaoHanh = source.ThoiGianBaoHanh,
        AmThanh = source.AmThanh,
        ChupAnh = source.ChupAnh,
        Anh = source.Anh,
        GhiChu = source.GhiChu
WHEN NOT MATCHED THEN
    INSERT (MaSP, TenSP, MaLoai, MaNhanHieu, MaManHinh, GiaNhap, GiaBan, SoLuong, ThoiGianBaoHanh, AmThanh, ChupAnh, Anh, GhiChu)
    VALUES (source.MaSP, source.TenSP, source.MaLoai, source.MaNhanHieu, source.MaManHinh, source.GiaNhap, source.GiaBan, source.SoLuong, source.ThoiGianBaoHanh, source.AmThanh, source.ChupAnh, source.Anh, source.GhiChu);

SELECT COUNT(*) AS TongSoSanPham, SUM(SoLuong) AS TongTonKho, AVG(GiaBan) AS GiaBanTrungBinh FROM SanPham;
GO
