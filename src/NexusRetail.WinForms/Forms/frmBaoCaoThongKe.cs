using NexusRetail.WinForms.Services;
using System.Diagnostics;
using System.Globalization;

namespace NexusRetail.WinForms.Forms
{
    public partial class frmBaoCaoThongKe : Form
    {
        public frmBaoCaoThongKe()
        {
            InitializeComponent();
        }

        private async void frmBaoCaoThongKe_Load(object sender, EventArgs e)
        {
            cboLoaiBaoCao.SelectedIndex = 0;
            numThang.Value = DateTime.Now.Month;
            numQuy.Value = (DateTime.Now.Month - 1) / 3 + 1;
            numNam.Value = 2026;

            await TaiDuLieuBaoCaoAsync();
        }

        private void cboLoaiBaoCao_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Điều chỉnh hiển thị bộ lọc tùy theo loại báo cáo
            switch (cboLoaiBaoCao.SelectedIndex)
            {
                case 0: // BR-06: Tháng + Năm
                    numThang.Enabled = true;
                    numQuy.Enabled = false;
                    numNam.Enabled = true;
                    break;
                case 1: // BR-07: Quý + Năm
                    numThang.Enabled = false;
                    numQuy.Enabled = true;
                    numNam.Enabled = true;
                    break;
                case 2: // BR-08: Năm
                    numThang.Enabled = false;
                    numQuy.Enabled = false;
                    numNam.Enabled = true;
                    break;
                case 3: // BR-09: Tháng + Năm
                    numThang.Enabled = true;
                    numQuy.Enabled = false;
                    numNam.Enabled = true;
                    break;
            }
        }

        private async void btnXemBaoCao_Click(object sender, EventArgs e)
        {
            await TaiDuLieuBaoCaoAsync();
        }

