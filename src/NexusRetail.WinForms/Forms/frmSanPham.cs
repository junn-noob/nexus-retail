using NexusRetail.Shared.DTOs.Common;
using NexusRetail.Shared.DTOs.SanPham;
using NexusRetail.WinForms.Services;

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

            if (_role != "Admin")
            {
                btnXoa.Enabled = false;
                btnXoa.Text += " 🔒";
            }

            LoadProductsAsync();
        }

        private async void LoadProductsAsync(string? q = null)
        {
            _products = await WinFormsApiClient.Instance.GetProductsAsync(q);
            dgvSanPham.DataSource = null;
            dgvSanPham.DataSource = _products;

            FormatGrid();
        }

        private void FormatGrid()
        {
            if (dgvSanPham.Columns["MaSp"] != null) dgvSanPham.Columns["MaSp"].HeaderText = "Mã SP";
            if (dgvSanPham.Columns["TenSp"] != null) dgvSanPham.Columns["TenSp"].HeaderText = "Tên Sản Phẩm";
            if (dgvSanPham.Columns["TenNhanHieu"] != null) dgvSanPham.Columns["TenNhanHieu"].HeaderText = "Nhãn Hiệu";
            if (dgvSanPham.Columns["TenLoai"] != null) dgvSanPham.Columns["TenLoai"].HeaderText = "Phân Loại";
            if (dgvSanPham.Columns["SoLuong"] != null) dgvSanPham.Columns["SoLuong"].HeaderText = "Tồn Kho";
            if (dgvSanPham.Columns["GiaNhap"] != null)
            {
                dgvSanPham.Columns["GiaNhap"].HeaderText = "Giá Nhập";
                dgvSanPham.Columns["GiaNhap"].DefaultCellStyle.Format = "#,##0 ₫";
            }
            if (dgvSanPham.Columns["GiaBan"] != null)
            {
                dgvSanPham.Columns["GiaBan"].HeaderText = "Giá Bán (+10%)";
                dgvSanPham.Columns["GiaBan"].DefaultCellStyle.Format = "#,##0 ₫";
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
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            LoadProductsAsync(txtTimKiem.Text.Trim());
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (_role != "Admin")
            {
                MessageBox.Show("Chỉ có tài khoản Admin mới có quyền xóa sản phẩm!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtMaSp.Text))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa sản phẩm {txtTenSp.Text} ({txtMaSp.Text}) không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                MessageBox.Show("Sản phẩm đã phát sinh hóa đơn bán hàng hoặc hóa đơn nhập hàng, hệ thống bảo vệ dữ liệu không cho phép xóa!", "Ràng buộc toàn vẹn", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
