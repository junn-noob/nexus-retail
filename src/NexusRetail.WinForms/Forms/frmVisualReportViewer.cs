using System.Data;
using System.Drawing.Printing;
using NexusRetail.WinForms.UI;

namespace NexusRetail.WinForms.Forms
{
    /// <summary>
    /// Trình xem và in báo cáo trực quan trên nền WinForms (PrintDocument / xuất Excel).
    /// </summary>
    public class frmVisualReportViewer : Form
    {
        private readonly string _tieuDeBaoCao;
        private readonly string _thoiGianBaoCao;
        private readonly DataTable _duLieuBaoCao;
        private readonly string _thongTinTongKet;

        private int _printRowIndex;
        private int _printPageNumber;

        private Panel pnlToolbar = null!;
        private Panel pnlReportCanvas = null!;
        private Panel pnlReportPage = null!;
        private Label lblReportHeader = null!;
        private Label lblSubReportHeader = null!;
        private DataGridView dgvReportData = null!;
        private Panel pnlReportFooter = null!;
        private Label lblTongKetTien = null!;
        private Label lblChuKyNguoiLap = null!;
        private Label lblChuKyThuTruong = null!;
        private Button btnPrint = null!;
        private Button btnExportExcel = null!;
        private Button btnClose = null!;
        private Label lblPageInfo = null!;

        public frmVisualReportViewer(string tieuDeBaoCao, string thoiGianBaoCao, DataTable duLieuBaoCao, string thongTinTongKet)
        {
            _tieuDeBaoCao = tieuDeBaoCao;
            _thoiGianBaoCao = thoiGianBaoCao;
            _duLieuBaoCao = duLieuBaoCao;
            _thongTinTongKet = thongTinTongKet;
            InitializeComponentManual(tieuDeBaoCao, thoiGianBaoCao, duLieuBaoCao, thongTinTongKet);
        }

        private void InitializeComponentManual(string tieuDe, string thoiGian, DataTable data, string tongKet)
        {
            Text = $"Trình xem báo cáo - {tieuDe}";
            Size = new Size(1050, 780);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(241, 245, 249);
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);

            pnlToolbar = new Panel
            {
                Dock = DockStyle.Top,
                Size = new Size(ClientSize.Width, 48),
                BackColor = Color.FromArgb(15, 34, 64),
                Padding = new Padding(12, 6, 12, 6)
            };

