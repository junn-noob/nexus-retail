using NexusRetail.WinForms.Services;
using System.Globalization;

namespace NexusRetail.WinForms.Forms
{
    public partial class frmTraCuuHoaDon : Form
    {
        private List<InvoiceSearchItem> _saleList = new();
        private List<InboundInvoiceSearchItem> _inboundList = new();

        public frmTraCuuHoaDon()
        {
            InitializeComponent();
        }

        private async void frmTraCuuHoaDon_Load(object sender, EventArgs e)
        {
            SetupDataGridColumns();
            await TaiDanhSachHoaDonBanAsync();
        }

        private void SetupDataGridColumns()
        {
            // Cấu hình bảng Hóa đơn bán
            dgvHoaDonBan.Columns.Clear();
            dgvHoaDonBan.Columns.Add("colMaHdb", "Mã HĐ Bán");
            dgvHoaDonBan.Columns.Add("colNgayBan", "Ngày Lập");
            dgvHoaDonBan.Columns.Add("colKhach", "Khách Hàng");
            dgvHoaDonBan.Columns.Add("colSdt", "Số Điện Thoại");
            dgvHoaDonBan.Columns.Add("colNv", "Nhân Viên Thu Ngân");
            dgvHoaDonBan.Columns.Add("colTongTien", "Tổng Tiền HĐ");

            // Cấu hình bảng Chi tiết bán
            dgvChiTietBan.Columns.Clear();
            dgvChiTietBan.Columns.Add("colCtMaSp", "Mã SP");
            dgvChiTietBan.Columns.Add("colCtTenSp", "Tên Điện Thoại / Linh Kiện");
            dgvChiTietBan.Columns.Add("colCtDonGia", "Đơn Giá");
            dgvChiTietBan.Columns.Add("colCtSoLuong", "Số Lượng");
            dgvChiTietBan.Columns.Add("colCtKhuyenMai", "Khuyến Mãi");
            dgvChiTietBan.Columns.Add("colCtThanhTien", "Thành Tiền");

            // Cấu hình bảng Hóa đơn nhập
            dgvHoaDonNhap.Columns.Clear();
            dgvHoaDonNhap.Columns.Add("colMaHdn", "Mã HĐ Nhập");
            dgvHoaDonNhap.Columns.Add("colNgayNhap", "Ngày Nhập Kho");
            dgvHoaDonNhap.Columns.Add("colNcc", "Nhà Cung Cấp");
            dgvHoaDonNhap.Columns.Add("colNvNhap", "Nhân Viên Nhập");
            dgvHoaDonNhap.Columns.Add("colSoMatHang", "Số Mặt Hàng");
            dgvHoaDonNhap.Columns.Add("colTongTienNhap", "Tổng Tiền Nhập");

            // Cấu hình bảng Chi tiết nhập
            dgvChiTietNhap.Columns.Clear();
            dgvChiTietNhap.Columns.Add("colCtNhapMaSp", "Mã SP");
            dgvChiTietNhap.Columns.Add("colCtNhapTenSp", "Tên Thiết Bị");
            dgvChiTietNhap.Columns.Add("colCtNhapDonGia", "Đơn Giá Nhập");
            dgvChiTietNhap.Columns.Add("colCtNhapSoLuong", "Số Lượng");
            dgvChiTietNhap.Columns.Add("colCtNhapKhuyenMai", "Chiết Khấu (%)");
            dgvChiTietNhap.Columns.Add("colCtNhapThanhTien", "Thành Tiền");
        }

        private async Task TaiDanhSachHoaDonBanAsync(string? q = null)
        {
            dgvHoaDonBan.Rows.Clear();
            dgvChiTietBan.Rows.Clear();
            var culture = new CultureInfo("vi-VN");

            _saleList = await WinFormsApiClient.Instance.SearchSaleInvoicesAsync(q);
            foreach (var item in _saleList)
            {
                dgvHoaDonBan.Rows.Add(
                    item.MaHdb,
                    item.NgayBan.ToString("dd/MM/yyyy HH:mm"),
                    item.TenKhach,
                    item.DienThoai,
                    item.TenNv,
                    item.TongTien.ToString("N0", culture) + " đ"
                );
            }

            lblThongTin.Text = $"Đã tải {_saleList.Count} hóa đơn bán hàng.";
        }

