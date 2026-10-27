namespace NexusRetail.WinForms.Forms
{
    partial class frmBaoCaoThongKe
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            pnlHeader = new Panel();
            lblSubTitle = new Label();
            lblTitle = new Label();
            pnlControls = new Panel();
            btnBieuMauKho = new Button();
            btnXuatExcel = new Button();
            btnXemBaoCao = new Button();
            numNam = new NumericUpDown();
            lblNam = new Label();
            numQuy = new NumericUpDown();
            lblQuy = new Label();
            numThang = new NumericUpDown();
            lblThang = new Label();
            cboLoaiBaoCao = new ComboBox();
            lblLoaiBaoCao = new Label();
            dgvBaoCao = new DataGridView();
            pnlFooter = new Panel();
            btnDong = new Button();
            lblTongKet = new Label();
            lblSoDong = new Label();
            pnlHeader.SuspendLayout();
            pnlControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numNam).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numQuy).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numThang).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvBaoCao).BeginInit();
            pnlFooter.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(15, 23, 42);
            pnlHeader.Controls.Add(lblSubTitle);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Margin = new Padding(4, 5, 4, 5);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1811, 125);
            pnlHeader.TabIndex = 0;
            // 
            // lblSubTitle
            // 
            lblSubTitle.AutoSize = true;
            lblSubTitle.Font = new Font("Segoe UI", 9.5F);
            lblSubTitle.ForeColor = Color.FromArgb(148, 163, 184);
            lblSubTitle.Location = new Point(34, 72);
            lblSubTitle.Margin = new Padding(4, 0, 4, 0);
            lblSubTitle.Name = "lblSubTitle";
            lblSubTitle.Size = new Size(644, 25);
            lblSubTitle.TabIndex = 1;
            lblSubTitle.Text = "Thống kê doanh số bán hàng, đơn nhập kho lớn nhất và xuất báo cáo Excel";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(31, 18);
            lblTitle.Margin = new Padding(4, 0, 4, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(630, 45);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "BÁO CÁO THỐNG KÊ DOANH THU & KHO";
            // 
            // pnlControls
            // 
            pnlControls.BackColor = Color.White;
            pnlControls.BorderStyle = BorderStyle.FixedSingle;
            pnlControls.Controls.Add(btnBieuMauKho);
            pnlControls.Controls.Add(btnXuatExcel);
            pnlControls.Controls.Add(btnXemBaoCao);
            pnlControls.Controls.Add(numNam);
            pnlControls.Controls.Add(lblNam);
            pnlControls.Controls.Add(numQuy);
            pnlControls.Controls.Add(lblQuy);
            pnlControls.Controls.Add(numThang);
            pnlControls.Controls.Add(lblThang);
            pnlControls.Controls.Add(cboLoaiBaoCao);
            pnlControls.Controls.Add(lblLoaiBaoCao);
            pnlControls.Dock = DockStyle.Top;
            pnlControls.Location = new Point(0, 125);
            pnlControls.Margin = new Padding(4, 5, 4, 5);
            pnlControls.Name = "pnlControls";
            pnlControls.Padding = new Padding(23, 20, 23, 20);
            pnlControls.Size = new Size(1811, 132);
            pnlControls.TabIndex = 1;
            // 
            // btnBieuMauKho
            // 
            btnBieuMauKho.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBieuMauKho.BackColor = Color.FromArgb(3, 105, 161);
            btnBieuMauKho.Cursor = Cursors.Hand;
            btnBieuMauKho.FlatAppearance.BorderSize = 0;
            btnBieuMauKho.FlatStyle = FlatStyle.Flat;
            btnBieuMauKho.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnBieuMauKho.ForeColor = Color.White;
            btnBieuMauKho.Location = new Point(1583, 33);
            btnBieuMauKho.Margin = new Padding(4, 5, 4, 5);
            btnBieuMauKho.Name = "btnBieuMauKho";
            btnBieuMauKho.Size = new Size(199, 60);
            btnBieuMauKho.TabIndex = 10;
            btnBieuMauKho.Text = "📋 Biểu Mẫu BM03";
            btnBieuMauKho.UseVisualStyleBackColor = false;
            btnBieuMauKho.Click += btnBieuMauKho_Click;
            // 
            // btnXuatExcel
            // 
            btnXuatExcel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnXuatExcel.BackColor = Color.FromArgb(5, 150, 105);
            btnXuatExcel.Cursor = Cursors.Hand;
            btnXuatExcel.FlatAppearance.BorderSize = 0;
            btnXuatExcel.FlatStyle = FlatStyle.Flat;
            btnXuatExcel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnXuatExcel.ForeColor = Color.White;
            btnXuatExcel.Location = new Point(1390, 33);
            btnXuatExcel.Margin = new Padding(4, 5, 4, 5);
            btnXuatExcel.Name = "btnXuatExcel";
            btnXuatExcel.Size = new Size(171, 60);
            btnXuatExcel.TabIndex = 9;
            btnXuatExcel.Text = "📊 Xuất Excel";
            btnXuatExcel.UseVisualStyleBackColor = false;
            btnXuatExcel.Click += btnXuatExcel_Click;
            // 
            // btnXemBaoCao
            // 
            btnXemBaoCao.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnXemBaoCao.BackColor = Color.FromArgb(79, 70, 229);
            btnXemBaoCao.Cursor = Cursors.Hand;
            btnXemBaoCao.FlatAppearance.BorderSize = 0;
            btnXemBaoCao.FlatStyle = FlatStyle.Flat;
            btnXemBaoCao.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnXemBaoCao.ForeColor = Color.White;
            btnXemBaoCao.Location = new Point(1196, 33);
            btnXemBaoCao.Margin = new Padding(4, 5, 4, 5);
            btnXemBaoCao.Name = "btnXemBaoCao";
            btnXemBaoCao.Size = new Size(171, 60);
            btnXemBaoCao.TabIndex = 8;
            btnXemBaoCao.Text = "🔍 Xem Báo Cáo";
            btnXemBaoCao.UseVisualStyleBackColor = false;
            btnXemBaoCao.Click += btnXemBaoCao_Click;
            // 
            // numNam
            // 
            numNam.Font = new Font("Segoe UI", 9.5F);
            numNam.Location = new Point(970, 47);
            numNam.Margin = new Padding(4, 5, 4, 5);
            numNam.Maximum = new decimal(new int[] { 2035, 0, 0, 0 });
            numNam.Minimum = new decimal(new int[] { 2020, 0, 0, 0 });
            numNam.Name = "numNam";
            numNam.Size = new Size(107, 33);
            numNam.TabIndex = 7;
            numNam.Value = new decimal(new int[] { 2026, 0, 0, 0 });
            // 
            // lblNam
            // 
            lblNam.AutoSize = true;
            lblNam.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblNam.ForeColor = Color.FromArgb(51, 65, 85);
            lblNam.Location = new Point(905, 55);
            lblNam.Margin = new Padding(4, 0, 4, 0);
            lblNam.Name = "lblNam";
            lblNam.Size = new Size(57, 25);
            lblNam.TabIndex = 6;
            lblNam.Text = "Năm:";
            // 
            // numQuy
            // 
            numQuy.Font = new Font("Segoe UI", 9.5F);
            numQuy.Location = new Point(826, 47);
            numQuy.Margin = new Padding(4, 5, 4, 5);
            numQuy.Maximum = new decimal(new int[] { 4, 0, 0, 0 });
            numQuy.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numQuy.Name = "numQuy";
            numQuy.Size = new Size(71, 33);
            numQuy.TabIndex = 5;
            numQuy.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // lblQuy
            // 
            lblQuy.AutoSize = true;
            lblQuy.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblQuy.ForeColor = Color.FromArgb(51, 65, 85);
            lblQuy.Location = new Point(766, 55);
            lblQuy.Margin = new Padding(4, 0, 4, 0);
            lblQuy.Name = "lblQuy";
            lblQuy.Size = new Size(52, 25);
            lblQuy.TabIndex = 4;
            lblQuy.Text = "Quý:";
            // 
            // numThang
            // 
            numThang.Font = new Font("Segoe UI", 9.5F);
            numThang.Location = new Point(687, 47);
            numThang.Margin = new Padding(4, 5, 4, 5);
            numThang.Maximum = new decimal(new int[] { 12, 0, 0, 0 });
            numThang.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numThang.Name = "numThang";
            numThang.Size = new Size(71, 33);
            numThang.TabIndex = 3;
            numThang.Value = new decimal(new int[] { 9, 0, 0, 0 });
            // 
            // lblThang
            // 
            lblThang.AutoSize = true;
            lblThang.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblThang.ForeColor = Color.FromArgb(51, 65, 85);
            lblThang.Location = new Point(608, 55);
            lblThang.Margin = new Padding(4, 0, 4, 0);
            lblThang.Name = "lblThang";
            lblThang.Size = new Size(71, 25);
            lblThang.TabIndex = 2;
            lblThang.Text = "Tháng:";
            // 
            // cboLoaiBaoCao
            // 
            cboLoaiBaoCao.DropDownStyle = ComboBoxStyle.DropDownList;
            cboLoaiBaoCao.Font = new Font("Segoe UI", 9.5F);
            cboLoaiBaoCao.FormattingEnabled = true;
            cboLoaiBaoCao.Items.AddRange(new object[] { "Top 5 sản phẩm bán chạy nhất trong tháng", "Hóa đơn nhập hàng lớn nhất trong quý", "Tổng kết doanh số bán theo năm", "Top 5 nhân viên có doanh số thấp nhất tháng", "Báo cáo tổng hợp xuất - nhập - tồn kho (Biểu mẫu BM03)" });
            cboLoaiBaoCao.Location = new Point(164, 48);
            cboLoaiBaoCao.Margin = new Padding(4, 5, 4, 5);
            cboLoaiBaoCao.Name = "cboLoaiBaoCao";
            cboLoaiBaoCao.Size = new Size(436, 33);
            cboLoaiBaoCao.TabIndex = 1;
            cboLoaiBaoCao.SelectedIndexChanged += cboLoaiBaoCao_SelectedIndexChanged;
            // 
            // lblLoaiBaoCao
            // 
            lblLoaiBaoCao.AutoSize = true;
            lblLoaiBaoCao.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblLoaiBaoCao.ForeColor = Color.FromArgb(51, 65, 85);
            lblLoaiBaoCao.Location = new Point(29, 55);
            lblLoaiBaoCao.Margin = new Padding(4, 0, 4, 0);
            lblLoaiBaoCao.Name = "lblLoaiBaoCao";
            lblLoaiBaoCao.Size = new Size(124, 25);
            lblLoaiBaoCao.TabIndex = 0;
            lblLoaiBaoCao.Text = "Loại báo cáo:";
            // 
            // dgvBaoCao
            // 
            dgvBaoCao.AllowUserToAddRows = false;
            dgvBaoCao.AllowUserToDeleteRows = false;
            dgvBaoCao.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBaoCao.BackgroundColor = Color.FromArgb(248, 250, 252);
            dgvBaoCao.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(241, 245, 249);
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = Color.FromArgb(51, 65, 85);
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvBaoCao.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvBaoCao.ColumnHeadersHeight = 35;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.White;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(224, 231, 255);
            dataGridViewCellStyle2.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dgvBaoCao.DefaultCellStyle = dataGridViewCellStyle2;
            dgvBaoCao.Dock = DockStyle.Fill;
            dgvBaoCao.EnableHeadersVisualStyles = false;
            dgvBaoCao.Location = new Point(0, 257);
            dgvBaoCao.Margin = new Padding(4, 5, 4, 5);
            dgvBaoCao.MultiSelect = false;
            dgvBaoCao.Name = "dgvBaoCao";
            dgvBaoCao.ReadOnly = true;
            dgvBaoCao.RowHeadersVisible = false;
            dgvBaoCao.RowHeadersWidth = 62;
            dgvBaoCao.RowTemplate.Height = 30;
            dgvBaoCao.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBaoCao.Size = new Size(1811, 761);
            dgvBaoCao.TabIndex = 2;
            // 
            // pnlFooter
            // 
            pnlFooter.BackColor = Color.White;
            pnlFooter.BorderStyle = BorderStyle.FixedSingle;
            pnlFooter.Controls.Add(btnDong);
            pnlFooter.Controls.Add(lblTongKet);
            pnlFooter.Controls.Add(lblSoDong);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(0, 1018);
            pnlFooter.Margin = new Padding(4, 5, 4, 5);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Padding = new Padding(23, 20, 23, 20);
            pnlFooter.Size = new Size(1811, 99);
            pnlFooter.TabIndex = 3;
            // 
            // btnDong
            // 
            btnDong.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnDong.BackColor = Color.FromArgb(241, 245, 249);
            btnDong.Cursor = Cursors.Hand;
            btnDong.FlatAppearance.BorderSize = 0;
            btnDong.FlatStyle = FlatStyle.Flat;
            btnDong.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnDong.ForeColor = Color.FromArgb(71, 85, 105);
            btnDong.Location = new Point(1656, 20);
            btnDong.Margin = new Padding(4, 5, 4, 5);
            btnDong.Name = "btnDong";
            btnDong.Size = new Size(136, 57);
            btnDong.TabIndex = 2;
            btnDong.Text = "Đóng";
            btnDong.UseVisualStyleBackColor = false;
            btnDong.Click += btnDong_Click;
            // 
            // lblTongKet
            // 
            lblTongKet.AutoSize = true;
            lblTongKet.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblTongKet.ForeColor = Color.FromArgb(220, 38, 38);
            lblTongKet.Location = new Point(314, 30);
            lblTongKet.Margin = new Padding(4, 0, 4, 0);
            lblTongKet.Name = "lblTongKet";
            lblTongKet.Size = new Size(179, 30);
            lblTongKet.TabIndex = 1;
            lblTongKet.Text = "Tổng giá trị: 0 đ";
            // 
            // lblSoDong
            // 
            lblSoDong.AutoSize = true;
            lblSoDong.Font = new Font("Segoe UI", 9.5F);
            lblSoDong.ForeColor = Color.FromArgb(71, 85, 105);
            lblSoDong.Location = new Point(29, 33);
            lblSoDong.Margin = new Padding(4, 0, 4, 0);
            lblSoDong.Name = "lblSoDong";
            lblSoDong.Size = new Size(165, 25);
            lblSoDong.TabIndex = 0;
            lblSoDong.Text = "Tổng số kết quả: 0";
            // 
            // frmBaoCaoThongKe
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1811, 1117);
            Controls.Add(dgvBaoCao);
            Controls.Add(pnlFooter);
            Controls.Add(pnlControls);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9F);
            Margin = new Padding(4, 5, 4, 5);
            MinimumSize = new Size(1348, 963);
            Name = "frmBaoCaoThongKe";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Nexus Retail - Báo Cáo Thống Kê & Phân Tích Nghiệp Vụ";
            Load += frmBaoCaoThongKe_Load;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlControls.ResumeLayout(false);
            pnlControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numNam).EndInit();
            ((System.ComponentModel.ISupportInitialize)numQuy).EndInit();
            ((System.ComponentModel.ISupportInitialize)numThang).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvBaoCao).EndInit();
            pnlFooter.ResumeLayout(false);
            pnlFooter.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblSubTitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlControls;
        private System.Windows.Forms.Label lblLoaiBaoCao;
        private System.Windows.Forms.ComboBox cboLoaiBaoCao;
        private System.Windows.Forms.Label lblThang;
        private System.Windows.Forms.NumericUpDown numThang;
        private System.Windows.Forms.Label lblQuy;
        private System.Windows.Forms.NumericUpDown numQuy;
        private System.Windows.Forms.Label lblNam;
        private System.Windows.Forms.NumericUpDown numNam;
        private System.Windows.Forms.Button btnXemBaoCao;
        private System.Windows.Forms.Button btnXuatExcel;
        private System.Windows.Forms.Button btnBieuMauKho;
        private System.Windows.Forms.DataGridView dgvBaoCao;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Label lblSoDong;
        private System.Windows.Forms.Label lblTongKet;
        private System.Windows.Forms.Button btnDong;
    }
}
