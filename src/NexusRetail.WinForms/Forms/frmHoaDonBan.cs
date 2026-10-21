using NexusRetail.Shared.DTOs.BanHang;
using NexusRetail.Shared.DTOs.SanPham;
using NexusRetail.WinForms.Services;
using System.Data;

namespace NexusRetail.WinForms.Forms
{
    public partial class frmHoaDonBan : Form
    {
        private readonly string _nhanVien;
        private List<SanPhamDto> _products = new();
        private readonly DataTable _cartTable = new();

        public frmHoaDonBan(string nhanVien)
        {
            _nhanVien = nhanVien;
            InitializeComponent();

            txtNhanVien.Text = _nhanVien;
            InitCartTable();
            LoadProductsAsync();
        }

        private void InitCartTable()
        {
            _cartTable.Columns.Add("MaSp", typeof(string));
            _cartTable.Columns.Add("TenSp", typeof(string));
            _cartTable.Columns.Add("SoLuong", typeof(int));
            _cartTable.Columns.Add("DonGia", typeof(decimal));
            _cartTable.Columns.Add("KhuyenMai", typeof(decimal));
            _cartTable.Columns.Add("ThanhTien", typeof(decimal));

            dgvChiTiet.DataSource = _cartTable;

            if (dgvChiTiet.Columns["MaSp"] != null) dgvChiTiet.Columns["MaSp"].HeaderText = "Mã SP";
            if (dgvChiTiet.Columns["TenSp"] != null) dgvChiTiet.Columns["TenSp"].HeaderText = "Tên Sản Phẩm";
            if (dgvChiTiet.Columns["SoLuong"] != null) dgvChiTiet.Columns["SoLuong"].HeaderText = "Số Lượng";
            if (dgvChiTiet.Columns["DonGia"] != null)
            {
                dgvChiTiet.Columns["DonGia"].HeaderText = "Đơn Giá";
                dgvChiTiet.Columns["DonGia"].DefaultCellStyle.Format = "#,##0 ₫";
            }
            if (dgvChiTiet.Columns["KhuyenMai"] != null)
            {
                dgvChiTiet.Columns["KhuyenMai"].HeaderText = "Khuyến Mãi";
                dgvChiTiet.Columns["KhuyenMai"].DefaultCellStyle.Format = "#,##0 ₫";
            }
            if (dgvChiTiet.Columns["ThanhTien"] != null)
            {
                dgvChiTiet.Columns["ThanhTien"].HeaderText = "Thành Tiền";
                dgvChiTiet.Columns["ThanhTien"].DefaultCellStyle.Format = "#,##0 ₫";
            }

            dgvChiTiet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private async void LoadProductsAsync()
        {
            _products = await WinFormsApiClient.Instance.GetProductsAsync();
            cboSanPham.DataSource = _products;
            cboSanPham.DisplayMember = "TenSp";
            cboSanPham.ValueMember = "MaSp";

            if (_products.Count > 0)
            {
                cboSanPham_SelectedIndexChanged(this, EventArgs.Empty);
            }
        }

        private void cboSanPham_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cboSanPham.SelectedItem is SanPhamDto sp)
            {
                txtDonGia.Text = $"{sp.GiaBan:N0} ₫";
                lblTonKho.Text = $"Tồn kho: {sp.SoLuong} máy";
                lblTonKho.ForeColor = sp.SoLuong > 0 ? Color.FromArgb(16, 185, 129) : Color.Red;
            }
        }