        private async Task TaiDanhSachHoaDonNhapAsync(string? q = null)
        {
            dgvHoaDonNhap.Rows.Clear();
            dgvChiTietNhap.Rows.Clear();
            var culture = new CultureInfo("vi-VN");

            _inboundList = await WinFormsApiClient.Instance.SearchInboundInvoicesAsync();

            var filtered = _inboundList.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                string kw = q.Trim().ToLower();
                filtered = filtered.Where(x =>
                    x.MaHdn.ToLower().Contains(kw) ||
                    x.TenNcc.ToLower().Contains(kw) ||
                    x.TenNv.ToLower().Contains(kw));
            }

            foreach (var item in filtered)
            {
                dgvHoaDonNhap.Rows.Add(
                    item.MaHdn,
                    item.NgayNhap.ToString("dd/MM/yyyy HH:mm"),
                    item.TenNcc,
                    item.TenNv,
                    item.SoMatHang,
                    item.TongTien.ToString("N0", culture) + " đ"
                );
            }

            lblThongTin.Text = $"Đã tải {dgvHoaDonNhap.Rows.Count} phiếu nhập kho.";
        }

        private async void dgvHoaDonBan_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvHoaDonBan.SelectedRows.Count == 0) return;

            string maHdb = dgvHoaDonBan.SelectedRows[0].Cells["colMaHdb"].Value?.ToString() ?? "";
            if (string.IsNullOrEmpty(maHdb)) return;

            var details = await WinFormsApiClient.Instance.GetSaleInvoiceDetailsAsync(maHdb);
            dgvChiTietBan.Rows.Clear();
            var culture = new CultureInfo("vi-VN");

            foreach (var ct in details)
            {
                dgvChiTietBan.Rows.Add(
                    ct.MaSp,
                    ct.TenSp,
                    ct.DonGia.ToString("N0", culture) + " đ",
                    ct.SoLuong,
                    ct.KhuyenMai > 0 ? ct.KhuyenMai.ToString("N0", culture) + " đ" : "-",
                    ct.ThanhTien.ToString("N0", culture) + " đ"
                );
            }
        }

        private async void dgvHoaDonNhap_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvHoaDonNhap.SelectedRows.Count == 0) return;

            string maHdn = dgvHoaDonNhap.SelectedRows[0].Cells["colMaHdn"].Value?.ToString() ?? "";
            if (string.IsNullOrEmpty(maHdn)) return;

            var details = await WinFormsApiClient.Instance.GetInboundInvoiceDetailsAsync(maHdn);
            dgvChiTietNhap.Rows.Clear();
            var culture = new CultureInfo("vi-VN");

            foreach (var ct in details)
            {
                dgvChiTietNhap.Rows.Add(
                    ct.MaSp,
                    ct.TenSp,
                    ct.DonGia.ToString("N0", culture) + " đ",
                    ct.SoLuong,
                    ct.KhuyenMai > 0 ? $"{ct.KhuyenMai}%" : "-",
                    ct.ThanhTien.ToString("N0", culture) + " đ"
                );
            }
        }

        private async void tabMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabMain.SelectedIndex == 0)
            {
                await TaiDanhSachHoaDonBanAsync(txtTimKiemBan.Text);
            }
            else
            {
                await TaiDanhSachHoaDonNhapAsync(txtTimKiemNhap.Text);
            }
        }

        private async void btnTimKiemBan_Click(object sender, EventArgs e)
        {
            await TaiDanhSachHoaDonBanAsync(txtTimKiemBan.Text);
        }

        private async void btnLamMoiBan_Click(object sender, EventArgs e)
        {
            txtTimKiemBan.Clear();
            await TaiDanhSachHoaDonBanAsync();
        }

        private void txtTimKiemBan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnTimKiemBan_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private async void btnTimKiemNhap_Click(object sender, EventArgs e)
        {
            await TaiDanhSachHoaDonNhapAsync(txtTimKiemNhap.Text);
        }

        private async void btnLamMoiNhap_Click(object sender, EventArgs e)
        {
            txtTimKiemNhap.Clear();
            await TaiDanhSachHoaDonNhapAsync();
        }

        private void txtTimKiemNhap_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnTimKiemNhap_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
