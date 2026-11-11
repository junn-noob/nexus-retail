using NexusRetail.WinForms.Services;
using NexusRetail.WinForms.UI;
using System.Diagnostics;
using System.Globalization;

namespace NexusRetail.WinForms.Forms
{
    public partial class frmBaoCaoThongKe : Form
    {
        private readonly IPosReportClient _reportClient = WinFormsApiClient.Report;

        public frmBaoCaoThongKe()
        {
            InitializeComponent();

            UiTheme.BeautifyForm(this);
            ApplyCustomStyles();

            KeyPreview = true;
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.F5)
                {
                    e.Handled = true;
                    btnXemBaoCao_Click(this, EventArgs.Empty);
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    Close();
                }
            };
        }

        private void ApplyCustomStyles()
        {
            UiTheme.ApplyFormHeader(pnlHeader, lblTitle, lblSubTitle, "BÁO CÁO THỐNG KÊ DOANH THU & KHO", "Thống kê doanh số bán hàng, hóa đơn nhập kho lớn nhất và xuất báo cáo Excel");
            UiTheme.ApplyModernGrid(dgvBaoCao);

            UiTheme.ApplyPrimaryButton(btnXemBaoCao, "🔍 Xem Báo Cáo (F5)");
            UiTheme.ApplySuccessButton(btnXuatExcel, "📥 Xuất File Excel (.xlsx)");
            UiTheme.ApplySecondaryButton(btnBieuMauKho, "📄 Báo Cáo Xuất Nhập Tồn");
            UiTheme.ApplySecondaryButton(btnDong, "❌ Đóng (Esc)");

            var btnCrystal = new Button
            {
                Text = "📊 Báo Cáo Trực Quan",
                Size = new Size(210, 36),
                Location = new Point(520, 50),
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };
            UiTheme.ApplyPrimaryButton(btnCrystal, "📊 Báo Cáo Trực Quan");
            btnCrystal.Click += (s, e) => MoVisualReportViewer();
            pnlControls.Controls.Add(btnCrystal);

            lblTongKet.Font = new Font("Segoe UI", 11.5f, FontStyle.Bold);
            lblTongKet.ForeColor = UiTheme.PrimaryDark;
        }

        private async void frmBaoCaoThongKe_Load(object sender, EventArgs e)
        {
            cboLoaiBaoCao.SelectedIndex = 0;
            numThang.Value = DateTime.Now.Month;
            numQuy.Value = (DateTime.Now.Month - 1) / 3 + 1;
            int curYear = DateTime.Now.Year;
            numNam.Value = (curYear >= numNam.Minimum && curYear <= numNam.Maximum) ? curYear : 2026;

            await TaiDuLieuBaoCaoAsync();
        }

        private void cboLoaiBaoCao_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Điều chỉnh hiển thị bộ lọc tùy theo loại báo cáo
            switch (cboLoaiBaoCao.SelectedIndex)
            {
                case 0: // Tháng + Năm
                    numThang.Enabled = true;
                    numQuy.Enabled = false;
                    numNam.Enabled = true;
                    break;
                case 1: // Quý + Năm
                    numThang.Enabled = false;
                    numQuy.Enabled = true;
                    numNam.Enabled = true;
                    break;
                case 2: // Năm
                    numThang.Enabled = false;
                    numQuy.Enabled = false;
                    numNam.Enabled = true;
                    break;
                case 3: // Tháng + Năm
                    numThang.Enabled = true;
                    numQuy.Enabled = false;
                    numNam.Enabled = true;
                    break;
                case 4: // Báo cáo Xuất Nhập Tồn Kho (Tháng + Năm)
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
                    case 0: // Top 5 SP bán chạy
                        {
                            dgvBaoCao.Columns.Add("colStt", "STT");
                            dgvBaoCao.Columns.Add("colMaSp", "Mã SP");
                            dgvBaoCao.Columns.Add("colTenSp", "Tên Sản Phẩm");
                            dgvBaoCao.Columns.Add("colLoai", "Phân Loại");
                            dgvBaoCao.Columns.Add("colNhanHieu", "Nhãn Hiệu");
                            dgvBaoCao.Columns.Add("colSoLuong", "Số Lượng Đã Bán");
                            dgvBaoCao.Columns.Add("colDoanhThu", "Tổng Doanh Thu");

                            var data = await _reportClient.GetReportSpBanChayAsync(thang, nam);
                            if (IsDisposed || Disposing) return;
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

                    case 1: // HĐ Nhập lớn nhất trong quý
                        {
                            dgvBaoCao.Columns.Add("colStt", "STT");
                            dgvBaoCao.Columns.Add("colMaHdn", "Mã Hóa Đơn Nhập");
                            dgvBaoCao.Columns.Add("colNgayNhap", "Ngày Nhập Kho");
                            dgvBaoCao.Columns.Add("colNcc", "Nhà Cung Cấp");
                            dgvBaoCao.Columns.Add("colNv", "Nhân Viên Nhập");
                            dgvBaoCao.Columns.Add("colTongTien", "Tổng Tiền Nhập");

                            var data = await _reportClient.GetReportHdnLonNhatAsync(quy, nam);
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

                    case 2: // Doanh số bán theo năm
                        {
                            dgvBaoCao.Columns.Add("colStt", "STT");
                            dgvBaoCao.Columns.Add("colMaHdb", "Mã Hóa Đơn Bán");
                            dgvBaoCao.Columns.Add("colNgayBan", "Ngày Lập HĐ");
                            dgvBaoCao.Columns.Add("colKhach", "Khách Hàng");
                            dgvBaoCao.Columns.Add("colNv", "Nhân Viên Thu Ngân");
                            dgvBaoCao.Columns.Add("colTongTien", "Tổng Tiền HĐ");

                            var data = await _reportClient.GetReportDoanhSoNamAsync(nam);
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

                    case 3: // Top 5 NV doanh số thấp
                        {
                            dgvBaoCao.Columns.Add("colStt", "STT");
                            dgvBaoCao.Columns.Add("colMaNv", "Mã Nhân Viên");
                            dgvBaoCao.Columns.Add("colTenNv", "Họ Tên Nhân Viên");
                            dgvBaoCao.Columns.Add("colGioiTinh", "Giới Tính");
                            dgvBaoCao.Columns.Add("colDienThoai", "Số Điện Thoại");
                            dgvBaoCao.Columns.Add("colDoanhSo", "Doanh Số Đạt Được");

                            var data = await _reportClient.GetReportNvDoanhSoThapAsync(thang, nam);
                            for (int i = 0; i < data.Count; i++)
                            {
                                var item = data[i];
                                dgvBaoCao.Rows.Add(i + 1, item.MaNv, item.TenNv, item.GioiTinh, item.DienThoai, item.TongTienBan.ToString("N0", culture) + " đ");
                            }

                            lblSoDong.Text = $"Top {data.Count} nhân viên cần cải thiện hiệu suất";
                            lblTongKet.Text = $"Tháng {thang}/{nam}";
                        }
                        break;

                    case 4: // Báo cáo tổng hợp xuất - nhập - tồn kho
                        {
                            dgvBaoCao.Columns.Add("colStt", "STT");
                            dgvBaoCao.Columns.Add("colMaSp", "Mã SP");
                            dgvBaoCao.Columns.Add("colTenSp", "Tên Thiết Bị");
                            dgvBaoCao.Columns.Add("colThuongHieu", "Thương Hiệu");
                            dgvBaoCao.Columns.Add("colNhapSl", "Nhập Trong Kỳ");
                            dgvBaoCao.Columns.Add("colNhapTien", "Tiền Nhập");
                            dgvBaoCao.Columns.Add("colXuatSl", "Xuất Trong Kỳ");
                            dgvBaoCao.Columns.Add("colXuatTien", "Doanh Thu Xuất");
                            dgvBaoCao.Columns.Add("colTonCuoiSl", "Tồn Cuối Kỳ");
                            dgvBaoCao.Columns.Add("colTonCuoiTien", "Giá Trị Tồn Kho");
                            dgvBaoCao.Columns.Add("colTrangThai", "Trạng Thái");

                            var data = await _reportClient.GetReportTonKhoAsync(thang, nam);
                            int tongTon = 0;
                            decimal tongGiaTriTon = 0;

                            for (int i = 0; i < data.Count; i++)
                            {
                                var item = data[i];
                                tongTon += item.TonCuoiSl;
                                tongGiaTriTon += item.TonCuoiTien;

                                dgvBaoCao.Rows.Add(
                                    i + 1,
                                    item.MaSp,
                                    item.TenSp,
                                    string.IsNullOrWhiteSpace(item.TenNhanHieu) ? "Chính hãng" : item.TenNhanHieu,
                                    item.NhapSl,
                                    item.NhapTien.ToString("N0", culture) + " đ",
                                    item.XuatSl,
                                    item.XuatTien.ToString("N0", culture) + " đ",
                                    item.TonCuoiSl,
                                    item.TonCuoiTien.ToString("N0", culture) + " đ",
                                    item.TrangThai
                                );
                            }

                            lblSoDong.Text = $"Toàn bộ danh mục ({data.Count} thiết bị - Tổng tồn: {tongTon} máy)";
                            lblTongKet.Text = $"Tổng giá trị tồn kho: {tongGiaTriTon.ToString("N0", culture)} đ";

                            // Đồng thời kích hoạt hiển thị báo cáo tồn kho trên trình duyệt
                            await HienThiBaoCaoTonKhoAsync(thang, nam);
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
                if (!IsDisposed && !Disposing)
                {
                    btnXemBaoCao.Enabled = true;
                    btnXemBaoCao.Text = "🔍 Xem Báo Cáo";
                }
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
                case 4:
                    endpoint = $"api/bao-cao/ton-kho/excel?month={thang}&year={nam}";
                    defaultFileName = $"BaoCao_XuatNhapTon_Thang_{thang}_{nam}.xlsx";
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

                var (success, message) = await _reportClient.DownloadExcelAsync(endpoint, sfd.FileName);

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

        private async void btnBieuMauKho_Click(object sender, EventArgs e)
        {
            int thang = (int)numThang.Value;
            int nam = (int)numNam.Value;
            await HienThiBaoCaoTonKhoAsync(thang, nam);
        }

        private async Task HienThiBaoCaoTonKhoAsync(int thang, int nam)
        {
            try
            {
                btnBieuMauKho.Enabled = false;
                btnBieuMauKho.Text = "⏳ Đang kết xuất...";

                var data = await _reportClient.GetReportTonKhoAsync(thang, nam);
                var list = new List<ItemBieuMauKho>();

                for (int i = 0; i < data.Count; i++)
                {
                    var p = data[i];
                    int tonDauSl = Math.Max(0, p.TonCuoiSl + p.XuatSl - p.NhapSl);
                    decimal donGiaVon = p.TonCuoiSl > 0 ? (p.TonCuoiTien / p.TonCuoiSl) : (p.NhapSl > 0 ? (p.NhapTien / p.NhapSl) : 0);

                    list.Add(new ItemBieuMauKho
                    {
                        Stt = i + 1,
                        MaSp = p.MaSp,
                        TenSp = p.TenSp,
                        ThuongHieu = string.IsNullOrWhiteSpace(p.TenNhanHieu) ? "Chính hãng" : p.TenNhanHieu,
                        TonDauSl = tonDauSl,
                        TonDauTien = tonDauSl * donGiaVon,
                        NhapSl = p.NhapSl,
                        NhapTien = p.NhapTien,
                        XuatSl = p.XuatSl,
                        XuatTien = p.XuatTien,
                        TonCuoiSl = p.TonCuoiSl,
                        TonCuoiTien = p.TonCuoiTien,
                        TrangThai = p.TrangThai
                    });
                }

                string maBc = $"BC-XNT-{nam}-{thang:D2}";
                string html = BieuMauService.TaoHtmlBaoCaoKho(maBc, thang, nam, list);
                BieuMauService.MoBieuMauTrenTrinhDuyet(html, maBc);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hiển thị báo cáo tồn kho: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnBieuMauKho.Enabled = true;
                btnBieuMauKho.Text = "📋 Báo Cáo Xuất Nhập Tồn";
            }
        }

        private void MoVisualReportViewer()
        {
            if (dgvBaoCao.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng nhấn 'Xem Báo Cáo' để tải dữ liệu trước khi mở chế độ xem trực quan!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var dt = new System.Data.DataTable();
            foreach (DataGridViewColumn col in dgvBaoCao.Columns)
            {
                dt.Columns.Add(col.HeaderText);
            }
            foreach (DataGridViewRow row in dgvBaoCao.Rows)
            {
                if (row.IsNewRow) continue;
                var r = dt.NewRow();
                for (int i = 0; i < dgvBaoCao.Columns.Count; i++)
                {
                    r[i] = row.Cells[i].Value?.ToString() ?? "";
                }
                dt.Rows.Add(r);
            }

            string tieuDe = cboLoaiBaoCao.SelectedItem?.ToString() ?? "BÁO CÁO THỐNG KÊ DOANH THU";
            string thoiGian = $"Tháng {numThang.Value} / Năm {numNam.Value}";
            if (cboLoaiBaoCao.SelectedIndex == 1) thoiGian = $"Quý {numQuy.Value} / Năm {numNam.Value}";
            else if (cboLoaiBaoCao.SelectedIndex == 2) thoiGian = $"Năm {numNam.Value}";

            using var viewer = new frmVisualReportViewer(tieuDe, thoiGian, dt, lblTongKet.Text);
            viewer.ShowDialog(this);
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
