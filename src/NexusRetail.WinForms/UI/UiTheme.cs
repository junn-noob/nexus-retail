using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

namespace NexusRetail.WinForms.UI
{
    /// <summary>
    /// Modern Enterprise UI Theme Engine for NexusRetail POS
    /// Đem lại giao diện phẳng, hiện đại, màu sắc hài hòa chuẩn KiotViet / Square POS 2026.
    /// </summary>
    public static class UiTheme
    {
        // 1. Color Palette Tokens - Chuẩn KiotViet Brand 2026
        public static readonly Color Primary = Color.FromArgb(0, 161, 55);         // KiotViet Green #00A137
        public static readonly Color PrimaryDark = Color.FromArgb(0, 136, 46);     // #00882E
        public static readonly Color PrimaryLight = Color.FromArgb(232, 248, 238); // #E8F8EE
        public static readonly Color DarkSlate = Color.FromArgb(15, 34, 64);        // KiotViet Dark Navy #0F2240
        public static readonly Color SurfaceNavy = Color.FromArgb(30, 41, 59);      // #1E293B
        public static readonly Color Background = Color.FromArgb(241, 245, 249);    // #F1F5F9
        public static readonly Color CardWhite = Color.FromArgb(255, 255, 255);
        public static readonly Color BorderLight = Color.FromArgb(226, 232, 240);   // #E2E8F0
        public static readonly Color Success = Color.FromArgb(0, 161, 55);          // KiotViet Green
        public static readonly Color SuccessDark = Color.FromArgb(0, 136, 46);
        public static readonly Color Warning = Color.FromArgb(245, 158, 11);        // #F59E0B Amber
        public static readonly Color Danger = Color.FromArgb(220, 38, 38);          // #DC2626
        public static readonly Color DangerDark = Color.FromArgb(185, 28, 28);
        public static readonly Color TextMain = Color.FromArgb(15, 23, 42);
        public static readonly Color TextMuted = Color.FromArgb(100, 116, 139);     // #64748B
        public static readonly Color RowAlternate = Color.FromArgb(248, 250, 252);
        public static readonly Color SelectionRow = Color.FromArgb(232, 248, 238);  // Kiot Soft Green

        // 2. Typography
        public static readonly Font FontSm = new Font("Segoe UI", 9f, FontStyle.Regular);
        public static readonly Font FontRegular = new Font("Segoe UI", 10f, FontStyle.Regular);
        public static readonly Font FontBold = new Font("Segoe UI", 10f, FontStyle.Bold);
        public static readonly Font FontButton = new Font("Segoe UI", 10.5f, FontStyle.Bold);
        public static readonly Font FontHeader = new Font("Segoe UI", 14f, FontStyle.Bold);
        public static readonly Font FontTitle = new Font("Segoe UI", 16f, FontStyle.Bold);
        public static readonly Font FontDisplay = new Font("Segoe UI", 20f, FontStyle.Bold);

        /// <summary>
        /// Kích hoạt DoubleBuffering cho DataGridView để cuộn mượt mà 60fps, không nhấp nháy.
        /// </summary>
        public static void EnableDoubleBuffer(DataGridView dgv)
        {
            try
            {
                typeof(DataGridView).InvokeMember("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                    null, dgv, new object[] { true });
            }
            catch { }
        }

        /// <summary>
        /// Tạo giao diện bảng dữ liệu chuẩn ERP / POS thương mại: Header đen sang trọng, dòng cao thoáng, highlight hàng thông minh.
        /// </summary>
        public static void ApplyModernGrid(DataGridView dgv)
        {
            if (dgv == null) return;

            EnableDoubleBuffer(dgv);

            dgv.BackgroundColor = CardWhite;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = BorderLight;
            dgv.EnableHeadersVisualStyles = false;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AllowUserToResizeRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Header Style
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 42;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = DarkSlate;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

            // Row Style
            dgv.RowTemplate.Height = 38;
            dgv.DefaultCellStyle.BackColor = CardWhite;
            dgv.DefaultCellStyle.ForeColor = TextMain;
            dgv.DefaultCellStyle.Font = FontRegular;
            dgv.DefaultCellStyle.SelectionBackColor = SelectionRow;
            dgv.DefaultCellStyle.SelectionForeColor = PrimaryDark;
            dgv.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);

            // Alternating Rows
            dgv.AlternatingRowsDefaultCellStyle.BackColor = RowAlternate;
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = TextMain;
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = SelectionRow;
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = PrimaryDark;
        }