            btnPrint = new Button
            {
                Text = "🖨 In Báo Cáo",
                Size = new Size(120, 34),
                Location = new Point(12, 7),
                BackColor = Color.FromArgb(0, 161, 55),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPrint.FlatAppearance.BorderSize = 0;
            btnPrint.Click += (s, e) => InBaoCao();

            btnExportExcel = new Button
            {
                Text = "📥 Xuất Excel",
                Size = new Size(110, 34),
                Location = new Point(140, 7),
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnExportExcel.FlatAppearance.BorderSize = 0;
            btnExportExcel.Click += (s, e) => MessageBox.Show("Vui lòng sử dụng tính năng 'Xuất File Excel (.xlsx)' trên màn hình Thống kê để tải tệp ClosedXML chuẩn định dạng kế toán!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            lblPageInfo = new Label
            {
                Text = "📄 Trang 1 / 1  |  Tỷ lệ: 100% (Chuẩn A4 dọc)",
                ForeColor = Color.FromArgb(203, 213, 225),
                AutoSize = true,
                Location = new Point(270, 15),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular)
            };

            btnClose = new Button
            {
                Text = "❌ Đóng",
                Size = new Size(90, 34),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(pnlToolbar.Width - 102, 7),
                BackColor = Color.FromArgb(71, 85, 105),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => Close();

            pnlToolbar.Controls.Add(btnPrint);
            pnlToolbar.Controls.Add(btnExportExcel);
            pnlToolbar.Controls.Add(lblPageInfo);
            pnlToolbar.Controls.Add(btnClose);

            pnlReportCanvas = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(226, 232, 240),
                Padding = new Padding(16)
            };

            pnlReportPage = new Panel
            {
                Size = new Size(920, 665),
                Location = new Point(45, 12),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(24)
            };

            var lblDonVi = new Label
            {
                Text = "DUY MOBILE • HỆ THỐNG BÁN LẺ THIẾT BỊ DI ĐỘNG\nNỀN TẢNG QUẢN LÝ BÁN HÀNG & KHO NEXUS RETAIL (POS)",
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                TextAlign = ContentAlignment.TopCenter
            };

            lblReportHeader = new Label
            {
                Text = tieuDe.ToUpper(),
                Dock = DockStyle.Top,
                Height = 36,
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 34, 64),
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblSubReportHeader = new Label
            {
                Text = $"Thời gian báo cáo: {thoiGian}  •  Đơn vị tiền tệ: Việt Nam Đồng (VNĐ)",
                Dock = DockStyle.Top,
                Height = 26,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // SECTION 2: DETAILS (LƯỚI DỮ LIỆU BÁO CÁO)
            dgvReportData = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 300,
                DataSource = data,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            UiTheme.ApplyModernGrid(dgvReportData);

            // SECTION 3: REPORT FOOTER & CHỮ KÝ
            pnlReportFooter = new Panel
            {
                Dock = DockStyle.Top,
                Height = 165,
                Padding = new Padding(8)
            };

            lblTongKetTien = new Label
            {
                Text = $"TỔNG CỘNG: {tongKet}",
                Dock = DockStyle.Top,
                Height = 32,
                Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 136, 46),
                TextAlign = ContentAlignment.MiddleRight
            };

            lblChuKyNguoiLap = new Label
            {
                Text = $"Hà Nội, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}\nNGƯỜI LẬP BIỂU\n(Ký và ghi rõ họ tên)\n\n\n\nNguyễn Văn Hùng",
                Size = new Size(300, 105),
                Location = new Point(40, 50),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                TextAlign = ContentAlignment.TopCenter
            };

            lblChuKyThuTruong = new Label
            {
                Text = "CHỦ CƠ SỞ / QUẢN LÝ PHÊ DUYỆT\n(Ký, đóng dấu và ghi rõ họ tên)\n\n\n\nNguyễn Thành Duy",
                Size = new Size(300, 105),
                Location = new Point(520, 50),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                TextAlign = ContentAlignment.TopCenter
            };

            pnlReportFooter.Controls.Add(lblTongKetTien);
            pnlReportFooter.Controls.Add(lblChuKyNguoiLap);
            pnlReportFooter.Controls.Add(lblChuKyThuTruong);

            pnlReportPage.Controls.Add(pnlReportFooter);
            pnlReportPage.Controls.Add(dgvReportData);
            pnlReportPage.Controls.Add(lblSubReportHeader);
            pnlReportPage.Controls.Add(lblReportHeader);
            pnlReportPage.Controls.Add(lblDonVi);

            pnlReportCanvas.Controls.Add(pnlReportPage);

            Controls.Add(pnlReportCanvas);
            Controls.Add(pnlToolbar);
        }

        private void InBaoCao()
        {
            try
            {
                _printRowIndex = 0;
                _printPageNumber = 0;

                using var pd = new PrintDocument();
                pd.DocumentName = _tieuDeBaoCao;
                pd.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169);
                pd.DefaultPageSettings.Margins = new Margins(50, 50, 50, 50);

                pd.BeginPrint += (_, _) =>
                {
                    _printRowIndex = 0;
                    _printPageNumber = 0;
                };
                pd.PrintPage += DrawReportPage;

                using var previewDlg = new PrintPreviewDialog
                {
                    Document = pd,
                    Text = $"Xem trước bản in (Vector A4) - {_tieuDeBaoCao}",
                    Width = 1024,
                    Height = 768,
                    UseAntiAlias = true
                };
                previewDlg.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lệnh in ấn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DrawReportPage(object? sender, PrintPageEventArgs ev)
        {
            var g = ev.Graphics;
            if (g == null)
            {
                ev.HasMorePages = false;
                return;
            }

            _printPageNumber++;
            Rectangle margin = ev.MarginBounds;
            float y = margin.Top;

            using var fontCompany = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            using var fontTitle = new Font("Segoe UI", 14f, FontStyle.Bold);
            using var fontSub = new Font("Segoe UI", 9f, FontStyle.Italic);
            using var fontHeader = new Font("Segoe UI", 9f, FontStyle.Bold);
            using var fontCell = new Font("Segoe UI", 9f, FontStyle.Regular);
            using var fontTotal = new Font("Segoe UI", 10.5f, FontStyle.Bold);

            using var brushNavy = new SolidBrush(Color.FromArgb(15, 34, 64));
            using var brushMuted = new SolidBrush(Color.FromArgb(100, 116, 139));
            using var brushHeaderBg = new SolidBrush(Color.FromArgb(241, 245, 249));
            using var brushZebra = new SolidBrush(Color.FromArgb(248, 250, 252));
            using var brushGreen = new SolidBrush(Color.FromArgb(0, 136, 46));
            using var penBorder = new Pen(Color.FromArgb(203, 213, 225), 0.75f);

            var centerFmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            var leftFmt = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
            var rightFmt = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };

            // Report Header (page 1 full header; continuation header on subsequent pages)
            if (_printPageNumber == 1)
            {
                g.DrawString("HỆ THỐNG BÁN LẺ NEXUS RETAIL  •  BÁO CÁO QUẢN TRỊ", fontCompany, brushMuted, new RectangleF(margin.Left, y, margin.Width, 20), centerFmt);
                y += 24;
                g.DrawString(_tieuDeBaoCao.ToUpperInvariant(), fontTitle, brushNavy, new RectangleF(margin.Left, y, margin.Width, 28), centerFmt);
                y += 30;
                g.DrawString($"Thời gian báo cáo: {_thoiGianBaoCao}  •  Đơn vị: VNĐ", fontSub, brushMuted, new RectangleF(margin.Left, y, margin.Width, 20), centerFmt);
                y += 28;
            }
            else
            {
                g.DrawString($"{_tieuDeBaoCao.ToUpperInvariant()} (Tiếp theo)", fontHeader, brushNavy, new RectangleF(margin.Left, y, margin.Width, 22), leftFmt);
                y += 26;
            }

            // Table column layout
            int colCount = Math.Max(1, _duLieuBaoCao.Columns.Count);
            float colWidth = (float)margin.Width / colCount;
            const float headerHeight = 28f;
            const float rowHeight = 24f;

            // Table Header
            for (int c = 0; c < colCount; c++)
            {
                var cellRect = new RectangleF(margin.Left + c * colWidth, y, colWidth, headerHeight);
                g.FillRectangle(brushHeaderBg, cellRect);
                g.DrawRectangle(penBorder, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                string headerText = _duLieuBaoCao.Columns[c].ColumnName;
                g.DrawString(headerText, fontHeader, brushNavy, RectangleF.Inflate(cellRect, -4, 0), centerFmt);
            }
            y += headerHeight;

            // Table Rows (reserve 35px at bottom for page number footer)
            float maxRowBottom = margin.Bottom - 35f;
            while (_printRowIndex < _duLieuBaoCao.Rows.Count)
            {
                if (y + rowHeight > maxRowBottom)
                {
                    DrawPageFooter(g, margin, fontSub, brushMuted, _printPageNumber);
                    ev.HasMorePages = true;
                    return;
                }

                DataRow row = _duLieuBaoCao.Rows[_printRowIndex];
                bool isAlt = _printRowIndex % 2 == 1;
                for (int c = 0; c < colCount; c++)
                {
                    var cellRect = new RectangleF(margin.Left + c * colWidth, y, colWidth, rowHeight);
                    if (isAlt) g.FillRectangle(brushZebra, cellRect);
                    g.DrawRectangle(penBorder, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);

                    object rawVal = row[c];
                    bool isNumeric = rawVal is decimal or double or float or int or long;
                    string cellText = rawVal switch
                    {
                        decimal d => d.ToString("#,##0"),
                        double dbl => dbl.ToString("#,##0"),
                        DateTime dt => dt.ToString("dd/MM/yyyy"),
                        _ => rawVal?.ToString() ?? string.Empty
                    };

                    g.DrawString(cellText, fontCell, Brushes.Black, RectangleF.Inflate(cellRect, -4, 0), isNumeric ? rightFmt : leftFmt);
                }

                y += rowHeight;
                _printRowIndex++;
            }

            // Summary & Signatures Section (start new page if insufficient room)
            const float footerBlockHeight = 135f;
            if (y + footerBlockHeight > maxRowBottom)
            {
                DrawPageFooter(g, margin, fontSub, brushMuted, _printPageNumber);
                ev.HasMorePages = true;
                return;
            }

            y += 12;
            g.DrawString($"TỔNG CỘNG: {_thongTinTongKet}", fontTotal, brushGreen, new RectangleF(margin.Left, y, margin.Width, 24), rightFmt);
            y += 32;

            float sigWidth = margin.Width / 2f;
            string leftSig = $"Hà Nội, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}\nNGƯỜI LẬP BIỂU\n(Ký và ghi rõ họ tên)\n\n\nNguyễn Văn Hùng";
            string rightSig = "\nCHỦ CƠ SỞ / QUẢN LÝ PHÊ DUYỆT\n(Ký, đóng dấu và ghi rõ họ tên)\n\n\nNguyễn Thành Duy";
            g.DrawString(leftSig, fontCell, Brushes.Black, new RectangleF(margin.Left, y, sigWidth, 95), centerFmt);
            g.DrawString(rightSig, fontCell, Brushes.Black, new RectangleF(margin.Left + sigWidth, y, sigWidth, 95), centerFmt);

            DrawPageFooter(g, margin, fontSub, brushMuted, _printPageNumber);
            ev.HasMorePages = false;
        }

        private static void DrawPageFooter(Graphics g, Rectangle margin, Font font, Brush brush, int pageNumber)
        {
            var fmt = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
            g.DrawString($"In lúc {DateTime.Now:dd/MM/yyyy HH:mm}  •  Trang {pageNumber}", font, brush, new RectangleF(margin.Left, margin.Bottom - 22, margin.Width, 20), fmt);
        }
    }
}