        private async Task TaiDuLieuBaoCaoAsync()
        {
            btnXemBaoCao.Enabled = false;
            btnXemBaoCao.Text = "⏳ Đang tải...";
            dgvBaoCao.Columns.Clear();
            dgvBaoCao.Rows.Clear();

            var culture = new CultureInfo("vi-VN");
            int thang = (int)numThang.Value;
            int quy = (int)numQuy.Value;
            int nam = (int)numNam.Value;

            try
            {
                switch (cboLoaiBaoCao.SelectedIndex)
                {
                    case 0: // BR-06: Top 5 SP bán chạy
                        {
                            dgvBaoCao.Columns.Add("colStt", "STT");
                            dgvBaoCao.Columns.Add("colMaSp", "Mã SP");
                            dgvBaoCao.Columns.Add("colTenSp", "Tên Sản Phẩm");
                            dgvBaoCao.Columns.Add("colLoai", "Phân Loại");
                            dgvBaoCao.Columns.Add("colNhanHieu", "Nhãn Hiệu");
                            dgvBaoCao.Columns.Add("colSoLuong", "Số Lượng Đã Bán");
                            dgvBaoCao.Columns.Add("colDoanhThu", "Tổng Doanh Thu");

                            var data = await WinFormsApiClient.Instance.GetReportSpBanChayAsync(thang, nam);
                            decimal tongDoanhThu = 0;
                            int tongSl = 0;
                            for (int i = 0; i < data.Count; i++)
                            {
                                var item = data[i];
                                tongDoanhThu += item.TongDoanhThu;
                                tongSl += item.TongSoLuong;
                                dgvBaoCao.Rows.Add(i + 1, item.MaSp, item.TenSp, item.TenLoai, item.TenNhanHieu, item.TongSoLuong, item.TongDoanhThu.ToString("N0", culture) + " đ");
                            }

                            lblSoDong.Text = $"Top {data.Count} sản phẩm (Tổng bán: {tongSl} máy)";
                            lblTongKet.Text = $"Tổng doanh thu: {tongDoanhThu.ToString("N0", culture)} đ";
                        }
                        break;

                    case 1: // BR-07: HĐ Nhập lớn nhất trong quý
                        {
                            dgvBaoCao.Columns.Add("colStt", "STT");
                            dgvBaoCao.Columns.Add("colMaHdn", "Mã Hóa Đơn Nhập");
                            dgvBaoCao.Columns.Add("colNgayNhap", "Ngày Nhập Kho");
                            dgvBaoCao.Columns.Add("colNcc", "Nhà Cung Cấp");
                            dgvBaoCao.Columns.Add("colNv", "Nhân Viên Nhập");
                            dgvBaoCao.Columns.Add("colTongTien", "Tổng Tiền Nhập");

                            var data = await WinFormsApiClient.Instance.GetReportHdnLonNhatAsync(quy, nam);
                            decimal tongTien = 0;
                            for (int i = 0; i < data.Count; i++)
                            {
                                var item = data[i];
                                tongTien += item.TongTien;
                                dgvBaoCao.Rows.Add(i + 1, item.MaHdn, item.NgayNhap.ToString("dd/MM/yyyy HH:mm"), item.TenNcc, item.TenNv, item.TongTien.ToString("N0", culture) + " đ");
                            }

                            lblSoDong.Text = $"Tổng số hóa đơn nhập trong Quý {quy}/{nam}: {data.Count}";
                            lblTongKet.Text = $"Tổng vốn nhập hàng: {tongTien.ToString("N0", culture)} đ";
                        }
                        break;

                    case 2: // BR-08: Doanh số bán theo năm
                        {
                            dgvBaoCao.Columns.Add("colStt", "STT");
                            dgvBaoCao.Columns.Add("colMaHdb", "Mã Hóa Đơn Bán");
                            dgvBaoCao.Columns.Add("colNgayBan", "Ngày Lập HĐ");
                            dgvBaoCao.Columns.Add("colKhach", "Khách Hàng");
                            dgvBaoCao.Columns.Add("colNv", "Nhân Viên Thu Ngân");
                            dgvBaoCao.Columns.Add("colTongTien", "Tổng Tiền HĐ");

                            var data = await WinFormsApiClient.Instance.GetReportDoanhSoNamAsync(nam);
                            decimal tongDoanhThu = 0;
                            for (int i = 0; i < data.Count; i++)
                            {
                                var item = data[i];
                                tongDoanhThu += item.TongTien;
                                dgvBaoCao.Rows.Add(i + 1, item.MaHdb, item.NgayBan.ToString("dd/MM/yyyy HH:mm"), item.TenKhach, item.TenNv, item.TongTien.ToString("N0", culture) + " đ");
                            }

                            lblSoDong.Text = $"Tổng số đơn bán trong Năm {nam}: {data.Count}";
                            lblTongKet.Text = $"Tổng doanh số cả năm: {tongDoanhThu.ToString("N0", culture)} đ";
                        }
                        break;

                    case 3: // BR-09: Top 5 NV doanh số thấp
                        {
                            dgvBaoCao.Columns.Add("colStt", "STT");
                            dgvBaoCao.Columns.Add("colMaNv", "Mã Nhân Viên");
                            dgvBaoCao.Columns.Add("colTenNv", "Họ Tên Nhân Viên");
                            dgvBaoCao.Columns.Add("colGioiTinh", "Giới Tính");
                            dgvBaoCao.Columns.Add("colDienThoai", "Số Điện Thoại");
                            dgvBaoCao.Columns.Add("colDoanhSo", "Doanh Số Đạt Được");

                            var data = await WinFormsApiClient.Instance.GetReportNvDoanhSoThapAsync(thang, nam);
                            for (int i = 0; i < data.Count; i++)
                            {
                                var item = data[i];
                                dgvBaoCao.Rows.Add(i + 1, item.MaNv, item.TenNv, item.GioiTinh, item.DienThoai, item.TongTienBan.ToString("N0", culture) + " đ");
                            }

                            lblSoDong.Text = $"Top {data.Count} nhân viên cần cải thiện hiệu suất";
                            lblTongKet.Text = $"Tháng {thang}/{nam}";
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi truy vấn báo cáo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnXemBaoCao.Enabled = true;
                btnXemBaoCao.Text = "🔍 Xem Báo Cáo";
            }
        }

        private async void btnXuatExcel_Click(object sender, EventArgs e)
        {
            int thang = (int)numThang.Value;
            int quy = (int)numQuy.Value;
            int nam = (int)numNam.Value;

            string endpoint = "";
            string defaultFileName = "";

            switch (cboLoaiBaoCao.SelectedIndex)
            {
                case 0:
                    endpoint = $"api/bao-cao/sp-ban-chay/excel?month={thang}&year={nam}";
                    defaultFileName = $"BaoCao_Top5_SanPham_Thang_{thang}_{nam}.xlsx";
                    break;
                case 1:
                    endpoint = $"api/bao-cao/hdn-lon-nhat/excel?quarter={quy}&year={nam}";
                    defaultFileName = $"BaoCao_HDN_LonNhat_Quy_{quy}_{nam}.xlsx";
                    break;
                case 2:
                    endpoint = $"api/bao-cao/doanh-so-nam/excel?year={nam}";
                    defaultFileName = $"BaoCao_DoanhSo_Nam_{nam}.xlsx";
                    break;
                case 3:
                    endpoint = $"api/bao-cao/nv-doanh-so-thap/excel?month={thang}&year={nam}";
                    defaultFileName = $"BaoCao_5NV_DoanhSoThap_Thang_{thang}_{nam}.xlsx";
                    break;
            }

            using var sfd = new SaveFileDialog
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = defaultFileName,
                Title = "Lưu file Báo cáo Excel"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                btnXuatExcel.Enabled = false;
                btnXuatExcel.Text = "⏳ Đang kết xuất...";

                var (success, message) = await WinFormsApiClient.Instance.DownloadExcelAsync(endpoint, sfd.FileName);

                btnXuatExcel.Enabled = true;
                btnXuatExcel.Text = "📊 Xuất Excel";

                if (success)
                {
                    var ask = MessageBox.Show($"Xuất báo cáo Excel thành công!\n\nĐường dẫn: {sfd.FileName}\n\nBạn có muốn mở file Excel ngay bây giờ không?", "Thành công", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (ask == DialogResult.Yes)
                    {
                        try
                        {
                            Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                        }
                        catch { }
                    }
                }
                else
                {
                    MessageBox.Show(message, "Lỗi xuất file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
