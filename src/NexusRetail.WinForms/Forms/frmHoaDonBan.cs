using NexusRetail.Shared.DTOs.BanHang;
using NexusRetail.Shared.DTOs.SanPham;
using NexusRetail.WinForms.Services;
using NexusRetail.WinForms.UI;
using System.Data;

namespace NexusRetail.WinForms.Forms
{
    public partial class frmHoaDonBan : Form
    {
        private readonly string _nhanVien;
        private readonly IPosCatalogClient _catalogClient = WinFormsApiClient.Catalog;
        private readonly IPosSalesClient _salesClient = WinFormsApiClient.Sales;
        private List<SanPhamDto> _products = new();
        private readonly DataTable _cartTable = new();

        public frmHoaDonBan(string nhanVien)
        {
            _nhanVien = nhanVien;
            InitializeComponent();

            UiTheme.BeautifyForm(this);
            ApplyCustomStyles();

            KeyPreview = true;
            KeyDown += FrmHoaDonBan_KeyDown;

            txtNhanVien.Text = _nhanVien;
            InitCartTable();
            LoadProductsAsync();

            txtDienThoai.Leave += async (s, e) => await TraCuuKhachHangNhanhAsync();
            txtDienThoai.KeyDown += async (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    await TraCuuKhachHangNhanhAsync();
                }
            };
        }

        private async Task TraCuuKhachHangNhanhAsync()
        {
            string sdt = txtDienThoai.Text.Trim();
            if (string.IsNullOrWhiteSpace(sdt) || sdt.Length < 9) return;

            try
            {
                var orders = await _salesClient.SearchSaleInvoicesAsync(sdt);
                var existing = orders.FirstOrDefault(x => x.DienThoai == sdt && !string.IsNullOrWhiteSpace(x.TenKhach));
                if (existing != null && (string.IsNullOrWhiteSpace(txtTenKhach.Text) || txtTenKhach.Text == "Khách mua tại quầy"))
                {
                    txtTenKhach.Text = existing.TenKhach;
                }
            }
            catch { }
        }