        private void btnThemMon_Click(object sender, EventArgs e)
        {
            if (cboSanPham.SelectedItem is not SanPhamDto sp) return;

            int sl = (int)numSoLuong.Value;
            decimal km = numKhuyenMai.Value;

            if (sl <= 0)
            {
                MessageBox.Show("Số lượng mua phải lớn hơn 0!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (sp.SoLuong < sl)
            {
                MessageBox.Show($"Sản phẩm '{sp.TenSp}' chỉ còn {sp.SoLuong} máy trong kho!", "Không đủ tồn kho", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra xem sản phẩm đã có trong bảng chưa
            DataRow? existing = _cartTable.AsEnumerable().FirstOrDefault(r => r.Field<string>("MaSp") == sp.MaSp);
            if (existing != null)
            {
                int currentSl = existing.Field<int>("SoLuong");
                int newSl = currentSl + sl;
                if (newSl > sp.SoLuong)
                {
                    MessageBox.Show($"Tổng số lượng đặt ({newSl}) vượt quá tồn kho ({sp.SoLuong})!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                existing["SoLuong"] = newSl;
                existing["ThanhTien"] = (newSl * sp.GiaBan) - km;
            }
            else
            {
                decimal thanhTien = (sl * sp.GiaBan) - km;
                _cartTable.Rows.Add(sp.MaSp, sp.TenSp, sl, sp.GiaBan, km, Math.Max(0, thanhTien));
            }

            UpdateTotalAmount();
        }

        private void btnXoaMon_Click(object sender, EventArgs e)
        {
            if (dgvChiTiet.CurrentRow != null)
            {
                dgvChiTiet.Rows.Remove(dgvChiTiet.CurrentRow);
                UpdateTotalAmount();
            }
        }

        private decimal CalculateTotal()
        {
            decimal sum = 0m;
            foreach (DataRow row in _cartTable.Rows)
            {
                sum += Convert.ToDecimal(row["ThanhTien"]);
            }
            return sum;
        }

        private void UpdateTotalAmount()
        {
            decimal total = CalculateTotal();
            lblTongTien.Text = $"{total:N0} ₫";
        }

        // 1. THANH TOÁN TIỀN MẶT
        private async void btnThanhToanTienMat_Click(object sender, EventArgs e)
        {
            await ProcessPaymentAsync(isBankTransfer: false);
        }

        // 2. THANH TOÁN CHUYỂN KHOẢN VIETQR (MBBANK WEBHOOK TỰ ĐỘNG)
        private async void btnThanhToanQR_Click(object sender, EventArgs e)
        {
            decimal total = CalculateTotal();
            if (total <= 0 || _cartTable.Rows.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống! Vui lòng chọn sản phẩm vào đơn trước.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Sinh mã hóa đơn tạm để chuyển khoản (ví dụ HDB01)
            string maHdbTemp = $"HDB{DateTime.Now:HHmmss}";

            // Mở Popup VietQR MBBank lắng nghe Webhook tự động
            using var qrForm = new frmThanhToanQR(maHdbTemp, total);
            var result = qrForm.ShowDialog(this);

            if (result == DialogResult.OK && qrForm.IsPaymentSuccessful)
            {
                // Webhook đã báo tiền vào tài khoản ➔ Hoàn tất hóa đơn trên hệ thống
                await ProcessPaymentAsync(isBankTransfer: true, maHdbGiaoDich: maHdbTemp);
            }
            else
            {
                MessageBox.Show("Giao dịch chuyển khoản QR chưa hoàn tất hoặc đã bị hủy.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async Task ProcessPaymentAsync(bool isBankTransfer, string? maHdbGiaoDich = null)
        {
            decimal total = CalculateTotal();
            if (total <= 0 || _cartTable.Rows.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dto = new HoaDonBanDto
            {
                MaNv = _nhanVien,
                TenKhachVangLai = string.IsNullOrWhiteSpace(txtTenKhach.Text) ? "Khách Mua Tại Quầy" : txtTenKhach.Text.Trim(),
                DienThoai = string.IsNullOrWhiteSpace(txtDienThoai.Text) ? "0900000000" : txtDienThoai.Text.Trim(),
                DanhSachMon = _cartTable.AsEnumerable().Select(r => new ChiTietHdbDto
                {
                    MaSp = r.Field<string>("MaSp") ?? "",
                    TenSp = r.Field<string>("TenSp") ?? "",
                    SoLuong = r.Field<int>("SoLuong"),
                    KhuyenMai = r.Field<decimal>("KhuyenMai")
                }).ToList()
            };

            var (success, message, maHdb, _) = await WinFormsApiClient.Instance.CreateSaleOrderAsync(dto);

            if (success)
            {
                string effectiveMaHdb = !string.IsNullOrWhiteSpace(maHdb) ? maHdb : (!string.IsNullOrWhiteSpace(maHdbGiaoDich) ? maHdbGiaoDich : $"HDB-{DateTime.Now:yyyy-MMdd-HHmm}");

                // Trích xuất danh sách sản phẩm sang dữ liệu biểu mẫu BM02
                var bieuMauItems = _cartTable.AsEnumerable().Select((r, idx) =>
                {
                    string msp = r.Field<string>("MaSp") ?? "";
                    string tsp = r.Field<string>("TenSp") ?? "";
                    int sl = r.Field<int>("SoLuong");
                    decimal dg = r.Field<decimal>("DonGia");
                    decimal tt = r.Field<decimal>("ThanhTien");

                    var prod = _products.FirstOrDefault(p => p.MaSp == msp);
                    string quyCach = prod != null ? $"{prod.TenLoai} - {prod.TenNhanHieu}" : "Chính hãng (VN/A)";
                    string bh = prod != null && prod.ThoiGianBaoHanh > 0 ? $"{prod.ThoiGianBaoHanh}T" : "12T";
                    string imei = $"IMEI: 3589{idx + 1:D2}{DateTime.Now:MMddHHmm}";

                    return new ItemBieuMauBan
                    {
                        Stt = idx + 1,
                        MaSp = msp,
                        TenSp = tsp,
                        QuyCach = quyCach,
                        Imei = imei,
                        BaoHanh = bh,
                        SoLuong = sl,
                        DonGia = dg,
                        ThanhTien = tt
                    };
                }).ToList();

                decimal tongTienHang = bieuMauItems.Sum(x => x.DonGia * x.SoLuong);
                decimal tongGiamGia = tongTienHang - total;
                if (tongGiamGia < 0) tongGiamGia = 0;

                string html = BieuMauService.TaoHtmlHoaDonBan(
                    maHdb: effectiveMaHdb,
                    tenKhach: dto.TenKhachVangLai,
                    dienThoai: dto.DienThoai,
                    diaChi: "Số 45 Đống Đa, TP. Hà Nội",
                    tenNv: _nhanVien,
                    danhSachSp: bieuMauItems,
                    tongTien: tongTienHang,
                    giamGia: tongGiamGia
                );

                // Hiển thị ngay biểu mẫu BM02 trên trình duyệt để in / xuất ảnh
                BieuMauService.MoBieuMauTrenTrinhDuyet(html, $"HoaDonBan_{effectiveMaHdb}");

                string phuongThuc = isBankTransfer ? "Chuyển khoản VietQR MBBank (Webhook Tự Động)" : "Tiền mặt tại quầy";
                MessageBox.Show($"✅ THANH TOÁN THÀNH CÔNG!\n\n• Mã HĐ: {effectiveMaHdb}\n• Tổng tiền: {total:N0} ₫\n• Phương thức: {phuongThuc}\n• Biểu mẫu BM02 (Hóa đơn bán lẻ & Phiếu bảo hành) đã được hiển thị để in ấn hoặc xuất ảnh PNG.",
                    "Hóa Đơn Bán Lẻ Duy Mobile", MessageBoxButtons.OK, MessageBoxIcon.Information);

                _cartTable.Clear();
                UpdateTotalAmount();
                LoadProductsAsync();
            }
            else
            {
                MessageBox.Show(message, "Lỗi thanh toán", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
