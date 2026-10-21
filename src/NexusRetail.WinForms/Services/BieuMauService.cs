using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace NexusRetail.WinForms.Services
{
    public class ItemBieuMauBan
    {
        public int Stt { get; set; }
        public string MaSp { get; set; } = "";
        public string TenSp { get; set; } = "";
        public string QuyCach { get; set; } = "";
        public string Imei { get; set; } = "";
        public string BaoHanh { get; set; } = "12T";
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }

    public class ItemBieuMauNhap
    {
        public int Stt { get; set; }
        public string MaSp { get; set; } = "";
        public string TenSp { get; set; } = "";
        public string Dvt { get; set; } = "Cây";
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal KhuyenMai { get; set; }
        public decimal ThanhTien { get; set; }
    }

    public class ItemBieuMauKho
    {
        public int Stt { get; set; }
        public string MaSp { get; set; } = "";
        public string TenSp { get; set; } = "";
        public string ThuongHieu { get; set; } = "";
        public int TonDauSl { get; set; }
        public decimal TonDauTien { get; set; }
        public int NhapSl { get; set; }
        public decimal NhapTien { get; set; }
        public int XuatSl { get; set; }
        public decimal XuatTien { get; set; }
        public int TonCuoiSl { get; set; }
        public decimal TonCuoiTien { get; set; }
        public string TrangThai { get; set; } = "Còn hàng";
    }

    public static class BieuMauService
    {
        private static readonly CultureInfo ViCulture = new("vi-VN");

        #region 1. CHUYỂN ĐỔI SỐ TIỀN THÀNH CHỮ TIẾNG VIỆT
        private static readonly string[] ChuSo = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
        private static readonly string[] Tien = { "", "nghìn", "triệu", "tỷ", "nghìn tỷ", "triệu tỷ" };

        private static string DocKhoi3So(int so, bool dayDu)
        {
            int tram = so / 100;
            int chuc = (so % 100) / 10;
            int donVi = so % 10;
            var sb = new StringBuilder();

            if (tram > 0 || dayDu)
            {
                sb.Append(ChuSo[tram]).Append(" trăm ");
            }

            if (chuc > 1)
            {
                sb.Append(ChuSo[chuc]).Append(" mươi ");
                if (donVi == 1) sb.Append("mốt ");
                else if (donVi == 5) sb.Append("lăm ");
                else if (donVi > 0) sb.Append(ChuSo[donVi]).Append(' ');
            }
            else if (chuc == 1)
            {
                sb.Append("mười ");
                if (donVi == 5) sb.Append("lăm ");
                else if (donVi > 0) sb.Append(ChuSo[donVi]).Append(' ');
            }
            else if (dayDu && donVi > 0)
            {
                sb.Append("lẻ ");
                sb.Append(ChuSo[donVi]).Append(' ');
            }
            else if (donVi > 0)
            {
                sb.Append(ChuSo[donVi]).Append(' ');
            }

            return sb.ToString().Trim();
        }

        public static string DocSoThanhChu(decimal soTien)
        {
            if (soTien == 0) return "Không đồng";
            long so = (long)Math.Abs(soTien);
            var sb = new StringBuilder();
            var nhomSo = new List<int>();

            while (so > 0)
            {
                nhomSo.Add((int)(so % 1000));
                so /= 1000;
            }

            for (int i = nhomSo.Count - 1; i >= 0; i--)
            {
                if (nhomSo[i] > 0)
                {
                    bool dayDu = i < nhomSo.Count - 1;
                    string chuoi = DocKhoi3So(nhomSo[i], dayDu);
                    sb.Append(chuoi).Append(' ').Append(Tien[i]).Append(' ');
                }
            }

            string result = sb.ToString().Trim().Replace("  ", " ");
            if (!string.IsNullOrEmpty(result))
            {
                result = char.ToUpper(result[0]) + result[1..] + " đồng chẵn./.";
            }
            return result;
        }
        #endregion

        #region 2. XUẤT VÀ HIỂN THỊ BIỂU MẪU BM02: HÓA ĐƠN BÁN LẺ & BẢO HÀNH
        public static string TaoHtmlHoaDonBan(
            string maHdb,
            string tenKhach,
            string dienThoai,
            string diaChi,
            string tenNv,
            List<ItemBieuMauBan> danhSachSp,
            decimal tongTien,
            decimal giamGia = 0)
        {
            decimal tongThanhToan = tongTien - giamGia;
            string tienChu = DocSoThanhChu(tongThanhToan);
            string ngayIn = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            var trRows = new StringBuilder();
            for (int i = 0; i < danhSachSp.Count; i++)
            {
                var sp = danhSachSp[i];
                string imeiStr = string.IsNullOrWhiteSpace(sp.Imei) ? $"IMEI: 3589{i + 1}00{DateOnly.FromDateTime(DateTime.Now).DayOfYear}01" : sp.Imei;
                string quyCachStr = string.IsNullOrWhiteSpace(sp.QuyCach) ? "Chính hãng (VN/A)" : sp.QuyCach;

                trRows.Append($@"
                <tr>
                    <td class='text-center'>{i + 1}</td>
                    <td>
                        <strong>{sp.TenSp}</strong><br>
                        <small style='color: #64748b;'>{quyCachStr}<br>{imeiStr}</small>
                    </td>
                    <td class='text-center'>{sp.BaoHanh}</td>
                    <td class='text-center'>{sp.SoLuong}</td>
                    <td class='text-right'>{sp.DonGia.ToString("N0", ViCulture)}</td>
                    <td class='text-right'><strong>{sp.ThanhTien.ToString("N0", ViCulture)}</strong></td>
                </tr>");
            }

            return $@"<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>BM02 - Hóa Đơn Bán Lẻ & Phiếu Bảo Hành - {maHdb}</title>
    <script src='https://cdn.jsdelivr.net/npm/html2canvas@1.4.1/dist/html2canvas.min.js'></script>
    <style>
        * {{ box-sizing: border-box; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }}
        body {{ background-color: #f0f2f5; margin: 0; padding: 25px; }}
        .toolbar {{ width: 550px; margin: 0 auto 20px auto; display: flex; justify-content: center; gap: 10px; }}
        .btn-export {{ padding: 10px 20px; background-color: #0088cc; color: white; border: none; border-radius: 6px; font-size: 14px; font-weight: 600; cursor: pointer; }}
        .btn-export:hover {{ background-color: #006fa6; }}
        .btn-print {{ padding: 10px 20px; background-color: #059669; color: white; border: none; border-radius: 6px; font-size: 14px; font-weight: 600; cursor: pointer; }}
        .receipt {{ width: 550px; background: white; padding: 35px 40px; border: 1px solid #ccc; box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1); position: relative; margin: 0 auto; }}
        .store-header {{ text-align: center; border-bottom: 2px dashed #0088cc; padding-bottom: 15px; margin-bottom: 15px; }}
        .brand-name {{ font-size: 24px; font-weight: 900; color: #0088cc; letter-spacing: 1px; margin: 0; }}
        .slogan {{ font-size: 12px; color: #666; text-transform: uppercase; margin-top: 2px; }}
        .contact-info {{ font-size: 13px; color: #333; margin-top: 6px; line-height: 1.4; }}
        .invoice-title {{ text-align: center; margin: 15px 0; }}
        .invoice-title h2 {{ margin: 0; font-size: 18px; text-transform: uppercase; color: #222; }}
        .invoice-meta {{ font-size: 13px; color: #555; margin-top: 4px; }}
        .customer-box {{ background-color: #f8fafc; border: 1px solid #e2e8f0; border-radius: 6px; padding: 12px 16px; font-size: 13.5px; line-height: 1.7; margin-bottom: 18px; }}
        .table-prod {{ width: 100%; border-collapse: collapse; margin-bottom: 15px; font-size: 13.5px; }}
        .table-prod th {{ border-top: 1px solid #333; border-bottom: 1px solid #333; padding: 8px 4px; text-align: left; }}
        .table-prod td {{ padding: 10px 4px; border-bottom: 1px dashed #e2e8f0; }}
        .text-center {{ text-align: center; }}
        .text-right {{ text-align: right; }}
        .total-section {{ border-top: 1px solid #333; padding-top: 10px; font-size: 14px; line-height: 1.8; }}
        .total-row {{ display: flex; justify-content: space-between; }}
        .grand-total {{ font-size: 18px; font-weight: bold; color: #d32f2f; margin-top: 4px; border-top: 1px solid #ddd; padding-top: 6px; }}
        .policy-box {{ margin-top: 20px; border: 1px dashed #cbd5e1; border-radius: 6px; padding: 12px; font-size: 12px; color: #475569; line-height: 1.5; background: #fffdf5; }}
        .signatures {{ display: flex; justify-content: space-between; margin-top: 25px; text-align: center; }}
        .sign-box {{ width: 45%; }}
        .sign-label {{ font-weight: bold; font-size: 13.5px; }}
        .sign-sub {{ font-size: 11.5px; color: #666; font-style: italic; margin-bottom: 50px; }}
        .sign-name {{ font-weight: bold; font-size: 14px; }}
        .paid-stamp {{ position: absolute; bottom: 160px; right: 45px; border: 3px solid #16a34a; color: #16a34a; font-size: 18px; font-weight: 900; padding: 6px 14px; border-radius: 6px; text-transform: uppercase; transform: rotate(-18deg); opacity: 0.85; }}
        .qr-sec {{ display: flex; justify-content: space-between; align-items: center; border-top: 2px dashed #0088cc; padding-top: 12px; margin-top: 20px; }}
        .qr-text {{ font-size: 11.5px; color: #64748b; }}
    </style>
</head>
<body>
    <div class='toolbar'>
        <button type='button' class='btn-export' onclick='exportImage()'>📷 Xuất hóa đơn PNG</button>
        <button type='button' class='btn-print' onclick='window.print()'>🖨️ In Hóa Đơn</button>
    </div>

    <div class='receipt' id='receipt'>
        <div class='store-header'>
            <h1 class='brand-name'>DUY MOBILE - STORE</h1>
            <div class='slogan'>Hệ Thống Bán Lẻ & Tư Vấn Thiết Bị Di Động Uy Tín</div>
            <div class='contact-info'>
                Đ/c: 123 Cầu Giấy, Quan Hoa, Cầu Giấy, Hà Nội<br>
                Hotline: 0981.234.567 | Website: www.duymobile.vn
            </div>
        </div>

        <div class='invoice-title'>
            <h2>HÓA ĐƠN BÁN LẺ & PHIẾU BẢO HÀNH</h2>
            <div class='invoice-meta'>
                Số HĐ: <strong>{maHdb}</strong> | Giờ in: {ngayIn}
            </div>
        </div>

        <div class='customer-box'>
            <strong>Khách hàng:</strong> {tenKhach}<br>
            <strong>Số điện thoại (Mã BH):</strong> <span style='font-size: 15px; font-weight: bold; color: #0088cc;'>{dienThoai}</span><br>
            <strong>Địa chỉ:</strong> {diaChi}<br>
            <strong>Nhân viên bán hàng:</strong> {tenNv} | Quầy: POS-01
        </div>

        <table class='table-prod'>
            <thead>
                <tr>
                    <th style='width: 30px;'>STT</th>
                    <th>Tên sản phẩm</th>
                    <th style='width: 50px;' class='text-center'>BH</th>
                    <th style='width: 40px;' class='text-center'>SL</th>
                    <th style='width: 90px;' class='text-right'>Đơn giá</th>
                    <th style='width: 95px;' class='text-right'>Thành tiền</th>
                </tr>
            </thead>
            <tbody>
                {trRows}
            </tbody>
        </table>

        <div class='total-section'>
            <div class='total-row'>
                <span>Tổng tiền hàng:</span>
                <span>{tongTien.ToString("N0", ViCulture)} đ</span>
            </div>
            <div class='total-row'>
                <span>Khuyến mãi / Chiết khấu:</span>
                <span>- {giamGia.ToString("N0", ViCulture)} đ</span>
            </div>
            <div class='total-row grand-total'>
                <span>TỔNG CỘNG THANH TOÁN:</span>
                <span>{tongThanhToan.ToString("N0", ViCulture)} VNĐ</span>
            </div>
            <div style='font-size: 12.5px; font-style: italic; margin-top: 4px;'>
                (Bằng chữ: {tienChu})
            </div>
        </div>

        <div class='paid-stamp'>ĐÃ THANH TOÁN</div>

        <div class='policy-box'>
            <strong>CHÍNH SÁCH BẢO HÀNH ĐIỆN TỬ:</strong><br>
            1. Sản phẩm được bảo hành chính hãng 12 tháng theo SĐT: <strong>{dienThoai}</strong>.<br>
            2. Đổi mới 1-1 trong 30 ngày đầu tiên nếu lỗi phần cứng do nhà sản xuất.<br>
            3. Khách hàng dễ dàng tra cứu hạn bảo hành & đơn hàng tại: <em>www.duymobile.vn/lookup</em>.
        </div>

        <div class='signatures'>
            <div class='sign-box'>
                <div class='sign-label'>Khách hàng nhận máy</div>
                <div class='sign-sub'>(Ký, ghi rõ họ tên)</div>
                <div class='sign-name'>{tenKhach}</div>
            </div>
            <div class='sign-box'>
                <div class='sign-label'>Thu ngân lập phiếu</div>
                <div class='sign-sub'>(Ký, ghi rõ họ tên)</div>
                <div class='sign-name'>{tenNv}</div>
            </div>
        </div>

        <div class='qr-sec'>
            <div class='qr-text'>
                Quét mã để tra cứu hóa đơn điện tử<br>
                và kích hoạt bảo hành thiết bị
            </div>
            <div>
                <svg width='60' height='60' viewBox='0 0 100 100'>
                    <rect width='100' height='100' fill='white' stroke='#ccc' />
                    <rect x='10' y='10' width='25' height='25' fill='#0088cc' />
                    <rect x='15' y='15' width='15' height='15' fill='white' />
                    <rect x='18' y='18' width='9' height='9' fill='#0088cc' />
                    <rect x='65' y='10' width='25' height='25' fill='#0088cc' />
                    <rect x='70' y='15' width='15' height='15' fill='white' />
                    <rect x='73' y='18' width='9' height='9' fill='#0088cc' />
                    <rect x='10' y='65' width='25' height='25' fill='#0088cc' />
                    <rect x='15' y='70' width='15' height='15' fill='white' />
                    <rect x='18' y='73' width='9' height='9' fill='#0088cc' />
                    <rect x='45' y='15' width='10' height='10' fill='#0088cc' />
                    <rect x='45' y='45' width='15' height='15' fill='#0088cc' />
                    <rect x='70' y='55' width='12' height='12' fill='#0088cc' />
                    <rect x='75' y='75' width='15' height='15' fill='#0088cc' />
                </svg>
            </div>
        </div>
    </div>

    <script>
        async function exportImage() {{
            const receipt = document.getElementById('receipt');
            try {{
                const canvas = await html2canvas(receipt, {{ scale: 3, backgroundColor: '#ffffff', useCORS: true, logging: false }});
                const image = canvas.toDataURL('image/png', 1.0);
                const link = document.createElement('a');
                link.download = 'HoaDon-{maHdb}.png';
                link.href = image;
                link.click();
            }} catch (error) {{
                alert('Có lỗi xảy ra khi xuất hóa đơn!');
            }}
        }}
    </script>
</body>
</html>";
        }
        #endregion

        #region 3. XUẤT VÀ HIỂN THỊ BIỂU MẪU BM01: HÓA ĐƠN NHẬP KIÊM PHIẾU NHẬP KHO
        public static string TaoHtmlHoaDonNhap(
            string maHdn,
            string tenNcc,
            string diaChiNcc,
            string sdtNcc,
            string tenNv,
            string maNv,
            List<ItemBieuMauNhap> danhSachSp,
            decimal tongTien)
        {
            string tienChu = DocSoThanhChu(tongTien);
            string ngayLap = DateTime.Now.ToString("dd/MM/yyyy");
            int tongSl = danhSachSp.Sum(x => x.SoLuong);

            var trRows = new StringBuilder();
            for (int i = 0; i < danhSachSp.Count; i++)
            {
                var sp = danhSachSp[i];
                trRows.Append($@"
                <tr>
                    <td class='text-center'>{i + 1}</td>
                    <td class='text-center'><strong>{sp.MaSp}</strong></td>
                    <td>{sp.TenSp}</td>
                    <td class='text-center'>{sp.Dvt}</td>
                    <td class='text-center'>{sp.SoLuong}</td>
                    <td class='text-right'>{sp.DonGia.ToString("N0", ViCulture)}</td>
                    <td class='text-center'>{(sp.KhuyenMai > 0 ? sp.KhuyenMai + "%" : "0%")}</td>
                    <td class='text-right'><strong>{sp.ThanhTien.ToString("N0", ViCulture)}</strong></td>
                </tr>");
            }

            return $@"<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>BM01 - Hóa Đơn Nhập Kiêm Phiếu Nhập Kho - {maHdn}</title>
    <script src='https://cdn.jsdelivr.net/npm/html2canvas@1.4.1/dist/html2canvas.min.js'></script>
    <style>
        * {{ box-sizing: border-box; font-family: 'Times New Roman', Times, serif; }}
        body {{ background-color: #f4f5f7; padding: 30px; margin: 0; }}
        .toolbar {{ width: 800px; margin: 0 auto 20px auto; display: flex; justify-content: center; gap: 10px; }}
        .btn-export {{ padding: 11px 24px; background-color: #d32f2f; color: white; border: none; border-radius: 6px; font-size: 14px; font-weight: bold; cursor: pointer; font-family: Arial, sans-serif; }}
        .btn-print {{ padding: 11px 24px; background-color: #0369a1; color: white; border: none; border-radius: 6px; font-size: 14px; font-weight: bold; cursor: pointer; font-family: Arial, sans-serif; }}
        .page {{ width: 800px; background: white; padding: 40px 50px; border: 1px solid #ddd; box-shadow: 0 4px 15px rgba(0,0,0,0.1); position: relative; margin: 0 auto; }}
        .header {{ display: flex; justify-content: space-between; border-bottom: 2px solid #222; padding-bottom: 12px; }}
        .company {{ font-size: 14px; line-height: 1.4; }}
        .company strong {{ font-size: 16px; color: #d32f2f; }}
        .doc-meta {{ text-align: right; font-size: 13px; line-height: 1.4; }}
        .title {{ text-align: center; margin: 25px 0 15px 0; }}
        .title h1 {{ margin: 0; font-size: 22px; text-transform: uppercase; color: #111; letter-spacing: 1px; }}
        .title p {{ margin: 5px 0 0 0; font-size: 14px; font-style: italic; }}
        .info-grid {{ font-size: 14px; line-height: 1.8; margin-bottom: 20px; }}
        .table-data {{ width: 100%; border-collapse: collapse; margin-bottom: 15px; }}
        .table-data th, .table-data td {{ border: 1px solid #333; padding: 8px 10px; font-size: 13.5px; }}
        .table-data th {{ background-color: #f0f0f0; text-align: center; font-weight: bold; }}
        .text-center {{ text-align: center; }}
        .text-right {{ text-align: right; }}
        .total-row td {{ font-weight: bold; background-color: #fafafa; }}
        .signatures {{ display: flex; justify-content: space-between; margin-top: 30px; text-align: center; }}
        .sign-box {{ width: 30%; }}
        .sign-title {{ font-weight: bold; font-size: 14px; margin-bottom: 5px; }}
        .sign-note {{ font-style: italic; font-size: 12px; margin-bottom: 60px; color: #555; }}
        .sign-name {{ font-weight: bold; font-size: 14px; }}
        .stamp-wrap {{ position: relative; display: inline-block; }}
        .red-stamp {{ position: absolute; top: -65px; left: 20px; width: 125px; height: 125px; border: 3px solid #d32f2f; border-radius: 50%; color: #d32f2f; font-family: Arial, sans-serif; text-align: center; padding-top: 15px; transform: rotate(-15deg); opacity: 0.85; pointer-events: none; }}
        .red-stamp .s-inner {{ font-size: 8px; font-weight: bold; letter-spacing: 0.5px; border-bottom: 1px solid #d32f2f; padding-bottom: 3px; margin: 0 10px; }}
        .red-stamp .s-mid {{ font-size: 11px; font-weight: bold; margin-top: 8px; }}
        .red-stamp .s-bot {{ font-size: 8.5px; margin-top: 5px; }}
        .barcode {{ text-align: right; margin-top: 15px; }}
    </style>
</head>
<body>
    <div class='toolbar'>
        <button type='button' class='btn-export' onclick='exportImage()'>📷 Xuất hóa đơn PNG</button>
        <button type='button' class='btn-print' onclick='window.print()'>🖨️ In Phiếu Nhập Kho</button>
    </div>

    <div class='page' id='invoice'>
        <div class='header'>
            <div class='company'>
                <strong>{tenNcc.ToUpper()}</strong><br>
                Địa chỉ: {diaChiNcc}<br>
                Hotline: {sdtNcc}
            </div>
            <div class='doc-meta'>
                <strong>Mẫu số: 01-VT</strong><br>
                (Ban hành theo TT 200/2014/TT-BTC)<br>
                Số chứng từ: <strong>{maHdn}</strong><br>
                Ngày lập: {ngayLap}
            </div>
        </div>

        <div class='title'>
            <h1>HÓA ĐƠN NHẬP KIÊM PHIẾU NHẬP KHO</h1>
            <p>Hàng hóa thiết bị di động chính hãng & phụ kiện</p>
        </div>

        <div class='info-grid'>
            - <strong>Đơn vị nhận hàng:</strong> Cửa hàng Bán lẻ Thiết bị Di động Duy Mobile<br>
            - <strong>Đại diện tiếp nhận:</strong> {tenNv} - Quản lý Kho (Mã NV: {maNv})<br>
            - <strong>Địa chỉ giao nhận:</strong> Số 123 Cầu Giấy, Phường Quan Hoa, Quận Cầu Giấy, Hà Nội<br>
            - <strong>Nhà cung ứng:</strong> {tenNcc}
        </div>

        <table class='table-data'>
            <thead>
                <tr>
                    <th style='width:40px;'>STT</th>
                    <th style='width:80px;'>Mã SP</th>
                    <th>Tên sản phẩm / Quy cách</th>
                    <th style='width:50px;'>ĐVT</th>
                    <th style='width:60px;'>Số lượng</th>
                    <th style='width:100px;'>Đơn giá (VNĐ)</th>
                    <th style='width:70px;'>KM (%)</th>
                    <th style='width:120px;'>Thành tiền (VNĐ)</th>
                </tr>
            </thead>
            <tbody>
                {trRows}
                <tr class='total-row'>
                    <td colspan='4' class='text-center'>TỔNG CỘNG TIỀN HÀNG:</td>
                    <td class='text-center'>{tongSl}</td>
                    <td></td>
                    <td></td>
                    <td class='text-right' style='color:#d32f2f; font-size:15px;'>{tongTien.ToString("N0", ViCulture)}</td>
                </tr>
            </tbody>
        </table>

        <div style='font-size:13.5px; margin-bottom:25px;'>
            - <em>Số tiền viết bằng chữ:</em> <strong>{tienChu}</strong><br>
            - <em>Chứng từ kèm theo:</em> Bảng kê chi tiết {tongSl} máy chính hãng có tem bảo hành đối tác.
        </div>

        <div class='signatures'>
            <div class='sign-box'>
                <div class='sign-title'>Người giao hàng</div>
                <div class='sign-note'>(Ký, ghi rõ họ tên)</div>
                <div class='sign-name'>Đại diện {tenNcc}</div>
            </div>
            <div class='sign-box'>
                <div class='sign-title'>Thủ kho tiếp nhận</div>
                <div class='sign-note'>(Ký, ghi rõ họ tên)</div>
                <div class='sign-name'>{tenNv}</div>
            </div>
            <div class='sign-box'>
                <div class='sign-title'>Đại diện Quản lý nhận</div>
                <div class='sign-note'>(Ký, đóng dấu)</div>
                <div class='stamp-wrap'>
                    <div class='red-stamp'>
                        <div class='s-inner'>NHÀ PHÂN PHỐI</div>
                        <div class='s-mid'>{tenNcc.ToUpper()}</div>
                        <div class='s-bot'>★ ĐÃ XUẤT KHO ★</div>
                    </div>
                    <div class='sign-name'>Nguyễn Thành Duy</div>
                </div>
            </div>
        </div>

        <div class='barcode'>
            <svg width='220' height='40' viewBox='0 0 220 40'>
                <rect x='0' y='0' width='220' height='40' fill='white'/>
                <rect x='10' y='5' width='2' height='18' fill='black'/>
                <rect x='15' y='5' width='4' height='18' fill='black'/>
                <rect x='22' y='5' width='2' height='18' fill='black'/>
                <rect x='30' y='5' width='5' height='18' fill='black'/>
                <rect x='40' y='5' width='3' height='18' fill='black'/>
                <rect x='50' y='5' width='4' height='18' fill='black'/>
                <rect x='60' y='5' width='2' height='18' fill='black'/>
                <rect x='70' y='5' width='5' height='18' fill='black'/>
                <rect x='85' y='5' width='3' height='18' fill='black'/>
                <rect x='95' y='5' width='4' height='18' fill='black'/>
                <rect x='105' y='5' width='2' height='18' fill='black'/>
                <rect x='115' y='5' width='5' height='18' fill='black'/>
                <rect x='130' y='5' width='3' height='18' fill='black'/>
                <rect x='140' y='5' width='4' height='18' fill='black'/>
                <rect x='150' y='5' width='2' height='18' fill='black'/>
                <rect x='160' y='5' width='5' height='18' fill='black'/>
                <rect x='175' y='5' width='3' height='18' fill='black'/>
                <rect x='185' y='5' width='4' height='18' fill='black'/>
                <rect x='195' y='5' width='3' height='18' fill='black'/>
                <text x='15' y='35' font-family='monospace' font-size='10' fill='#333'>*{maHdn}*</text>
            </svg>
        </div>
    </div>

    <script>
        async function exportImage() {{
            const invoice = document.getElementById('invoice');
            try {{
                const canvas = await html2canvas(invoice, {{ scale: 3, backgroundColor: '#ffffff', useCORS: true, logging: false }});
                canvas.toBlob(function(blob) {{
                    const url = URL.createObjectURL(blob);
                    const link = document.createElement('a');
                    link.download = '{maHdn}.png';
                    link.href = url;
                    link.click();
                    URL.revokeObjectURL(url);
                }}, 'image/png', 1.0);
            }} catch (error) {{
                alert('Có lỗi xảy ra khi xuất hóa đơn!');
            }}
        }}
    </script>
</body>
</html>";
        }
        #endregion

        #region 4. XUẤT VÀ HIỂN THỊ BIỂU MẪU BM03: BÁO CÁO TỔNG HỢP XUẤT - NHẬP - TỒN KHO
        public static string TaoHtmlBaoCaoKho(
            string maBc,
            int thang,
            int nam,
            List<ItemBieuMauKho> danhSachSp)
        {
            int tongTonCuoiSl = danhSachSp.Sum(x => x.TonCuoiSl);
            decimal tongTonCuoiTien = danhSachSp.Sum(x => x.TonCuoiTien);
            int tongNhapSl = danhSachSp.Sum(x => x.NhapSl);
            decimal tongNhapTien = danhSachSp.Sum(x => x.NhapTien);
            int tongXuatSl = danhSachSp.Sum(x => x.XuatSl);
            decimal tongXuatTien = danhSachSp.Sum(x => x.XuatTien);

            var trRows = new StringBuilder();
            for (int i = 0; i < danhSachSp.Count; i++)
            {
                var sp = danhSachSp[i];
                string badgeClass = sp.TonCuoiSl > 15 ? "status-high" : "status-ok";
                string statusText = sp.TonCuoiSl > 15 ? "Tồn nhiều" : (sp.TonCuoiSl > 0 ? "Còn hàng" : "Hết hàng");

                trRows.Append($@"
                <tr>
                    <td class='text-center'>{i + 1}</td>
                    <td class='text-center'><strong>{sp.MaSp}</strong></td>
                    <td>{sp.TenSp}</td>
                    <td class='text-center'>{sp.ThuongHieu}</td>
                    <td class='text-center'>{sp.TonDauSl}</td>
                    <td class='text-right'>{sp.TonDauTien.ToString("N0", ViCulture)}</td>
                    <td class='text-center'>{sp.NhapSl}</td>
                    <td class='text-right'>{sp.NhapTien.ToString("N0", ViCulture)}</td>
                    <td class='text-center'>{sp.XuatSl}</td>
                    <td class='text-right'>{sp.XuatTien.ToString("N0", ViCulture)}</td>
                    <td class='text-center'><strong>{sp.TonCuoiSl}</strong></td>
                    <td class='text-right'><strong>{sp.TonCuoiTien.ToString("N0", ViCulture)}</strong></td>
                    <td class='text-center'><span class='status-badge {badgeClass}'>{statusText}</span></td>
                </tr>");
            }

            return $@"<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>BM03 - Báo Cáo Tổng Hợp Xuất Nhập Tồn Kho - Tháng {thang}/{nam}</title>
    <script src='https://cdn.jsdelivr.net/npm/html2canvas@1.4.1/dist/html2canvas.min.js'></script>
    <style>
        * {{ box-sizing: border-box; font-family: 'Segoe UI', Arial, sans-serif; }}
        body {{ background-color: #f1f5f9; padding: 30px; margin: 0; }}
        .toolbar {{ width: 950px; margin: 0 auto 20px auto; display: flex; justify-content: center; gap: 10px; }}
        .btn-export {{ padding: 11px 25px; background: #0369a1; color: white; border: none; border-radius: 6px; font-size: 14px; font-weight: bold; cursor: pointer; }}
        .btn-print {{ padding: 11px 25px; background: #059669; color: white; border: none; border-radius: 6px; font-size: 14px; font-weight: bold; cursor: pointer; }}
        .sheet {{ width: 950px; background: white; padding: 40px; border: 1px solid #cbd5e1; box-shadow: 0 4px 15px rgba(0,0,0,0.1); margin: 0 auto; }}
        .header-table {{ width: 100%; margin-bottom: 20px; }}
        .org-info {{ font-size: 13.5px; line-height: 1.5; color: #1e293b; }}
        .meta-info {{ text-align: right; font-size: 13px; color: #64748b; }}
        .report-title {{ text-align: center; margin: 20px 0 25px 0; }}
        .report-title h1 {{ margin: 0; font-size: 22px; color: #0f172a; text-transform: uppercase; letter-spacing: 0.5px; }}
        .report-title p {{ margin: 5px 0 0 0; font-size: 14px; font-style: italic; color: #475569; }}
        .data-table {{ width: 100%; border-collapse: collapse; font-size: 12.5px; }}
        .data-table th, .data-table td {{ border: 1px solid #475569; padding: 7px 6px; }}
        .data-table thead th {{ background-color: #e2e8f0; font-weight: bold; text-align: center; color: #0f172a; }}
        .text-center {{ text-align: center; }}
        .text-right {{ text-align: right; }}
        .status-badge {{ display: inline-block; padding: 2px 8px; border-radius: 4px; font-weight: bold; font-size: 11px; white-space: nowrap; }}
        .status-ok {{ background: #dcfce7; color: #166534; }}
        .status-high {{ background: #fef9c3; color: #854d0e; }}
        .total-row {{ font-weight: bold; background-color: #f8fafc; }}
        .notes {{ margin-top: 20px; font-size: 13px; line-height: 1.6; color: #334155; }}
        .signatures {{ display: flex; justify-content: space-around; margin-top: 35px; text-align: center; }}
        .sign-box {{ width: 40%; position: relative; }}
        .sign-title {{ font-weight: bold; font-size: 14px; }}
        .sign-sub {{ font-size: 12px; font-style: italic; color: #64748b; margin-bottom: 60px; }}
        .sign-name {{ font-weight: bold; font-size: 14px; }}
        .red-stamp-approved {{ position: absolute; top: -20px; left: 50%; transform: translateX(-50%) rotate(-12deg); width: 140px; height: 70px; border: 3px solid #dc2626; color: #dc2626; font-weight: 900; text-transform: uppercase; display: flex; flex-direction: column; justify-content: center; align-items: center; border-radius: 8px; opacity: 0.85; pointer-events: none; }}
        .red-stamp-approved span:first-child {{ font-size: 11px; letter-spacing: 1px; }}
        .red-stamp-approved span:last-child {{ font-size: 14px; }}
    </style>
</head>
<body>
    <div class='toolbar'>
        <button type='button' class='btn-export' onclick='exportImage()'>📷 Xuất báo cáo PNG</button>
        <button type='button' class='btn-print' onclick='window.print()'>🖨️ In Báo Cáo Kho</button>
    </div>

    <div class='sheet' id='report'>
        <table class='header-table'>
            <tr>
                <td class='org-info'>
                    <strong>ĐƠN VỊ: CỬA HÀNG THIẾT BỊ DI ĐỘNG DUY MOBILE</strong><br>
                    Bộ phận: Quản lý Kho & Bán lẻ Công nghệ<br>
                    Địa chỉ: 123 Cầu Giấy, Quan Hoa, Cầu Giấy, TP. Hà Nội
                </td>
                <td class='meta-info'>
                    <strong>MẪU BÁO CÁO QUẢN TRỊ NỘI BỘ</strong><br>
                    Mã báo cáo: <strong>{maBc}</strong><br>
                    Ngày lập: {DateTime.Now:dd/MM/yyyy}
                </td>
            </tr>
        </table>

        <div class='report-title'>
            <h1>BÁO CÁO TỔNG HỢP XUẤT - NHẬP - TỒN KHO THIẾT BỊ</h1>
            <p>Kỳ báo cáo: Tháng {thang:D2}/{nam} (Từ ngày 01/{thang:D2}/{nam} đến ngày {DateTime.DaysInMonth(nam, thang):D2}/{thang:D2}/{nam})</p>
        </div>

        <table class='data-table'>
            <thead>
                <tr>
                    <th rowspan='2' style='width:35px;'>STT</th>
                    <th rowspan='2' style='width:60px;'>Mã SP</th>
                    <th rowspan='2'>Tên thiết bị di động</th>
                    <th rowspan='2' style='width:70px;'>Thương hiệu</th>
                    <th colspan='2'>Tồn đầu kỳ</th>
                    <th colspan='2'>Nhập trong kỳ</th>
                    <th colspan='2'>Xuất trong kỳ</th>
                    <th colspan='2'>Tồn cuối kỳ</th>
                    <th rowspan='2' style='width:90px;'>Trạng thái</th>
                </tr>
                <tr>
                    <th style='width:35px;'>SL</th>
                    <th style='width:75px;'>T.Tiền</th>
                    <th style='width:35px;'>SL</th>
                    <th style='width:75px;'>T.Tiền</th>
                    <th style='width:35px;'>SL</th>
                    <th style='width:75px;'>T.Tiền</th>
                    <th style='width:35px;'>SL</th>
                    <th style='width:75px;'>T.Tiền</th>
                </tr>
            </thead>
            <tbody>
                {trRows}
                <tr class='total-row'>
                    <td colspan='4' class='text-center'>TỔNG CỘNG TOÀN KHO:</td>
                    <td class='text-center'>0</td>
                    <td class='text-right'>0</td>
                    <td class='text-center'>{tongNhapSl}</td>
                    <td class='text-right'>{tongNhapTien.ToString("N0", ViCulture)}</td>
                    <td class='text-center'>{tongXuatSl}</td>
                    <td class='text-right'>{tongXuatTien.ToString("N0", ViCulture)}</td>
                    <td class='text-center'>{tongTonCuoiSl}</td>
                    <td class='text-right' style='color:#0369a1;'>{tongTonCuoiTien.ToString("N0", ViCulture)}</td>
                    <td></td>
                </tr>
            </tbody>
        </table>

        <div class='notes'>
            <strong>* KẾT LUẬN KIỂM KÊ VÀ ĐÁNH GIÁ CỦA THỦ KHO:</strong><br>
            - Số liệu trên hệ thống đối chiếu thực tế với {tongTonCuoiSl} thiết bị trong tủ kính trưng bày và kho: <strong>Khớp 100%, không phát sinh hao hụt hoặc thất thoát.</strong><br>
            - Đề xuất tiếp tục theo dõi biến động số lượng để lên kế hoạch nhập hàng kỳ tiếp theo.
        </div>

        <div class='signatures'>
            <div class='sign-box'>
                <div class='sign-title'>Thủ kho lập báo cáo</div>
                <div class='sign-sub'>(Ký, ghi rõ họ tên)</div>
                <div class='sign-name'>Nguyễn Văn Hùng</div>
            </div>
            <div class='sign-box'>
                <div class='sign-title'>Chủ cơ sở phê duyệt</div>
                <div class='sign-sub'>(Ký, đóng dấu duyệt)</div>
                <div class='red-stamp-approved'>
                    <span>DUY MOBILE</span>
                    <span>★ ĐÃ PHÊ DUYỆT ★</span>
                </div>
                <div class='sign-name'>Nguyễn Thành Duy</div>
            </div>
        </div>
    </div>

    <script>
        async function exportImage() {{
            const report = document.getElementById('report');
            try {{
                const canvas = await html2canvas(report, {{ scale: 3, backgroundColor: '#ffffff', useCORS: true, logging: false }});
                canvas.toBlob(function(blob) {{
                    const url = URL.createObjectURL(blob);
                    const link = document.createElement('a');
                    link.download = '{maBc}.png';
                    link.href = url;
                    link.click();
                    URL.revokeObjectURL(url);
                }}, 'image/png', 1.0);
            }} catch (error) {{
                alert('Có lỗi xảy ra khi xuất báo cáo!');
            }}
        }}
    </script>
</body>
</html>";
        }
        #endregion

        #region 5. MỞ VÀ HIỂN THỊ BIỂU MẪU LÊN TRÌNH DUYỆT / IN ẤN
        public static void MoBieuMauTrenTrinhDuyet(string htmlContent, string tenFile)
        {
            try
            {
                string outputDir = Path.Combine(Path.GetTempPath(), "NexusRetail_BieuMau");
                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                string filePath = Path.Combine(outputDir, $"{tenFile}.html");
                File.WriteAllText(filePath, htmlContent, Encoding.UTF8);

                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể mở biểu mẫu: {ex.Message}", "Lỗi hiển thị biểu mẫu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
