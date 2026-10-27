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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.pnlHeaderAccent = new System.Windows.Forms.Panel();
            this.lblSubTitle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlControls = new System.Windows.Forms.Panel();
            this.btnBieuMauKho = new System.Windows.Forms.Button();
            this.btnXuatExcel = new System.Windows.Forms.Button();
            this.btnXemBaoCao = new System.Windows.Forms.Button();
            this.numNam = new System.Windows.Forms.NumericUpDown();
            this.lblNam = new System.Windows.Forms.Label();
            this.numQuy = new System.Windows.Forms.NumericUpDown();
            this.lblQuy = new System.Windows.Forms.Label();
            this.numThang = new System.Windows.Forms.NumericUpDown();
            this.lblThang = new System.Windows.Forms.Label();
            this.cboLoaiBaoCao = new System.Windows.Forms.ComboBox();
            this.lblLoaiBaoCao = new System.Windows.Forms.Label();
            this.dgvBaoCao = new System.Windows.Forms.DataGridView();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.btnDong = new System.Windows.Forms.Button();
            this.lblTongKet = new System.Windows.Forms.Label();
            this.lblSoDong = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numQuy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numThang)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBaoCao)).BeginInit();
            this.pnlFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            this.pnlHeader.Controls.Add(this.pnlHeaderAccent);
            this.pnlHeader.Controls.Add(this.lblSubTitle);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1260, 72);
            this.pnlHeader.TabIndex = 0;
            // 
            // pnlHeaderAccent
            // 
            this.pnlHeaderAccent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(161)))), ((int)(((byte)(55)))));
            this.pnlHeaderAccent.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlHeaderAccent.Location = new System.Drawing.Point(0, 68);
            this.pnlHeaderAccent.Name = "pnlHeaderAccent";
            this.pnlHeaderAccent.Size = new System.Drawing.Size(1260, 4);
            this.pnlHeaderAccent.TabIndex = 2;
            // 
            // lblSubTitle
            // 
            this.lblSubTitle.AutoSize = true;
            this.lblSubTitle.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblSubTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblSubTitle.Location = new System.Drawing.Point(24, 42);
            this.lblSubTitle.Name = "lblSubTitle";
            this.lblSubTitle.Size = new System.Drawing.Size(532, 17);
            this.lblSubTitle.TabIndex = 1;
            this.lblSubTitle.Text = "Thống kê doanh số bán hàng, hóa đơn nhập kho lớn nhất và xuất báo cáo biểu mẫu Excel";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(22, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(465, 28);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "KIOTVIET • BÁO CÁO DOANH THU & KHO HÀNG";
            // 
            // pnlControls
            // 
            this.pnlControls.BackColor = System.Drawing.Color.White;
            this.pnlControls.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlControls.Controls.Add(this.btnBieuMauKho);
            this.pnlControls.Controls.Add(this.btnXuatExcel);
            this.pnlControls.Controls.Add(this.btnXemBaoCao);
            this.pnlControls.Controls.Add(this.numNam);
            this.pnlControls.Controls.Add(this.lblNam);
            this.pnlControls.Controls.Add(this.numQuy);
            this.pnlControls.Controls.Add(this.lblQuy);
            this.pnlControls.Controls.Add(this.numThang);
            this.pnlControls.Controls.Add(this.lblThang);
            this.pnlControls.Controls.Add(this.cboLoaiBaoCao);
            this.pnlControls.Controls.Add(this.lblLoaiBaoCao);
            this.pnlControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlControls.Location = new System.Drawing.Point(0, 72);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Padding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.pnlControls.Size = new System.Drawing.Size(1260, 96);
            this.pnlControls.TabIndex = 1;
            // 
            // btnBieuMauKho
            // 
            this.btnBieuMauKho.BackColor = System.Drawing.Color.White;
            this.btnBieuMauKho.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBieuMauKho.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnBieuMauKho.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBieuMauKho.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnBieuMauKho.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            this.btnBieuMauKho.Location = new System.Drawing.Point(340, 50);
            this.btnBieuMauKho.Name = "btnBieuMauKho";
            this.btnBieuMauKho.Size = new System.Drawing.Size(170, 36);
            this.btnBieuMauKho.TabIndex = 10;
            this.btnBieuMauKho.Text = "📋 Biểu Mẫu BM03";
            this.btnBieuMauKho.UseVisualStyleBackColor = false;
            this.btnBieuMauKho.Click += new System.EventHandler(this.btnBieuMauKho_Click);
            // 
            // btnXuatExcel
            // 
            this.btnXuatExcel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            this.btnXuatExcel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnXuatExcel.FlatAppearance.BorderSize = 0;
            this.btnXuatExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXuatExcel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnXuatExcel.ForeColor = System.Drawing.Color.White;
            this.btnXuatExcel.Location = new System.Drawing.Point(180, 50);
            this.btnXuatExcel.Name = "btnXuatExcel";
            this.btnXuatExcel.Size = new System.Drawing.Size(150, 36);
            this.btnXuatExcel.TabIndex = 9;
            this.btnXuatExcel.Text = "📊 Xuất Excel";
            this.btnXuatExcel.UseVisualStyleBackColor = false;
            this.btnXuatExcel.Click += new System.EventHandler(this.btnXuatExcel_Click);
            // 
            // btnXemBaoCao
            // 
            this.btnXemBaoCao.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(161)))), ((int)(((byte)(55)))));
            this.btnXemBaoCao.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnXemBaoCao.FlatAppearance.BorderSize = 0;
            this.btnXemBaoCao.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXemBaoCao.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnXemBaoCao.ForeColor = System.Drawing.Color.White;
            this.btnXemBaoCao.Location = new System.Drawing.Point(16, 50);
            this.btnXemBaoCao.Name = "btnXemBaoCao";
            this.btnXemBaoCao.Size = new System.Drawing.Size(155, 36);
            this.btnXemBaoCao.TabIndex = 8;
            this.btnXemBaoCao.Text = "🔍 Xem Báo Cáo (F5)";
            this.btnXemBaoCao.UseVisualStyleBackColor = false;
            this.btnXemBaoCao.Click += new System.EventHandler(this.btnXemBaoCao_Click);
            // 
            // numNam
            // 
            this.numNam.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.numNam.Location = new System.Drawing.Point(860, 14);
            this.numNam.Maximum = new decimal(new int[] {
            2035,
            0,
            0,
            0});
            this.numNam.Minimum = new decimal(new int[] {
            2020,
            0,
            0,
            0});
            this.numNam.Name = "numNam";
            this.numNam.Size = new System.Drawing.Size(75, 24);
            this.numNam.TabIndex = 7;
            this.numNam.Value = new decimal(new int[] {
            2026,
            0,
            0,
            0});
            // 
            // lblNam
            // 
            this.lblNam.AutoSize = true;
            this.lblNam.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblNam.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblNam.Location = new System.Drawing.Point(815, 18);
            this.lblNam.Name = "lblNam";
            this.lblNam.Size = new System.Drawing.Size(36, 15);
            this.lblNam.TabIndex = 6;
            this.lblNam.Text = "Năm:";
            // 
            // numQuy
            // 
            this.numQuy.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.numQuy.Location = new System.Drawing.Point(740, 14);
            this.numQuy.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numQuy.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numQuy.Name = "numQuy";
            this.numQuy.Size = new System.Drawing.Size(55, 24);
            this.numQuy.TabIndex = 5;
            this.numQuy.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // lblQuy
            // 
            this.lblQuy.AutoSize = true;
            this.lblQuy.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblQuy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblQuy.Location = new System.Drawing.Point(698, 18);
            this.lblQuy.Name = "lblQuy";
            this.lblQuy.Size = new System.Drawing.Size(32, 15);
            this.lblQuy.TabIndex = 4;
            this.lblQuy.Text = "Quý:";
            // 
            // numThang
            // 
            this.numThang.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.numThang.Location = new System.Drawing.Point(625, 14);
            this.numThang.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numThang.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numThang.Name = "numThang";
            this.numThang.Size = new System.Drawing.Size(55, 24);
            this.numThang.TabIndex = 3;
            this.numThang.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            // 
            // lblThang
            // 
            this.lblThang.AutoSize = true;
            this.lblThang.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblThang.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblThang.Location = new System.Drawing.Point(570, 18);
            this.lblThang.Name = "lblThang";
            this.lblThang.Size = new System.Drawing.Size(44, 15);
            this.lblThang.TabIndex = 2;
            this.lblThang.Text = "Tháng:";
            // 
            // cboLoaiBaoCao
            // 
            this.cboLoaiBaoCao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLoaiBaoCao.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cboLoaiBaoCao.FormattingEnabled = true;
            this.cboLoaiBaoCao.Items.AddRange(new object[] {
            "Top 5 sản phẩm bán chạy nhất trong tháng",
            "Hóa đơn nhập hàng lớn nhất trong quý",
            "Tổng kết doanh số bán theo năm",
            "Top 5 nhân viên có doanh số thấp nhất tháng",
            "Báo cáo tổng hợp xuất - nhập - tồn kho (Biểu mẫu BM03)"});
            this.cboLoaiBaoCao.Location = new System.Drawing.Point(125, 14);
            this.cboLoaiBaoCao.Name = "cboLoaiBaoCao";
            this.cboLoaiBaoCao.Size = new System.Drawing.Size(425, 24);
            this.cboLoaiBaoCao.TabIndex = 1;
            this.cboLoaiBaoCao.SelectedIndexChanged += new System.EventHandler(this.cboLoaiBaoCao_SelectedIndexChanged);
            // 
            // lblLoaiBaoCao
            // 
            this.lblLoaiBaoCao.AutoSize = true;
            this.lblLoaiBaoCao.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblLoaiBaoCao.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblLoaiBaoCao.Location = new System.Drawing.Point(16, 18);
            this.lblLoaiBaoCao.Name = "lblLoaiBaoCao";
            this.lblLoaiBaoCao.Size = new System.Drawing.Size(77, 15);
            this.lblLoaiBaoCao.TabIndex = 0;
            this.lblLoaiBaoCao.Text = "Loại báo cáo:";
            // 
            // dgvBaoCao
            // 
            this.dgvBaoCao.AllowUserToAddRows = false;
            this.dgvBaoCao.AllowUserToDeleteRows = false;
            this.dgvBaoCao.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvBaoCao.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.dgvBaoCao.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvBaoCao.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvBaoCao.ColumnHeadersHeight = 36;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(248)))), ((int)(((byte)(238)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvBaoCao.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvBaoCao.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBaoCao.EnableHeadersVisualStyles = false;
            this.dgvBaoCao.Location = new System.Drawing.Point(0, 168);
            this.dgvBaoCao.MultiSelect = false;
            this.dgvBaoCao.Name = "dgvBaoCao";
            this.dgvBaoCao.ReadOnly = true;
            this.dgvBaoCao.RowHeadersVisible = false;
            this.dgvBaoCao.RowTemplate.Height = 36;
            this.dgvBaoCao.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBaoCao.Size = new System.Drawing.Size(1260, 526);
            this.dgvBaoCao.TabIndex = 2;
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.White;
            this.pnlFooter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFooter.Controls.Add(this.btnDong);
            this.pnlFooter.Controls.Add(this.lblTongKet);
            this.pnlFooter.Controls.Add(this.lblSoDong);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 694);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            this.pnlFooter.Size = new System.Drawing.Size(1260, 56);
            this.pnlFooter.TabIndex = 3;
            // 
            // btnDong
            // 
            this.btnDong.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDong.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnDong.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDong.FlatAppearance.BorderSize = 0;
            this.btnDong.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDong.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnDong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnDong.Location = new System.Drawing.Point(1145, 10);
            this.btnDong.Name = "btnDong";
            this.btnDong.Size = new System.Drawing.Size(100, 34);
            this.btnDong.TabIndex = 2;
            this.btnDong.Text = "Đóng (Esc)";
            this.btnDong.UseVisualStyleBackColor = false;
            this.btnDong.Click += new System.EventHandler(this.btnDong_Click);
            // 
            // lblTongKet
            // 
            this.lblTongKet.AutoSize = true;
            this.lblTongKet.Font = new System.Drawing.Font("Segoe UI", 11.5F, System.Drawing.FontStyle.Bold);
            this.lblTongKet.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(161)))), ((int)(((byte)(55)))));
            this.lblTongKet.Location = new System.Drawing.Point(220, 16);
            this.lblTongKet.Name = "lblTongKet";
            this.lblTongKet.Size = new System.Drawing.Size(124, 21);
            this.lblTongKet.TabIndex = 1;
            this.lblTongKet.Text = "Tổng giá trị: 0 ₫";
            // 
            // lblSoDong
            // 
            this.lblSoDong.AutoSize = true;
            this.lblSoDong.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblSoDong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblSoDong.Location = new System.Drawing.Point(16, 18);
            this.lblSoDong.Name = "lblSoDong";
            this.lblSoDong.Size = new System.Drawing.Size(117, 17);
            this.lblSoDong.TabIndex = 0;
            this.lblSoDong.Text = "Tổng số kết quả: 0";
            // 
            // frmBaoCaoThongKe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1260, 750);
            this.Controls.Add(this.dgvBaoCao);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlControls);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimumSize = new System.Drawing.Size(1024, 600);
            this.Name = "frmBaoCaoThongKe";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "KiotViet POS • Báo Cáo Thống Kê & Phân Tích Nghiệp Vụ";
            this.Load += new System.EventHandler(this.frmBaoCaoThongKe_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlControls.ResumeLayout(false);
            this.pnlControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numQuy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numThang)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBaoCao)).EndInit();
            this.pnlFooter.ResumeLayout(false);
            this.pnlFooter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Panel pnlHeaderAccent;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubTitle;
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