        /// <summary>
        /// Nút hành động chính (Primary Action Button) - Nền xanh Indigo hiện đại
        /// </summary>
        public static void ApplyPrimaryButton(Button btn, string? text = null)
        {
            if (btn == null) return;
            if (!string.IsNullOrEmpty(text)) btn.Text = text;

            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Primary;
            btn.ForeColor = Color.White;
            btn.Font = FontButton;
            btn.Cursor = Cursors.Hand;
            btn.FlatAppearance.MouseOverBackColor = PrimaryDark;
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(55, 48, 163);
        }

        /// <summary>
        /// Nút hành động thành công (Thanh toán, Lưu, Thêm mới) - Xanh lá Emerald
        /// </summary>
        public static void ApplySuccessButton(Button btn, string? text = null)
        {
            if (btn == null) return;
            if (!string.IsNullOrEmpty(text)) btn.Text = text;

            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Success;
            btn.ForeColor = Color.White;
            btn.Font = FontButton;
            btn.Cursor = Cursors.Hand;
            btn.FlatAppearance.MouseOverBackColor = SuccessDark;
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(4, 120, 87);
        }

        /// <summary>
        /// Nút hành động nguy hiểm / hủy / xóa - Đỏ tươi Rose
        /// </summary>
        public static void ApplyDangerButton(Button btn, string? text = null)
        {
            if (btn == null) return;
            if (!string.IsNullOrEmpty(text)) btn.Text = text;

            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Danger;
            btn.ForeColor = Color.White;
            btn.Font = FontButton;
            btn.Cursor = Cursors.Hand;
            btn.FlatAppearance.MouseOverBackColor = DangerDark;
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(185, 28, 28);
        }

        /// <summary>
        /// Nút thứ cấp (Làm mới, Đóng, Tìm kiếm) - Trắng viền xám nhẹ
        /// </summary>
        public static void ApplySecondaryButton(Button btn, string? text = null)
        {
            if (btn == null) return;
            if (!string.IsNullOrEmpty(text)) btn.Text = text;

            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btn.FlatAppearance.BorderSize = 1;
            btn.BackColor = Color.White;
            btn.ForeColor = TextMain;
            btn.Font = FontButton;
            btn.Cursor = Cursors.Hand;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 245, 249);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(226, 232, 240);
        }

        /// <summary>
        /// Định dạng Header chuẩn thương mại của Form: nền tối sang trọng, tiêu đề trắng nổi bật, phụ đề xám xanh.
        /// </summary>
        public static void ApplyFormHeader(Panel pnl, Label lblTitle, Label lblSub, string? title = null, string? sub = null)
        {
            if (pnl != null)
            {
                pnl.BackColor = DarkSlate;
                pnl.Height = Math.Max(pnl.Height, 72);
            }
            if (lblTitle != null)
            {
                if (!string.IsNullOrEmpty(title)) lblTitle.Text = title;
                lblTitle.Font = FontHeader;
                lblTitle.ForeColor = Color.White;
            }
            if (lblSub != null)
            {
                if (!string.IsNullOrEmpty(sub)) lblSub.Text = sub;
                lblSub.Font = FontSm;
                lblSub.ForeColor = Color.FromArgb(148, 163, 184);
            }
        }

        /// <summary>
        /// Quét và làm đẹp toàn bộ controls trên Form tự động (Form Load hook)
        /// </summary>
        public static void BeautifyForm(Form form)
        {
            if (form == null) return;

            form.Font = FontRegular;
            form.BackColor = Background;

            BeautifyControlsRecursive(form);
        }

        private static void BeautifyControlsRecursive(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is DataGridView dgv)
                {
                    ApplyModernGrid(dgv);
                }
                else if (c is TextBox txt)
                {
                    txt.Font = new Font("Segoe UI", 10f);
                    txt.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (c is ComboBox cbo)
                {
                    cbo.Font = new Font("Segoe UI", 10f);
                    cbo.FlatStyle = FlatStyle.Flat;
                }
                else if (c is NumericUpDown num)
                {
                    num.Font = new Font("Segoe UI", 10f);
                    num.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (c is GroupBox grp)
                {
                    grp.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                    grp.ForeColor = PrimaryDark;
                    grp.BackColor = CardWhite;
                }

                if (c.HasChildren)
                {
                    BeautifyControlsRecursive(c);
                }
            }
        }
    }
}
