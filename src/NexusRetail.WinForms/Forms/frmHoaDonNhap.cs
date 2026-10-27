using NexusRetail.Shared.DTOs.NhapHang;
using NexusRetail.Shared.DTOs.SanPham;
using NexusRetail.WinForms.Services;
using NexusRetail.WinForms.UI;
using System.Globalization;

namespace NexusRetail.WinForms.Forms
{
    public partial class frmHoaDonNhap : Form
    {
        private readonly string _currentUser;
        private List<SanPhamDto> _productList = new();
        private List<NhaCungCapItem> _supplierList = new();
        private List<NhanVienItem> _employeeList = new();
        private readonly List<ChiTietHdnDto> _cart = new();

        public frmHoaDonNhap(string currentUser)
        {
            _currentUser = currentUser;
            InitializeComponent();

            UiTheme.BeautifyForm(this);
            ApplyCustomStyles();

            KeyPreview = true;
            KeyDown += FrmHoaDonNhap_KeyDown;
        }

        private void FrmHoaDonNhap_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F8)
            {
                e.Handled = true;
                btnLuuNhapKho_Click(this, EventArgs.Empty);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                if (_cart.Count > 0)
                {
                    var res = MessageBox.Show("Phiếu nhập đang có mặt hàng, bạn có chắc chắn muốn đóng?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
            UiTheme.ApplyFormHeader(pnlHeader, lblTitle, lblSubTitle, "LẬP HÓA ĐƠN NHẬP KHO THIẾT BỊ", "Cập nhật tồn kho (BR-01), cập nhật giá nhập (BR-02) và tự động tính giá niêm yết (BR-03)");
            UiTheme.ApplyModernGrid(dgvChiTiet);

            UiTheme.ApplyPrimaryButton(btnThemSp, "➕ Thêm Vào Phiếu Nhập");
            UiTheme.ApplyDangerButton(btnXoaDong, "🗑️ Xóa Dòng Đang Chọn");
            UiTheme.ApplySuccessButton(btnLuuNhapKho, "💾 LƯU PHIẾU NHẬP KHO (F8)");
            UiTheme.ApplySecondaryButton(btnDong, "❌ Đóng");

            lblTongTien.Font = UiTheme.FontDisplay;
            lblTongTien.ForeColor = UiTheme.Danger;
        }

        private async void frmHoaDonNhap_Load(object sender, EventArgs e)
        {
            await LoadInitialDataAsync();
        }

        private async Task LoadInitialDataAsync()
        {
            try
            {
                // 1. Tải danh sách nhà cung cấp
                _supplierList = await WinFormsApiClient.Instance.GetSuppliersAsync();
                cboNhaCungCap.DataSource = _supplierList;
                cboNhaCungCap.DisplayMember = "TenNcc";
                cboNhaCungCap.ValueMember = "MaNcc";

                // 2. Tải danh sách nhân viên
                _employeeList = await WinFormsApiClient.Instance.GetEmployeesAsync();
                cboNhanVien.DataSource = _employeeList;
                cboNhanVien.DisplayMember = "TenNv";
                cboNhanVien.ValueMember = "MaNv";

                // Tự động chọn nhân viên đang đăng nhập nếu khớp mã
                var matchedNv = _employeeList.FirstOrDefault(x => x.MaNv.Equals(_currentUser, StringComparison.OrdinalIgnoreCase) || x.TenNv.Contains(_currentUser));
                if (matchedNv != null)
                {
                    cboNhanVien.SelectedItem = matchedNv;
                }

                // 3. Tải danh mục sản phẩm
                _productList = await WinFormsApiClient.Instance.GetProductsAsync();
                cboSanPham.DataSource = _productList;
                cboSanPham.DisplayMember = "TenSp";
                cboSanPham.ValueMember = "MaSp";

                CapNhatGiaNhapTuSanPham();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu ban đầu: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void cboSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            CapNhatGiaNhapTuSanPham();
        }

        private void CapNhatGiaNhapTuSanPham()
        {
            if (cboSanPham.SelectedItem is SanPhamDto sp)
            {
                txtDonGia.Text = sp.GiaNhap.ToString("N0", new CultureInfo("vi-VN"));
            }
        }

        private void btnThemSp_Click(object sender, EventArgs e)
        {
            if (cboSanPham.SelectedItem is not SanPhamDto sp)
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm cần nhập!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int soLuong = (int)numSoLuong.Value;
            if (soLuong <= 0)
            {
                MessageBox.Show("Số lượng nhập phải lớn hơn 0!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string cleanPrice = txtDonGia.Text.Replace(".", "").Replace(",", "").Replace("đ", "").Trim();
            if (!decimal.TryParse(cleanPrice, out decimal donGia) || donGia <= 0)
            {
                MessageBox.Show("Đơn giá nhập không hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal khuyenMai = numKhuyenMai.Value;
            decimal thanhTien = soLuong * donGia * (1m - (khuyenMai / 100m));

            // Kiểm tra xem sản phẩm đã có trong phiếu nhập chưa, nếu có thì cộng dồn
            var existing = _cart.FirstOrDefault(x => x.MaSp == sp.MaSp);
            if (existing != null)
            {
                existing.SoLuong += soLuong;
                existing.DonGia = donGia;
                existing.KhuyenMai = khuyenMai;
                existing.ThanhTien = existing.SoLuong * existing.DonGia * (1m - (existing.KhuyenMai / 100m));
            }
            else
            {
                _cart.Add(new ChiTietHdnDto
                {
                    MaSp = sp.MaSp,
                    TenSp = sp.TenSp,
                    SoLuong = soLuong,
                    DonGia = donGia,
                    KhuyenMai = khuyenMai,
                    ThanhTien = thanhTien
                });
            }

            HienThiBangChiTiet();

            // Reset input
            numSoLuong.Value = 1;
            numKhuyenMai.Value = 0;
        }

        private void HienThiBangChiTiet()
        {
            dgvChiTiet.Rows.Clear();
            var culture = new CultureInfo("vi-VN");

            decimal tongTien = 0;
            for (int i = 0; i < _cart.Count; i++)
            {
                var item = _cart[i];
                tongTien += item.ThanhTien;
                dgvChiTiet.Rows.Add(
                    i + 1,
                    item.MaSp,
                    item.TenSp ?? "",
                    item.DonGia.ToString("N0", culture) + " đ",
                    item.SoLuong,
                    item.KhuyenMai > 0 ? $"{item.KhuyenMai}%" : "-",
                    item.ThanhTien.ToString("N0", culture) + " đ"
                );
            }

            lblSoMatHang.Text = $"Số mặt hàng nhập: {_cart.Count}";
            lblTongTien.Text = tongTien.ToString("N0", culture) + " đ";
        }

        private void btnXoaDong_Click(object sender, EventArgs e)
        {
            if (dgvChiTiet.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn dòng sản phẩm cần xóa khỏi phiếu nhập!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int index = dgvChiTiet.SelectedRows[0].Index;
            if (index >= 0 && index < _cart.Count)
            {
                _cart.RemoveAt(index);
                HienThiBangChiTiet();
            }
        }

        private async void btnLuuNhapKho_Click(object sender, EventArgs e)
        {
            if (_cart.Count == 0)
            {
                MessageBox.Show("Phiếu nhập hàng hiện đang trống! Vui lòng chọn ít nhất 1 sản phẩm.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboNhaCungCap.SelectedItem is not NhaCungCapItem ncc)
            {
                MessageBox.Show("Vui lòng chọn Nhà cung cấp!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboNhanVien.SelectedItem is not NhanVienItem nv)
            {
                MessageBox.Show("Vui lòng chọn Nhân viên nhập hàng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Xác nhận lập hóa đơn nhập {_cart.Count} mặt hàng từ '{ncc.TenNcc}'?\n\nTổng giá trị nhập: {lblTongTien.Text}\n\nTồn kho và đơn giá bán của sản phẩm sẽ được tự động cập nhật.",
                "Xác nhận nhập kho thiết bị",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            btnLuuNhapKho.Enabled = false;
            btnLuuNhapKho.Text = "⏳ Đang ghi sổ kho...";

            try
            {
                var dto = new HoaDonNhapDto
                {
                    MaNv = nv.MaNv,
                    MaNcc = ncc.MaNcc,
                    DanhSachMon = _cart.Select(x => new ChiTietHdnDto
                    {
                        MaSp = x.MaSp,
                        SoLuong = x.SoLuong,
                        DonGia = x.DonGia,
                        KhuyenMai = x.KhuyenMai
                    }).ToList()
                };

                var (success, message, maHdn, tongTien) = await WinFormsApiClient.Instance.CreateInboundOrderAsync(dto);

                if (success)
                {
                    string effectiveMaHdn = !string.IsNullOrWhiteSpace(maHdn) ? maHdn : string.Empty;

                    // Trích xuất danh sách sản phẩm sang dữ liệu biểu mẫu BM01
                    var bieuMauItems = _cart.Select((x, idx) => new ItemBieuMauNhap
                    {
                        Stt = idx + 1,
                        MaSp = x.MaSp,
                        TenSp = x.TenSp ?? "",
                        Dvt = "Cây",
                        SoLuong = x.SoLuong,
                        DonGia = x.DonGia,
                        KhuyenMai = x.KhuyenMai,
                        ThanhTien = x.ThanhTien
                    }).ToList();

                    decimal tongGiaTri = tongTien > 0 ? tongTien : bieuMauItems.Sum(x => x.ThanhTien);

                    string html = BieuMauService.TaoHtmlHoaDonNhap(
                        maHdn: effectiveMaHdn,
                        tenNcc: ncc.TenNcc,
                        diaChiNcc: !string.IsNullOrWhiteSpace(ncc.DiaChi) ? ncc.DiaChi : "Tòa nhà FPT Cầu Giấy, Hà Nội",
                        sdtNcc: !string.IsNullOrWhiteSpace(ncc.DienThoai) ? ncc.DienThoai : "(028) 7300 1234",
                        tenNv: nv.TenNv,
                        maNv: nv.MaNv,
                        danhSachSp: bieuMauItems,
                        tongTien: tongGiaTri
                    );

                    // Mở biểu mẫu BM01 trên trình duyệt để in / xuất ảnh
                    BieuMauService.MoBieuMauTrenTrinhDuyet(html, $"HoaDonNhap_{effectiveMaHdn}");

                    string info = $"🎉 NHẬP KHO THÀNH CÔNG!\n\n" +
                                  $"• Mã HĐ Nhập: {effectiveMaHdn}\n" +
                                  $"• Nhà cung cấp: {ncc.TenNcc}\n" +
                                  $"• Nhân viên tiếp nhận: {nv.TenNv}\n" +
                                  $"• Tổng tiền phiếu nhập: {tongGiaTri.ToString("N0", new CultureInfo("vi-VN"))} đ\n\n" +
                                  $"✓ Đã xuất và mở biểu mẫu BM01 (Phiếu Nhập Kho) để in ấn hoặc xuất ảnh PNG.";

                    MessageBox.Show(info, "Nhập kho hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    _cart.Clear();
                    HienThiBangChiTiet();
                    txtMaHdn.Text = effectiveMaHdn;
                }
                else
                {
                    MessageBox.Show(message, "Lỗi nhập kho", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                btnLuuNhapKho.Enabled = true;
                btnLuuNhapKho.Text = "💾 Lưu & Hoàn tất nhập kho";
            }
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
