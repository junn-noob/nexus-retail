using NexusRetail.Shared.DTOs.Common;
using NexusRetail.Shared.DTOs.SanPham;
using NexusRetail.WinForms.Services;
using NexusRetail.WinForms.UI;

namespace NexusRetail.WinForms.Forms
{
    public partial class frmSanPham : Form
    {
        private readonly string _role;
        private List<SanPhamDto> _products = new();

        public frmSanPham(string role)
        {
            _role = role;
            InitializeComponent();

            UiTheme.BeautifyForm(this);
            ApplyCustomStyles();

            if (_role != "Admin")
            {
                btnXoa.Enabled = false;
                btnXoa.Text = "🔒 Xóa (Admin)";
                btnXoa.BackColor = Color.FromArgb(241, 245, 249);
                btnXoa.ForeColor = Color.FromArgb(148, 163, 184);
            }

            KeyPreview = true;
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.F5)
                {
                    e.Handled = true;
                    btnLamMoi_Click(this, EventArgs.Empty);
                    LoadProductsAsync();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    Close();
                }
            };

            txtTimKiem.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    btnTimKiem_Click(this, EventArgs.Empty);
                }
            };

            LoadProductsAsync();
        }

        private void ApplyCustomStyles()
        {
            UiTheme.ApplyFormHeader(pnlTop, lblTitle, lblSub, "QUẢN LÝ DANH MỤC THIẾT BỊ DI ĐỘNG", "Quản lý tồn kho, giá vốn và giá niêm yết tự động (Markup 10%)");
            UiTheme.ApplyModernGrid(dgvSanPham);

            UiTheme.ApplySuccessButton(btnThem, "➕ Thêm Mới");
            UiTheme.ApplyPrimaryButton(btnSua, "✏️ Cập Nhật");
            UiTheme.ApplyDangerButton(btnXoa, "🗑️ Xóa");
            UiTheme.ApplySecondaryButton(btnLamMoi, "🔄 Làm Mới");
            UiTheme.ApplyPrimaryButton(btnTimKiem, "🔍 Tìm Kiếm");
        }

        private async void LoadProductsAsync(string? q = null)
        {
            _products = await WinFormsApiClient.Instance.GetProductsAsync(q);
            dgvSanPham.DataSource = null;
            dgvSanPham.DataSource = _products;

            FormatGrid();
            lblFooterInfo.Text = $"💡 Tổng cộng: {_products.Count} mặt hàng trong danh mục • [F5] Làm mới • [Enter] Tìm kiếm • [Esc] Đóng";
        }

        private void FormatGrid()
        {
            if (dgvSanPham.Columns["MaSp"] is { } colMa) colMa.HeaderText = "Mã SP";
            if (dgvSanPham.Columns["TenSp"] is { } colTen) colTen.HeaderText = "Tên Sản Phẩm";
            if (dgvSanPham.Columns["TenNhanHieu"] is { } colNh) colNh.HeaderText = "Nhãn Hiệu";
            if (dgvSanPham.Columns["TenLoai"] is { } colLoai) colLoai.HeaderText = "Phân Loại";
            if (dgvSanPham.Columns["SoLuong"] is { } colSl) colSl.HeaderText = "Tồn Kho";
            if (dgvSanPham.Columns["GiaNhap"] is { } colGn)
            {
                colGn.HeaderText = "Giá Nhập";
                colGn.DefaultCellStyle.Format = "#,##0 ₫";
            }
            if (dgvSanPham.Columns["GiaBan"] is { } colGb)
            {
                colGb.HeaderText = "Giá Bán (+10%)";
                colGb.DefaultCellStyle.Format = "#,##0 ₫";
            }

            dgvSanPham.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void dgvSanPham_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvSanPham.CurrentRow?.DataBoundItem is SanPhamDto sp)
            {
                txtMaSp.Text = sp.MaSp;
                txtTenSp.Text = sp.TenSp;
                txtGiaNhap.Text = sp.GiaNhap.ToString("N0");
                txtGiaBan.Text = sp.GiaBan.ToString("N0");
                numSoLuong.Value = Math.Max(0, sp.SoLuong);
                numBaoHanh.Value = Math.Max(1, sp.ThoiGianBaoHanh);
                txtGhiChu.Text = sp.GhiChu ?? "";
            }
        }

        // BR-03: TỰ ĐỘNG TÍNH GIÁ BÁN = GIÁ NHẬP * 1.1 KHI GÕ GIÁ NHẬP
        private void txtGiaNhap_TextChanged(object? sender, EventArgs e)
        {
            string clean = txtGiaNhap.Text.Replace(",", "").Replace(".", "").Trim();
            if (decimal.TryParse(clean, out decimal giaNhap))
            {
                decimal giaBan = Math.Round(giaNhap * 1.1m, 0);
                txtGiaBan.Text = giaBan.ToString("N0");
            }
            else
            {
                txtGiaBan.Text = "0";
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtMaSp.Text = "";
            txtTenSp.Text = "";
            txtGiaNhap.Text = "10000000";
            numSoLuong.Value = 10;
            numBaoHanh.Value = 12;
            txtGhiChu.Text = "";
            txtTenSp.Focus();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            LoadProductsAsync(txtTimKiem.Text.Trim());
        }

        private async void btnThem_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra dữ liệu đầu vào
            string tenSp = txtTenSp.Text.Trim();
            if (string.IsNullOrEmpty(tenSp))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenSp.Focus();
                return;
            }

            string cleanGia = txtGiaNhap.Text.Replace(",", "").Replace(".", "").Trim();
            if (!decimal.TryParse(cleanGia, out decimal giaNhap) || giaNhap <= 0)
            {
                MessageBox.Show("Giá nhập phải là số hợp lệ lớn hơn 0!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGiaNhap.Focus();
                return;
            }

            // 2. Tự động nhận diện nhãn hiệu và phân loại thông minh theo tên máy
            string maNhanHieu = "NH01"; // Apple mặc định
            string maLoai = "L01";      // Flagship
            string maManHinh = "MH01";  // Super Retina OLED

            string upper = tenSp.ToUpperInvariant();
            if (upper.Contains("SAMSUNG") || upper.Contains("GALAXY"))
            {
                maNhanHieu = "NH02";
                maManHinh = "MH02";
                maLoai = upper.Contains("FOLD") || upper.Contains("FLIP") ? "L06" : (upper.Contains("ULTRA") ? "L01" : "L02");
            }
            else if (upper.Contains("XIAOMI") || upper.Contains("REDMI"))
            {
                maNhanHieu = "NH03";
                maManHinh = "MH03";
                maLoai = upper.Contains("NOTE") || upper.Contains("REDMI") ? "L03" : "L01";
            }
            else if (upper.Contains("OPPO"))
            {
                maNhanHieu = "NH04";
                maManHinh = "MH04";
                maLoai = upper.Contains("FIND") ? "L01" : "L02";
            }
            else if (upper.Contains("VIVO"))
            {
                maNhanHieu = "NH05";
                maManHinh = "MH05";
                maLoai = upper.Contains("X100") || upper.Contains("X200") ? "L01" : "L02";
            }
            else if (upper.Contains("IPHONE") || upper.Contains("APPLE"))
            {
                maNhanHieu = "NH01";
                maManHinh = "MH01";
                maLoai = "L01";
            }

            // 3. Chuẩn bị DTO thêm mới
            var dto = new ThemSanPhamDto
            {
                TenSp = tenSp,
                MaLoai = maLoai,
                MaNhanHieu = maNhanHieu,
                MaManHinh = maManHinh,
                GiaNhap = giaNhap,
                SoLuong = (int)numSoLuong.Value,
                ThoiGianBaoHanh = (int)numBaoHanh.Value,
                AmThanh = "Loa kép Stereo, Hi-Res Audio",
                ChupAnh = "Camera AI chống rung OIS",
                Anh = "/images/products/phone_default.webp",
                GhiChu = string.IsNullOrWhiteSpace(txtGhiChu.Text) ? "Hàng chính hãng fullbox" : txtGhiChu.Text.Trim()
            };

            btnThem.Enabled = false;
            try
            {
                var (success, message, maSp) = await WinFormsApiClient.Instance.CreateProductAsync(dto);
                if (success)
                {
                    decimal giaBan = Math.Round(giaNhap * 1.1m, 0);
                    MessageBox.Show($"✔ Thêm mới sản phẩm '{tenSp}' thành công!\n• Mã sản phẩm: {maSp}\n• Giá nhập: {giaNhap:N0} ₫\n• Giá bán niêm yết (+10%): {giaBan:N0} ₫", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProductsAsync();
                    btnLamMoi_Click(sender, e);
                }
                else
                {
                    MessageBox.Show(message, "Lỗi thêm sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                btnThem.Enabled = true;
            }
        }

        private async void btnSua_Click(object sender, EventArgs e)
        {
            string maSp = txtMaSp.Text.Trim();
            if (string.IsNullOrEmpty(maSp))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa trên danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tenSp = txtTenSp.Text.Trim();
            if (string.IsNullOrEmpty(tenSp))
            {
                MessageBox.Show("Tên sản phẩm không được để trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenSp.Focus();
                return;
            }

            string cleanGia = txtGiaNhap.Text.Replace(",", "").Replace(".", "").Trim();
            if (!decimal.TryParse(cleanGia, out decimal giaNhap) || giaNhap <= 0)
            {
                MessageBox.Show("Giá nhập phải là số hợp lệ và lớn hơn 0!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGiaNhap.Focus();
                return;
            }

            var current = _products.FirstOrDefault(x => x.MaSp == maSp);
            string maLoai = current?.MaLoai ?? "L01";
            string maNhanHieu = current?.MaNhanHieu ?? "NH01";
            string maManHinh = current?.MaManHinh ?? "MH01";

            var dto = new SuaSanPhamDto
            {
                TenSp = tenSp,
                MaLoai = maLoai,
                MaNhanHieu = maNhanHieu,
                MaManHinh = maManHinh,
                GiaNhap = giaNhap,
                SoLuong = (int)numSoLuong.Value,
                ThoiGianBaoHanh = (int)numBaoHanh.Value,
                AmThanh = current?.AmThanh ?? "Loa kép Stereo",
                ChupAnh = current?.ChupAnh ?? "Camera AI OIS",
                Anh = current?.Anh ?? "/images/products/phone_default.webp",
                GhiChu = txtGhiChu.Text.Trim()
            };

            btnSua.Enabled = false;
            try
            {
                var (success, message) = await WinFormsApiClient.Instance.UpdateProductAsync(maSp, dto);
                if (success)
                {
                    decimal giaBan = Math.Round(giaNhap * 1.1m, 0);
                    MessageBox.Show($"✔ Cập nhật thông tin sản phẩm '{tenSp}' ({maSp}) thành công!\n• Giá bán mới: {giaBan:N0} ₫\n• Tồn kho: {numSoLuong.Value}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProductsAsync();
                }
                else
                {
                    MessageBox.Show(message, "Lỗi cập nhật sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                btnSua.Enabled = true;
            }
        }

        private async void btnXoa_Click(object sender, EventArgs e)
        {
            if (_role != "Admin")
            {
                MessageBox.Show("Chỉ có tài khoản Admin mới có quyền xóa sản phẩm!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maSp = txtMaSp.Text.Trim();
            if (string.IsNullOrEmpty(maSp))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa trên danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa sản phẩm {txtTenSp.Text} ({maSp}) không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            btnXoa.Enabled = false;
            try
            {
                var (success, message) = await WinFormsApiClient.Instance.DeleteProductAsync(maSp);
                if (success)
                {
                    MessageBox.Show($"✔ Đã xóa thành công sản phẩm {maSp}!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProductsAsync();
                    btnLamMoi_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Sản phẩm đã phát sinh hóa đơn bán hàng hoặc hóa đơn nhập hàng, hệ thống bảo vệ toàn vẹn dữ liệu không cho phép xóa!", "Ràng buộc dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                btnXoa.Enabled = true;
            }
        }
    }
}