        private void FrmHoaDonBan_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                e.Handled = true;
                cboSanPham.Focus();
            }
            else if (e.KeyCode == Keys.F3)
            {
                e.Handled = true;
                txtDienThoai.Focus();
                txtDienThoai.SelectAll();
            }
            else if (e.KeyCode == Keys.F4)
            {
                e.Handled = true;
                numSoLuong.Focus();
                numSoLuong.Select(0, numSoLuong.Text.Length);
            }
            else if (e.KeyCode == Keys.F5)
            {
                e.Handled = true;
                LoadProductsAsync();
            }
            else if (e.KeyCode == Keys.F8)
            {
                e.Handled = true;
                btnThanhToanTienMat_Click(this, EventArgs.Empty);
            }
            else if (e.KeyCode == Keys.F9)
            {
                e.Handled = true;
                btnThanhToanQR_Click(this, EventArgs.Empty);
            }
            else if (e.KeyCode == Keys.F12)
            {
                e.Handled = true;
                if (_cartTable.Rows.Count > 0)
                {
                    var res = MessageBox.Show("Xóa toàn bộ giỏ hàng hiện tại để lập đơn mới?", "Làm mới đơn", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.Yes)
                    {
                        _cartTable.Clear();
                        UpdateTotalAmount();
                    }
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                if (_cartTable.Rows.Count > 0)
                {
                    var res = MessageBox.Show("Đơn hàng đang có sản phẩm, bạn có chắc chắn muốn đóng màn hình bán hàng?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.Yes) Close();
                }
                else
                {
                    Close();
                }
            }
        }

        private void ApplyCustomStyles()
        {
            UiTheme.ApplyFormHeader(pnlTop, lblTitle, lblSub, "LẬP HÓA ĐƠN BÁN HÀNG TẠI QUẦY (POS)", "Hệ thống Bán Lẻ Thu Ngân • Lập hóa đơn và thanh toán tức thì");
            UiTheme.ApplyModernGrid(dgvChiTiet);

            UiTheme.ApplyPrimaryButton(btnThemMon, "➕ Thêm Vào Đơn");
            UiTheme.ApplyDangerButton(btnXoaMon, "🗑️ Xóa Món Đang Chọn");
            UiTheme.ApplySuccessButton(btnThanhToanTienMat, "💵 THANH TOÁN TIỀN MẶT (F8)");
            UiTheme.ApplyPrimaryButton(btnThanhToanQR, "📱 THANH TOÁN VIETQR (F9)");

            lblTongTien.Font = UiTheme.FontDisplay;
            lblTongTien.ForeColor = UiTheme.Danger;
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

            if (dgvChiTiet.Columns["MaSp"] is { } colMa) colMa.HeaderText = "Mã SP";
            if (dgvChiTiet.Columns["TenSp"] is { } colTen) colTen.HeaderText = "Tên Sản Phẩm";
            if (dgvChiTiet.Columns["SoLuong"] is { } colSl) colSl.HeaderText = "Số Lượng";
            if (dgvChiTiet.Columns["DonGia"] is { } colDg)
            {
                colDg.HeaderText = "Đơn Giá";
                colDg.DefaultCellStyle.Format = "#,##0 ₫";
            }
            if (dgvChiTiet.Columns["KhuyenMai"] is { } colKm)
            {
                colKm.HeaderText = "Chiết Khấu";
                colKm.DefaultCellStyle.Format = "0'%'";
            }
            if (dgvChiTiet.Columns["ThanhTien"] is { } colTt)
            {
                colTt.HeaderText = "Thành Tiền";
                colTt.DefaultCellStyle.Format = "#,##0 ₫";
            }

            dgvChiTiet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private async void LoadProductsAsync()
        {
            try
            {
                _products = await _catalogClient.GetProductsAsync();
                if (IsDisposed || Disposing) return;
                cboSanPham.DataSource = _products;
                cboSanPham.DisplayMember = "TenSp";
                cboSanPham.ValueMember = "MaSp";
                cboSanPham.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cboSanPham.AutoCompleteSource = AutoCompleteSource.ListItems;

                if (_products.Count > 0)
                {
                    cboSanPham_SelectedIndexChanged(this, EventArgs.Empty);
                }
            }
            catch { }
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

            if (km < 0 || km > 100)
            {
                MessageBox.Show("Mức chiết khấu phải từ 0% đến 100%!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (sp.SoLuong <= 0)
            {
                MessageBox.Show($"Sản phẩm '{sp.TenSp}' hiện đã hết hàng trong kho!", "Hết hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                decimal newKm = km > 0 ? km : existing.Field<decimal>("KhuyenMai");
                existing["SoLuong"] = newSl;
                existing["KhuyenMai"] = newKm;
                existing["ThanhTien"] = Math.Round(newSl * sp.GiaBan * (1m - (newKm / 100m)), 0);
            }
            else
            {
                decimal thanhTien = Math.Round(sl * sp.GiaBan * (1m - (km / 100m)), 0);
                _cartTable.Rows.Add(sp.MaSp, sp.TenSp, sl, sp.GiaBan, km, Math.Max(0, thanhTien));
            }

            UpdateTotalAmount();

            // Đặt lại số lượng và chiết khấu về mặc định sau khi thêm
            numSoLuong.Value = 1;
            numKhuyenMai.Value = 0;
        }

        private void btnXoaMon_Click(object sender, EventArgs e)
        {
            if (dgvChiTiet.CurrentRow?.DataBoundItem is DataRowView drv)
            {
                drv.Row.Delete();
                _cartTable.AcceptChanges();
                UpdateTotalAmount();
            }
            else if (dgvChiTiet.CurrentRow != null && dgvChiTiet.CurrentRow.Index >= 0 && dgvChiTiet.CurrentRow.Index < _cartTable.Rows.Count)
            {
                _cartTable.Rows.RemoveAt(dgvChiTiet.CurrentRow.Index);
                _cartTable.AcceptChanges();
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

        private async void btnThanhToanTienMat_Click(object sender, EventArgs e)
        {
            await ProcessPaymentAsync(isBankTransfer: false);
        }

        private async void btnThanhToanQR_Click(object sender, EventArgs e)
        {
            decimal total = CalculateTotal();
            if (total <= 0 || _cartTable.Rows.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống! Vui lòng chọn sản phẩm vào đơn trước.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Lấy mã hóa đơn bán kế tiếp chuẩn từ Server (Ví dụ: HDB13)
            string? maHdb = await _salesClient.GetNextInvoiceCodeAsync();
            if (string.IsNullOrWhiteSpace(maHdb))
            {
                MessageBox.Show("Không thể kết nối đến máy chủ API để tạo mã hóa đơn! Vui lòng đảm bảo dịch vụ Backend đang hoạt động.", "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Mở Popup VietQR MBBank lắng nghe Webhook tự động với đúng mã hóa đơn
            using var qrForm = new frmThanhToanQR(maHdb, total);
            var result = qrForm.ShowDialog(this);

            if (result == DialogResult.OK && qrForm.IsPaymentSuccessful)
            {
                // Webhook đã báo tiền vào tài khoản ➔ Hoàn tất hóa đơn trên hệ thống
                await ProcessPaymentAsync(isBankTransfer: true, maHdbGiaoDich: maHdb);
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
                MaHdb = maHdbGiaoDich, // Đồng bộ mã đơn hàng với mã QR giao dịch ngân hàng
                MaNv = _nhanVien,
                TenKhachVangLai = string.IsNullOrWhiteSpace(txtTenKhach.Text) ? "Khách Mua Tại Quầy" : txtTenKhach.Text.Trim(),
                DienThoai = string.IsNullOrWhiteSpace(txtDienThoai.Text) ? "0900000000" : txtDienThoai.Text.Trim(),
                DiaChiGiaoHang = "Tại quầy Duy Mobile",
                PhuongThucThanhToan = isBankTransfer ? "VietQR" : "Cash",
                DanhSachMon = _cartTable.AsEnumerable().Select(r => new ChiTietHdbDto
                {
                    MaSp = r.Field<string>("MaSp") ?? "",
                    TenSp = r.Field<string>("TenSp") ?? "",
                    SoLuong = r.Field<int>("SoLuong"),
                    KhuyenMai = r.Field<decimal>("KhuyenMai")
                }).ToList()
            };

            var (success, message, maHdb, _) = await _salesClient.CreateSaleOrderAsync(dto);

            if (success)
            {
                string effectiveMaHdb = !string.IsNullOrWhiteSpace(maHdb) ? maHdb : (maHdbGiaoDich ?? string.Empty);

                // Trích xuất danh sách sản phẩm sang dữ liệu in hóa đơn
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
                    diaChi: "Tại quầy Duy Mobile",
                    tenNv: _nhanVien,
                    danhSachSp: bieuMauItems,
                    tongTien: tongTienHang,
                    giamGia: tongGiamGia
                );

                // Hiển thị ngay hóa đơn bán lẻ trên trình duyệt để in / xuất ảnh
                BieuMauService.MoBieuMauTrenTrinhDuyet(html, $"HoaDonBan_{effectiveMaHdb}");

                string phuongThuc = isBankTransfer ? "Chuyển khoản VietQR MBBank" : "Tiền mặt tại quầy";
                MessageBox.Show($"✅ THANH TOÁN THÀNH CÔNG!\n\n• Mã HĐ: {effectiveMaHdb}\n• Tổng tiền: {total:N0} ₫\n• Phương thức: {phuongThuc}\n• Hóa đơn bán lẻ & Phiếu bảo hành đã được khởi tạo thành công.",
                    "Thông Báo Thanh Toán", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
