namespace NexusRetail.WinForms.Forms
{
    partial class frmTraCuuHoaDon
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.pnlHeaderAccent = new System.Windows.Forms.Panel();
            this.lblSubTitle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabBanHang = new System.Windows.Forms.TabPage();
            this.dgvHoaDonBan = new System.Windows.Forms.DataGridView();
            this.grpChiTietBan = new System.Windows.Forms.GroupBox();
            this.dgvChiTietBan = new System.Windows.Forms.DataGridView();
            this.pnlSearchBan = new System.Windows.Forms.Panel();
            this.btnInBieuMauBan = new System.Windows.Forms.Button();
            this.btnLamMoiBan = new System.Windows.Forms.Button();
            this.btnTimKiemBan = new System.Windows.Forms.Button();
            this.txtTimKiemBan = new System.Windows.Forms.TextBox();
            this.lblTimKiemBan = new System.Windows.Forms.Label();
            this.tabNhapHang = new System.Windows.Forms.TabPage();
            this.dgvHoaDonNhap = new System.Windows.Forms.DataGridView();
            this.grpChiTietNhap = new System.Windows.Forms.GroupBox();
            this.dgvChiTietNhap = new System.Windows.Forms.DataGridView();
            this.pnlSearchNhap = new System.Windows.Forms.Panel();
            this.btnInBieuMauNhap = new System.Windows.Forms.Button();
            this.btnLamMoiNhap = new System.Windows.Forms.Button();
            this.btnTimKiemNhap = new System.Windows.Forms.Button();
            this.txtTimKiemNhap = new System.Windows.Forms.TextBox();
            this.lblTimKiemNhap = new System.Windows.Forms.Label();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.btnDong = new System.Windows.Forms.Button();
            this.lblThongTin = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabBanHang.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHoaDonBan)).BeginInit();
            this.grpChiTietBan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiTietBan)).BeginInit();
            this.pnlSearchBan.SuspendLayout();
            this.tabNhapHang.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHoaDonNhap)).BeginInit();
            this.grpChiTietNhap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiTietNhap)).BeginInit();
            this.pnlSearchNhap.SuspendLayout();
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
            this.pnlHeader.Size = new System.Drawing.Size(1240, 72);
            this.pnlHeader.TabIndex = 0;
            // 
            // pnlHeaderAccent
            // 
            this.pnlHeaderAccent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(161)))), ((int)(((byte)(55)))));
            this.pnlHeaderAccent.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlHeaderAccent.Location = new System.Drawing.Point(0, 68);
            this.pnlHeaderAccent.Name = "pnlHeaderAccent";
            this.pnlHeaderAccent.Size = new System.Drawing.Size(1240, 4);
            this.pnlHeaderAccent.TabIndex = 2;
            // 
            // lblSubTitle
            // 
            this.lblSubTitle.AutoSize = true;
            this.lblSubTitle.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblSubTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblSubTitle.Location = new System.Drawing.Point(24, 42);
            this.lblSubTitle.Name = "lblSubTitle";
            this.lblSubTitle.Size = new System.Drawing.Size(564, 17);
            this.lblSubTitle.TabIndex = 1;
            this.lblSubTitle.Text = "Sổ giao dịch bán lẻ và nhập kho • Tìm kiếm chứng từ theo mã số, thời gian, khách " +
    "hàng và đối tác";
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
            this.lblTitle.Text = "KIOTVIET • SỔ GIAO DỊCH VÀ TRA CỨU HÓA ĐƠN";
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabBanHang);
            this.tabMain.Controls.Add(this.tabNhapHang);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.tabMain.Location = new System.Drawing.Point(0, 72);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(1240, 630);
            this.tabMain.TabIndex = 1;
            this.tabMain.SelectedIndexChanged += new System.EventHandler(this.tabMain_SelectedIndexChanged);
            // 
            // tabBanHang
            // 
            this.tabBanHang.Controls.Add(this.dgvHoaDonBan);
            this.tabBanHang.Controls.Add(this.grpChiTietBan);
            this.tabBanHang.Controls.Add(this.pnlSearchBan);
            this.tabBanHang.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tabBanHang.Location = new System.Drawing.Point(4, 26);
            this.tabBanHang.Name = "tabBanHang";
            this.tabBanHang.Padding = new System.Windows.Forms.Padding(8);
            this.tabBanHang.Size = new System.Drawing.Size(1232, 600);
            this.tabBanHang.TabIndex = 0;
            this.tabBanHang.Text = "🛒 Hóa Đơn Bán Lẻ";
            this.tabBanHang.UseVisualStyleBackColor = true;
            // 
            // dgvHoaDonBan
            // 
            this.dgvHoaDonBan.AllowUserToAddRows = false;
            this.dgvHoaDonBan.AllowUserToDeleteRows = false;
            this.dgvHoaDonBan.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHoaDonBan.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.dgvHoaDonBan.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvHoaDonBan.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvHoaDonBan.ColumnHeadersHeight = 36;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(248)))), ((int)(((byte)(238)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvHoaDonBan.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvHoaDonBan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHoaDonBan.EnableHeadersVisualStyles = false;
            this.dgvHoaDonBan.Location = new System.Drawing.Point(8, 64);
            this.dgvHoaDonBan.MultiSelect = false;
            this.dgvHoaDonBan.Name = "dgvHoaDonBan";
            this.dgvHoaDonBan.ReadOnly = true;
            this.dgvHoaDonBan.RowHeadersVisible = false;
            this.dgvHoaDonBan.RowTemplate.Height = 36;
            this.dgvHoaDonBan.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHoaDonBan.Size = new System.Drawing.Size(1216, 308);
            this.dgvHoaDonBan.TabIndex = 1;
            this.dgvHoaDonBan.SelectionChanged += new System.EventHandler(this.dgvHoaDonBan_SelectionChanged);
            // 
            // grpChiTietBan
            // 
            this.grpChiTietBan.Controls.Add(this.dgvChiTietBan);
            this.grpChiTietBan.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpChiTietBan.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpChiTietBan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            this.grpChiTietBan.Location = new System.Drawing.Point(8, 372);
            this.grpChiTietBan.Name = "grpChiTietBan";
            this.grpChiTietBan.Padding = new System.Windows.Forms.Padding(8);
            this.grpChiTietBan.Size = new System.Drawing.Size(1216, 220);
            this.grpChiTietBan.TabIndex = 2;
            this.grpChiTietBan.TabStop = false;
            this.grpChiTietBan.Text = "Chi tiết các mặt hàng trong đơn bán";
            // 
            // dgvChiTietBan
            // 
            this.dgvChiTietBan.AllowUserToAddRows = false;
            this.dgvChiTietBan.AllowUserToDeleteRows = false;
            this.dgvChiTietBan.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvChiTietBan.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.dgvChiTietBan.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvChiTietBan.ColumnHeadersHeight = 32;
            this.dgvChiTietBan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvChiTietBan.Location = new System.Drawing.Point(8, 25);
            this.dgvChiTietBan.MultiSelect = false;
            this.dgvChiTietBan.Name = "dgvChiTietBan";
            this.dgvChiTietBan.ReadOnly = true;
            this.dgvChiTietBan.RowHeadersVisible = false;
            this.dgvChiTietBan.RowTemplate.Height = 32;
            this.dgvChiTietBan.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvChiTietBan.Size = new System.Drawing.Size(1200, 187);
            this.dgvChiTietBan.TabIndex = 0;
            // 
            // pnlSearchBan
            // 
            this.pnlSearchBan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.pnlSearchBan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSearchBan.Controls.Add(this.btnInBieuMauBan);
            this.pnlSearchBan.Controls.Add(this.btnLamMoiBan);
            this.pnlSearchBan.Controls.Add(this.btnTimKiemBan);
            this.pnlSearchBan.Controls.Add(this.txtTimKiemBan);
            this.pnlSearchBan.Controls.Add(this.lblTimKiemBan);
            this.pnlSearchBan.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSearchBan.Location = new System.Drawing.Point(8, 8);
            this.pnlSearchBan.Name = "pnlSearchBan";
            this.pnlSearchBan.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.pnlSearchBan.Size = new System.Drawing.Size(1216, 56);
            this.pnlSearchBan.TabIndex = 0;
            // 
            // btnInBieuMauBan
            // 
            this.btnInBieuMauBan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInBieuMauBan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            this.btnInBieuMauBan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnInBieuMauBan.FlatAppearance.BorderSize = 0;
            this.btnInBieuMauBan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInBieuMauBan.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnInBieuMauBan.ForeColor = System.Drawing.Color.White;
            this.btnInBieuMauBan.Location = new System.Drawing.Point(1015, 10);
            this.btnInBieuMauBan.Name = "btnInBieuMauBan";
            this.btnInBieuMauBan.Size = new System.Drawing.Size(185, 34);
            this.btnInBieuMauBan.TabIndex = 4;
            this.btnInBieuMauBan.Text = "📄 Xem Biểu Mẫu BM02";
            this.btnInBieuMauBan.UseVisualStyleBackColor = false;
            this.btnInBieuMauBan.Click += new System.EventHandler(this.btnInBieuMauBan_Click);
            // 
            // btnLamMoiBan
            // 
            this.btnLamMoiBan.BackColor = System.Drawing.Color.White;
            this.btnLamMoiBan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLamMoiBan.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnLamMoiBan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLamMoiBan.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnLamMoiBan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnLamMoiBan.Location = new System.Drawing.Point(540, 10);
            this.btnLamMoiBan.Name = "btnLamMoiBan";
            this.btnLamMoiBan.Size = new System.Drawing.Size(110, 34);
            this.btnLamMoiBan.TabIndex = 3;
            this.btnLamMoiBan.Text = "🔄 Làm Mới (F5)";
            this.btnLamMoiBan.UseVisualStyleBackColor = false;
            this.btnLamMoiBan.Click += new System.EventHandler(this.btnLamMoiBan_Click);
            // 
            // btnTimKiemBan
            // 
            this.btnTimKiemBan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(161)))), ((int)(((byte)(55)))));
            this.btnTimKiemBan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTimKiemBan.FlatAppearance.BorderSize = 0;
            this.btnTimKiemBan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTimKiemBan.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnTimKiemBan.ForeColor = System.Drawing.Color.White;
            this.btnTimKiemBan.Location = new System.Drawing.Point(425, 10);
            this.btnTimKiemBan.Name = "btnTimKiemBan";
            this.btnTimKiemBan.Size = new System.Drawing.Size(105, 34);
            this.btnTimKiemBan.TabIndex = 2;
            this.btnTimKiemBan.Text = "🔍 Tìm Kiếm";
            this.btnTimKiemBan.UseVisualStyleBackColor = false;
            this.btnTimKiemBan.Click += new System.EventHandler(this.btnTimKiemBan_Click);
            // 
            // txtTimKiemBan
            // 
            this.txtTimKiemBan.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtTimKiemBan.Location = new System.Drawing.Point(90, 14);
            this.txtTimKiemBan.Name = "txtTimKiemBan";
            this.txtTimKiemBan.PlaceholderText = "Nhập mã HĐ bán, tên khách hàng hoặc SĐT...";
            this.txtTimKiemBan.Size = new System.Drawing.Size(325, 25);
            this.txtTimKiemBan.TabIndex = 1;
            // 
            // lblTimKiemBan
            // 
            this.lblTimKiemBan.AutoSize = true;
            this.lblTimKiemBan.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblTimKiemBan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblTimKiemBan.Location = new System.Drawing.Point(14, 18);
            this.lblTimKiemBan.Name = "lblTimKiemBan";
            this.lblTimKiemBan.Size = new System.Drawing.Size(68, 17);
            this.lblTimKiemBan.TabIndex = 0;
            this.lblTimKiemBan.Text = "Tìm kiếm:";
            // 
            // tabNhapHang
            // 
            this.tabNhapHang.Controls.Add(this.dgvHoaDonNhap);
            this.tabNhapHang.Controls.Add(this.grpChiTietNhap);
            this.tabNhapHang.Controls.Add(this.pnlSearchNhap);
            this.tabNhapHang.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tabNhapHang.Location = new System.Drawing.Point(4, 26);
            this.tabNhapHang.Name = "tabNhapHang";
            this.tabNhapHang.Padding = new System.Windows.Forms.Padding(8);
            this.tabNhapHang.Size = new System.Drawing.Size(1232, 600);
            this.tabNhapHang.TabIndex = 1;
            this.tabNhapHang.Text = "📦 Hóa Đơn Nhập Kho";
            this.tabNhapHang.UseVisualStyleBackColor = true;
            // 
            // dgvHoaDonNhap
            // 
            this.dgvHoaDonNhap.AllowUserToAddRows = false;
            this.dgvHoaDonNhap.AllowUserToDeleteRows = false;
            this.dgvHoaDonNhap.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHoaDonNhap.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.dgvHoaDonNhap.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvHoaDonNhap.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvHoaDonNhap.ColumnHeadersHeight = 36;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(248)))), ((int)(((byte)(238)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvHoaDonNhap.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvHoaDonNhap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHoaDonNhap.EnableHeadersVisualStyles = false;
            this.dgvHoaDonNhap.Location = new System.Drawing.Point(8, 64);
            this.dgvHoaDonNhap.MultiSelect = false;
            this.dgvHoaDonNhap.Name = "dgvHoaDonNhap";
            this.dgvHoaDonNhap.ReadOnly = true;
            this.dgvHoaDonNhap.RowHeadersVisible = false;
            this.dgvHoaDonNhap.RowTemplate.Height = 36;
            this.dgvHoaDonNhap.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHoaDonNhap.Size = new System.Drawing.Size(1216, 308);
            this.dgvHoaDonNhap.TabIndex = 1;
            this.dgvHoaDonNhap.SelectionChanged += new System.EventHandler(this.dgvHoaDonNhap_SelectionChanged);
            // 
            // grpChiTietNhap
            // 
            this.grpChiTietNhap.Controls.Add(this.dgvChiTietNhap);
            this.grpChiTietNhap.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpChiTietNhap.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpChiTietNhap.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            this.grpChiTietNhap.Location = new System.Drawing.Point(8, 372);
            this.grpChiTietNhap.Name = "grpChiTietNhap";
            this.grpChiTietNhap.Padding = new System.Windows.Forms.Padding(8);
            this.grpChiTietNhap.Size = new System.Drawing.Size(1216, 220);
            this.grpChiTietNhap.TabIndex = 2;
            this.grpChiTietNhap.TabStop = false;
            this.grpChiTietNhap.Text = "Chi tiết các mặt hàng trong đơn nhập kho";
            // 
            // dgvChiTietNhap
            // 
            this.dgvChiTietNhap.AllowUserToAddRows = false;
            this.dgvChiTietNhap.AllowUserToDeleteRows = false;
            this.dgvChiTietNhap.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvChiTietNhap.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.dgvChiTietNhap.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvChiTietNhap.ColumnHeadersHeight = 32;
            this.dgvChiTietNhap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvChiTietNhap.Location = new System.Drawing.Point(8, 25);
            this.dgvChiTietNhap.MultiSelect = false;
            this.dgvChiTietNhap.Name = "dgvChiTietNhap";
            this.dgvChiTietNhap.ReadOnly = true;
            this.dgvChiTietNhap.RowHeadersVisible = false;
            this.dgvChiTietNhap.RowTemplate.Height = 32;
            this.dgvChiTietNhap.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvChiTietNhap.Size = new System.Drawing.Size(1200, 187);
            this.dgvChiTietNhap.TabIndex = 0;
            // 
            // pnlSearchNhap
            // 
            this.pnlSearchNhap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.pnlSearchNhap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSearchNhap.Controls.Add(this.btnInBieuMauNhap);
            this.pnlSearchNhap.Controls.Add(this.btnLamMoiNhap);
            this.pnlSearchNhap.Controls.Add(this.btnTimKiemNhap);
            this.pnlSearchNhap.Controls.Add(this.txtTimKiemNhap);
            this.pnlSearchNhap.Controls.Add(this.lblTimKiemNhap);
            this.pnlSearchNhap.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSearchNhap.Location = new System.Drawing.Point(8, 8);
            this.pnlSearchNhap.Name = "pnlSearchNhap";
            this.pnlSearchNhap.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.pnlSearchNhap.Size = new System.Drawing.Size(1216, 56);
            this.pnlSearchNhap.TabIndex = 0;
            // 
            // btnInBieuMauNhap
            // 
            this.btnInBieuMauNhap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInBieuMauNhap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            this.btnInBieuMauNhap.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnInBieuMauNhap.FlatAppearance.BorderSize = 0;
            this.btnInBieuMauNhap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInBieuMauNhap.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnInBieuMauNhap.ForeColor = System.Drawing.Color.White;
            this.btnInBieuMauNhap.Location = new System.Drawing.Point(1015, 10);
            this.btnInBieuMauNhap.Name = "btnInBieuMauNhap";
            this.btnInBieuMauNhap.Size = new System.Drawing.Size(185, 34);
            this.btnInBieuMauNhap.TabIndex = 4;
            this.btnInBieuMauNhap.Text = "📄 Xem Biểu Mẫu BM01";
            this.btnInBieuMauNhap.UseVisualStyleBackColor = false;
            this.btnInBieuMauNhap.Click += new System.EventHandler(this.btnInBieuMauNhap_Click);
            // 
            // btnLamMoiNhap
            // 
            this.btnLamMoiNhap.BackColor = System.Drawing.Color.White;
            this.btnLamMoiNhap.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLamMoiNhap.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnLamMoiNhap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLamMoiNhap.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnLamMoiNhap.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnLamMoiNhap.Location = new System.Drawing.Point(540, 10);
            this.btnLamMoiNhap.Name = "btnLamMoiNhap";
            this.btnLamMoiNhap.Size = new System.Drawing.Size(110, 34);
            this.btnLamMoiNhap.TabIndex = 3;
            this.btnLamMoiNhap.Text = "🔄 Làm Mới (F5)";
            this.btnLamMoiNhap.UseVisualStyleBackColor = false;
            this.btnLamMoiNhap.Click += new System.EventHandler(this.btnLamMoiNhap_Click);
            // 
            // btnTimKiemNhap
            // 
            this.btnTimKiemNhap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(161)))), ((int)(((byte)(55)))));
            this.btnTimKiemNhap.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTimKiemNhap.FlatAppearance.BorderSize = 0;
            this.btnTimKiemNhap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTimKiemNhap.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnTimKiemNhap.ForeColor = System.Drawing.Color.White;
            this.btnTimKiemNhap.Location = new System.Drawing.Point(425, 10);
            this.btnTimKiemNhap.Name = "btnTimKiemNhap";
            this.btnTimKiemNhap.Size = new System.Drawing.Size(105, 34);
            this.btnTimKiemNhap.TabIndex = 2;
            this.btnTimKiemNhap.Text = "🔍 Tìm Kiếm";
            this.btnTimKiemNhap.UseVisualStyleBackColor = false;
            this.btnTimKiemNhap.Click += new System.EventHandler(this.btnTimKiemNhap_Click);
            // 
            // txtTimKiemNhap
            // 
            this.txtTimKiemNhap.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtTimKiemNhap.Location = new System.Drawing.Point(90, 14);
            this.txtTimKiemNhap.Name = "txtTimKiemNhap";
            this.txtTimKiemNhap.PlaceholderText = "Nhập mã HĐ nhập, tên nhà cung cấp...";
            this.txtTimKiemNhap.Size = new System.Drawing.Size(325, 25);
            this.txtTimKiemNhap.TabIndex = 1;
            // 
            // lblTimKiemNhap
            // 
            this.lblTimKiemNhap.AutoSize = true;
            this.lblTimKiemNhap.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblTimKiemNhap.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblTimKiemNhap.Location = new System.Drawing.Point(14, 18);
            this.lblTimKiemNhap.Name = "lblTimKiemNhap";
            this.lblTimKiemNhap.Size = new System.Drawing.Size(68, 17);
            this.lblTimKiemNhap.TabIndex = 0;
            this.lblTimKiemNhap.Text = "Tìm kiếm:";
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.pnlFooter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFooter.Controls.Add(this.btnDong);
            this.pnlFooter.Controls.Add(this.lblThongTin);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 702);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            this.pnlFooter.Size = new System.Drawing.Size(1240, 48);
            this.pnlFooter.TabIndex = 2;
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
            this.btnDong.Location = new System.Drawing.Point(1125, 6);
            this.btnDong.Name = "btnDong";
            this.btnDong.Size = new System.Drawing.Size(100, 34);
            this.btnDong.TabIndex = 1;
            this.btnDong.Text = "Đóng (Esc)";
            this.btnDong.UseVisualStyleBackColor = false;
            this.btnDong.Click += new System.EventHandler(this.btnDong_Click);
            // 
            // lblThongTin
            // 
            this.lblThongTin.AutoSize = true;
            this.lblThongTin.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblThongTin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblThongTin.Location = new System.Drawing.Point(16, 14);
            this.lblThongTin.Name = "lblThongTin";
            this.lblThongTin.Size = new System.Drawing.Size(460, 17);
            this.lblThongTin.TabIndex = 0;
            this.lblThongTin.Text = "Sổ giao dịch điện tử KiotViet POS • Dữ liệu đồng bộ trực tiếp từ máy chủ";
            // 
            // frmTraCuuHoaDon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1240, 750);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimumSize = new System.Drawing.Size(1024, 600);
            this.Name = "frmTraCuuHoaDon";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "KiotViet POS • Sổ Giao Dịch & Tra Cứu Hóa Đơn";
            this.Load += new System.EventHandler(this.frmTraCuuHoaDon_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.tabMain.ResumeLayout(false);
            this.tabBanHang.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHoaDonBan)).EndInit();
            this.grpChiTietBan.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiTietBan)).EndInit();
            this.pnlSearchBan.ResumeLayout(false);
            this.pnlSearchBan.PerformLayout();
            this.tabNhapHang.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHoaDonNhap)).EndInit();
            this.grpChiTietNhap.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiTietNhap)).EndInit();
            this.pnlSearchNhap.ResumeLayout(false);
            this.pnlSearchNhap.PerformLayout();
            this.pnlFooter.ResumeLayout(false);
            this.pnlFooter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Panel pnlHeaderAccent;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubTitle;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabBanHang;
        private System.Windows.Forms.TabPage tabNhapHang;
        private System.Windows.Forms.Panel pnlSearchBan;
        private System.Windows.Forms.Label lblTimKiemBan;
        private System.Windows.Forms.TextBox txtTimKiemBan;
        private System.Windows.Forms.Button btnTimKiemBan;
        private System.Windows.Forms.Button btnLamMoiBan;
        private System.Windows.Forms.Button btnInBieuMauBan;
        private System.Windows.Forms.DataGridView dgvHoaDonBan;
        private System.Windows.Forms.GroupBox grpChiTietBan;
        private System.Windows.Forms.DataGridView dgvChiTietBan;
        private System.Windows.Forms.Panel pnlSearchNhap;
        private System.Windows.Forms.Label lblTimKiemNhap;
        private System.Windows.Forms.TextBox txtTimKiemNhap;
        private System.Windows.Forms.Button btnTimKiemNhap;
        private System.Windows.Forms.Button btnLamMoiNhap;
        private System.Windows.Forms.Button btnInBieuMauNhap;
        private System.Windows.Forms.DataGridView dgvHoaDonNhap;
        private System.Windows.Forms.GroupBox grpChiTietNhap;
        private System.Windows.Forms.DataGridView dgvChiTietNhap;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Label lblThongTin;
        private System.Windows.Forms.Button btnDong;
    }
}
