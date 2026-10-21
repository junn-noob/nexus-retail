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
            this.lblSubTitle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabBanHang = new System.Windows.Forms.TabPage();
            this.grpChiTietBan = new System.Windows.Forms.GroupBox();
            this.dgvChiTietBan = new System.Windows.Forms.DataGridView();
            this.dgvHoaDonBan = new System.Windows.Forms.DataGridView();
            this.pnlSearchBan = new System.Windows.Forms.Panel();
            this.btnLamMoiBan = new System.Windows.Forms.Button();
            this.btnTimKiemBan = new System.Windows.Forms.Button();
            this.txtTimKiemBan = new System.Windows.Forms.TextBox();
            this.lblTimKiemBan = new System.Windows.Forms.Label();
            this.tabNhapHang = new System.Windows.Forms.TabPage();
            this.grpChiTietNhap = new System.Windows.Forms.GroupBox();
            this.dgvChiTietNhap = new System.Windows.Forms.DataGridView();
            this.dgvHoaDonNhap = new System.Windows.Forms.DataGridView();
            this.pnlSearchNhap = new System.Windows.Forms.Panel();
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
            this.grpChiTietBan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiTietBan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHoaDonBan)).BeginInit();
            this.pnlSearchBan.SuspendLayout();
            this.tabNhapHang.SuspendLayout();
            this.grpChiTietNhap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiTietNhap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHoaDonNhap)).BeginInit();
            this.pnlSearchNhap.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.pnlHeader.Controls.Add(this.lblSubTitle);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1008, 75);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblSubTitle
            // 
            this.lblSubTitle.AutoSize = true;
            this.lblSubTitle.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblSubTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblSubTitle.Location = new System.Drawing.Point(24, 43);
            this.lblSubTitle.Name = "lblSubTitle";
            this.lblSubTitle.Size = new System.Drawing.Size(560, 17);
            this.lblSubTitle.TabIndex = 1;
            this.lblSubTitle.Text = "Tra cứu và tìm kiếm thông tin hóa đơn bán lẻ, hóa đơn nhập kho theo chứng từ và đối tác";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(22, 11);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(430, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "TRA CỨU HÓA ĐƠN BÁN & NHẬP KHO";
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabBanHang);
            this.tabMain.Controls.Add(this.tabNhapHang);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.tabMain.Location = new System.Drawing.Point(0, 75);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(1008, 555);
            this.tabMain.TabIndex = 1;
            this.tabMain.SelectedIndexChanged += new System.EventHandler(this.tabMain_SelectedIndexChanged);
            // 
            // tabBanHang
            // 
            this.tabBanHang.Controls.Add(this.grpChiTietBan);
            this.tabBanHang.Controls.Add(this.dgvHoaDonBan);
            this.tabBanHang.Controls.Add(this.pnlSearchBan);
            this.tabBanHang.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tabBanHang.Location = new System.Drawing.Point(4, 26);
            this.tabBanHang.Name = "tabBanHang";
            this.tabBanHang.Padding = new System.Windows.Forms.Padding(8);
            this.tabBanHang.Size = new System.Drawing.Size(1000, 525);
            this.tabBanHang.TabIndex = 0;
            this.tabBanHang.Text = "🛒 Hóa Đơn Bán Hàng";
            this.tabBanHang.UseVisualStyleBackColor = true;
            // 
            // grpChiTietBan
            // 
            this.grpChiTietBan.Controls.Add(this.dgvChiTietBan);
            this.grpChiTietBan.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpChiTietBan.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpChiTietBan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.grpChiTietBan.Location = new System.Drawing.Point(8, 305);
            this.grpChiTietBan.Name = "grpChiTietBan";
            this.grpChiTietBan.Padding = new System.Windows.Forms.Padding(8);
            this.grpChiTietBan.Size = new System.Drawing.Size(984, 212);
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
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvChiTietBan.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvChiTietBan.ColumnHeadersHeight = 30;
            this.dgvChiTietBan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvChiTietBan.EnableHeadersVisualStyles = false;
            this.dgvChiTietBan.Location = new System.Drawing.Point(8, 25);
            this.dgvChiTietBan.MultiSelect = false;
            this.dgvChiTietBan.Name = "dgvChiTietBan";
            this.dgvChiTietBan.ReadOnly = true;
            this.dgvChiTietBan.RowHeadersVisible = false;
            this.dgvChiTietBan.RowTemplate.Height = 26;
            this.dgvChiTietBan.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvChiTietBan.Size = new System.Drawing.Size(968, 179);
            this.dgvChiTietBan.TabIndex = 0;
            // 
            // dgvHoaDonBan
            // 
            this.dgvHoaDonBan.AllowUserToAddRows = false;
            this.dgvHoaDonBan.AllowUserToDeleteRows = false;
            this.dgvHoaDonBan.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHoaDonBan.BackgroundColor = System.Drawing.Color.White;
            this.dgvHoaDonBan.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvHoaDonBan.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvHoaDonBan.ColumnHeadersHeight = 32;
            this.dgvHoaDonBan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHoaDonBan.EnableHeadersVisualStyles = false;
            this.dgvHoaDonBan.Location = new System.Drawing.Point(8, 64);
            this.dgvHoaDonBan.MultiSelect = false;
            this.dgvHoaDonBan.Name = "dgvHoaDonBan";
            this.dgvHoaDonBan.ReadOnly = true;
            this.dgvHoaDonBan.RowHeadersVisible = false;
            this.dgvHoaDonBan.RowTemplate.Height = 28;
            this.dgvHoaDonBan.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHoaDonBan.Size = new System.Drawing.Size(984, 453);
            this.dgvHoaDonBan.TabIndex = 1;
            this.dgvHoaDonBan.SelectionChanged += new System.EventHandler(this.dgvHoaDonBan_SelectionChanged);
            // 
            // pnlSearchBan
            // 
            this.pnlSearchBan.Controls.Add(this.btnLamMoiBan);
            this.pnlSearchBan.Controls.Add(this.btnTimKiemBan);
            this.pnlSearchBan.Controls.Add(this.txtTimKiemBan);
            this.pnlSearchBan.Controls.Add(this.lblTimKiemBan);
            this.pnlSearchBan.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSearchBan.Location = new System.Drawing.Point(8, 8);
            this.pnlSearchBan.Name = "pnlSearchBan";
            this.pnlSearchBan.Size = new System.Drawing.Size(984, 56);
            this.pnlSearchBan.TabIndex = 0;
            // 
            // btnLamMoiBan
            // 
            this.btnLamMoiBan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnLamMoiBan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLamMoiBan.FlatAppearance.BorderSize = 0;
            this.btnLamMoiBan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLamMoiBan.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnLamMoiBan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnLamMoiBan.Location = new System.Drawing.Point(620, 11);
            this.btnLamMoiBan.Name = "btnLamMoiBan";
            this.btnLamMoiBan.Size = new System.Drawing.Size(100, 32);
            this.btnLamMoiBan.TabIndex = 3;
            this.btnLamMoiBan.Text = "🔄 Làm mới";
            this.btnLamMoiBan.UseVisualStyleBackColor = false;
            this.btnLamMoiBan.Click += new System.EventHandler(this.btnLamMoiBan_Click);
            // 
            // btnTimKiemBan
            // 
            this.btnTimKiemBan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(70)))), ((int)(((byte)(229)))));
            this.btnTimKiemBan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTimKiemBan.FlatAppearance.BorderSize = 0;
            this.btnTimKiemBan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTimKiemBan.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnTimKiemBan.ForeColor = System.Drawing.Color.White;
            this.btnTimKiemBan.Location = new System.Drawing.Point(510, 11);
            this.btnTimKiemBan.Name = "btnTimKiemBan";
            this.btnTimKiemBan.Size = new System.Drawing.Size(100, 32);
            this.btnTimKiemBan.TabIndex = 2;
            this.btnTimKiemBan.Text = "🔍 Tìm kiếm";
            this.btnTimKiemBan.UseVisualStyleBackColor = false;
            this.btnTimKiemBan.Click += new System.EventHandler(this.btnTimKiemBan_Click);
            // 
            // txtTimKiemBan
            // 
            this.txtTimKiemBan.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtTimKiemBan.Location = new System.Drawing.Point(135, 15);
            this.txtTimKiemBan.Name = "txtTimKiemBan";
            this.txtTimKiemBan.PlaceholderText = "Nhập mã HĐB, tên khách, số điện thoại hoặc thu ngân...";
            this.txtTimKiemBan.Size = new System.Drawing.Size(365, 25);
            this.txtTimKiemBan.TabIndex = 1;
            this.txtTimKiemBan.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTimKiemBan_KeyDown);
            // 
            // lblTimKiemBan
            // 
            this.lblTimKiemBan.AutoSize = true;
            this.lblTimKiemBan.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblTimKiemBan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblTimKiemBan.Location = new System.Drawing.Point(12, 18);
            this.lblTimKiemBan.Name = "lblTimKiemBan";
            this.lblTimKiemBan.Size = new System.Drawing.Size(117, 17);
            this.lblTimKiemBan.TabIndex = 0;
            this.lblTimKiemBan.Text = "Từ khóa tìm kiếm:";
            // 
            // tabNhapHang
            // 
            this.tabNhapHang.Controls.Add(this.grpChiTietNhap);
            this.tabNhapHang.Controls.Add(this.dgvHoaDonNhap);
            this.tabNhapHang.Controls.Add(this.pnlSearchNhap);
            this.tabNhapHang.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tabNhapHang.Location = new System.Drawing.Point(4, 26);
            this.tabNhapHang.Name = "tabNhapHang";
            this.tabNhapHang.Padding = new System.Windows.Forms.Padding(8);
            this.tabNhapHang.Size = new System.Drawing.Size(1000, 525);
            this.tabNhapHang.TabIndex = 1;
            this.tabNhapHang.Text = "📦 Hóa Đơn Nhập Kho";
            this.tabNhapHang.UseVisualStyleBackColor = true;
            // 
            // grpChiTietNhap
            // 
            this.grpChiTietNhap.Controls.Add(this.dgvChiTietNhap);
            this.grpChiTietNhap.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpChiTietNhap.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpChiTietNhap.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.grpChiTietNhap.Location = new System.Drawing.Point(8, 305);
            this.grpChiTietNhap.Name = "grpChiTietNhap";
            this.grpChiTietNhap.Padding = new System.Windows.Forms.Padding(8);
            this.grpChiTietNhap.Size = new System.Drawing.Size(984, 212);
            this.grpChiTietNhap.TabIndex = 2;
            this.grpChiTietNhap.TabStop = false;
            this.grpChiTietNhap.Text = "Chi tiết thiết bị trong phiếu nhập";
            // 
            // dgvChiTietNhap
            // 
            this.dgvChiTietNhap.AllowUserToAddRows = false;
            this.dgvChiTietNhap.AllowUserToDeleteRows = false;
            this.dgvChiTietNhap.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvChiTietNhap.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.dgvChiTietNhap.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvChiTietNhap.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvChiTietNhap.ColumnHeadersHeight = 30;
            this.dgvChiTietNhap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvChiTietNhap.EnableHeadersVisualStyles = false;
            this.dgvChiTietNhap.Location = new System.Drawing.Point(8, 25);
            this.dgvChiTietNhap.MultiSelect = false;
            this.dgvChiTietNhap.Name = "dgvChiTietNhap";
            this.dgvChiTietNhap.ReadOnly = true;
            this.dgvChiTietNhap.RowHeadersVisible = false;
            this.dgvChiTietNhap.RowTemplate.Height = 26;
            this.dgvChiTietNhap.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvChiTietNhap.Size = new System.Drawing.Size(968, 179);
            this.dgvChiTietNhap.TabIndex = 0;
            // 
            // dgvHoaDonNhap
            // 
            this.dgvHoaDonNhap.AllowUserToAddRows = false;
            this.dgvHoaDonNhap.AllowUserToDeleteRows = false;
            this.dgvHoaDonNhap.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHoaDonNhap.BackgroundColor = System.Drawing.Color.White;
            this.dgvHoaDonNhap.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvHoaDonNhap.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvHoaDonNhap.ColumnHeadersHeight = 32;
            this.dgvHoaDonNhap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHoaDonNhap.EnableHeadersVisualStyles = false;
            this.dgvHoaDonNhap.Location = new System.Drawing.Point(8, 64);
            this.dgvHoaDonNhap.MultiSelect = false;
            this.dgvHoaDonNhap.Name = "dgvHoaDonNhap";
            this.dgvHoaDonNhap.ReadOnly = true;
            this.dgvHoaDonNhap.RowHeadersVisible = false;
            this.dgvHoaDonNhap.RowTemplate.Height = 28;
            this.dgvHoaDonNhap.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHoaDonNhap.Size = new System.Drawing.Size(984, 453);
            this.dgvHoaDonNhap.TabIndex = 1;
            this.dgvHoaDonNhap.SelectionChanged += new System.EventHandler(this.dgvHoaDonNhap_SelectionChanged);
            // 
            // pnlSearchNhap
            // 
            this.pnlSearchNhap.Controls.Add(this.btnLamMoiNhap);
            this.pnlSearchNhap.Controls.Add(this.btnTimKiemNhap);
            this.pnlSearchNhap.Controls.Add(this.txtTimKiemNhap);
            this.pnlSearchNhap.Controls.Add(this.lblTimKiemNhap);
            this.pnlSearchNhap.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSearchNhap.Location = new System.Drawing.Point(8, 8);
            this.pnlSearchNhap.Name = "pnlSearchNhap";
            this.pnlSearchNhap.Size = new System.Drawing.Size(984, 56);
            this.pnlSearchNhap.TabIndex = 0;
            // 
            // btnLamMoiNhap
            // 
            this.btnLamMoiNhap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnLamMoiNhap.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLamMoiNhap.FlatAppearance.BorderSize = 0;
            this.btnLamMoiNhap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLamMoiNhap.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnLamMoiNhap.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnLamMoiNhap.Location = new System.Drawing.Point(620, 11);
            this.btnLamMoiNhap.Name = "btnLamMoiNhap";
            this.btnLamMoiNhap.Size = new System.Drawing.Size(100, 32);
            this.btnLamMoiNhap.TabIndex = 3;
            this.btnLamMoiNhap.Text = "🔄 Làm mới";
            this.btnLamMoiNhap.UseVisualStyleBackColor = false;
            this.btnLamMoiNhap.Click += new System.EventHandler(this.btnLamMoiNhap_Click);
            // 
            // btnTimKiemNhap
            // 
            this.btnTimKiemNhap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(70)))), ((int)(((byte)(229)))));
            this.btnTimKiemNhap.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTimKiemNhap.FlatAppearance.BorderSize = 0;
            this.btnTimKiemNhap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTimKiemNhap.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnTimKiemNhap.ForeColor = System.Drawing.Color.White;
            this.btnTimKiemNhap.Location = new System.Drawing.Point(510, 11);
            this.btnTimKiemNhap.Name = "btnTimKiemNhap";
            this.btnTimKiemNhap.Size = new System.Drawing.Size(100, 32);
            this.btnTimKiemNhap.TabIndex = 2;
            this.btnTimKiemNhap.Text = "🔍 Tìm kiếm";
            this.btnTimKiemNhap.UseVisualStyleBackColor = false;
            this.btnTimKiemNhap.Click += new System.EventHandler(this.btnTimKiemNhap_Click);
            // 
            // txtTimKiemNhap
            // 
            this.txtTimKiemNhap.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtTimKiemNhap.Location = new System.Drawing.Point(135, 15);
            this.txtTimKiemNhap.Name = "txtTimKiemNhap";
            this.txtTimKiemNhap.PlaceholderText = "Nhập mã HĐN, nhà cung cấp hoặc nhân viên nhập...";
            this.txtTimKiemNhap.Size = new System.Drawing.Size(365, 25);
            this.txtTimKiemNhap.TabIndex = 1;
            this.txtTimKiemNhap.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTimKiemNhap_KeyDown);
            // 
            // lblTimKiemNhap
            // 
            this.lblTimKiemNhap.AutoSize = true;
            this.lblTimKiemNhap.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblTimKiemNhap.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblTimKiemNhap.Location = new System.Drawing.Point(12, 18);
            this.lblTimKiemNhap.Name = "lblTimKiemNhap";
            this.lblTimKiemNhap.Size = new System.Drawing.Size(117, 17);
            this.lblTimKiemNhap.TabIndex = 0;
            this.lblTimKiemNhap.Text = "Từ khóa tìm kiếm:";
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.White;
            this.pnlFooter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFooter.Controls.Add(this.btnDong);
            this.pnlFooter.Controls.Add(this.lblThongTin);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 630);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            this.pnlFooter.Size = new System.Drawing.Size(1008, 50);
            this.pnlFooter.TabIndex = 2;
            // 
            // btnDong
            // 
            this.btnDong.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDong.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnDong.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDong.FlatAppearance.BorderSize = 0;
            this.btnDong.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDong.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnDong.Location = new System.Drawing.Point(905, 8);
            this.btnDong.Name = "btnDong";
            this.btnDong.Size = new System.Drawing.Size(90, 32);
            this.btnDong.TabIndex = 1;
            this.btnDong.Text = "Đóng";
            this.btnDong.UseVisualStyleBackColor = false;
            this.btnDong.Click += new System.EventHandler(this.btnDong_Click);
            // 
            // lblThongTin
            // 
            this.lblThongTin.AutoSize = true;
            this.lblThongTin.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblThongTin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblThongTin.Location = new System.Drawing.Point(16, 15);
            this.lblThongTin.Name = "lblThongTin";
            this.lblThongTin.Size = new System.Drawing.Size(262, 17);
            this.lblThongTin.TabIndex = 0;
            this.lblThongTin.Text = "Nhấp vào một dòng để xem chi tiết hóa đơn";
            // 
            // frmTraCuuHoaDon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 680);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(950, 600);
            this.Name = "frmTraCuuHoaDon";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nexus Retail - Tra Cứu Hóa Đơn Bán & Nhập Kho";
            this.Load += new System.EventHandler(this.frmTraCuuHoaDon_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.tabMain.ResumeLayout(false);
            this.tabBanHang.ResumeLayout(false);
            this.grpChiTietBan.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiTietBan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHoaDonBan)).EndInit();
            this.pnlSearchBan.ResumeLayout(false);
            this.pnlSearchBan.PerformLayout();
            this.tabNhapHang.ResumeLayout(false);
            this.grpChiTietNhap.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiTietNhap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHoaDonNhap)).EndInit();
            this.pnlSearchNhap.ResumeLayout(false);
            this.pnlSearchNhap.PerformLayout();
            this.pnlFooter.ResumeLayout(false);
            this.pnlFooter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblSubTitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabBanHang;
        private System.Windows.Forms.Panel pnlSearchBan;
        private System.Windows.Forms.TextBox txtTimKiemBan;
        private System.Windows.Forms.Label lblTimKiemBan;
        private System.Windows.Forms.Button btnTimKiemBan;
        private System.Windows.Forms.Button btnLamMoiBan;
        private System.Windows.Forms.DataGridView dgvHoaDonBan;
        private System.Windows.Forms.GroupBox grpChiTietBan;
        private System.Windows.Forms.DataGridView dgvChiTietBan;
        private System.Windows.Forms.TabPage tabNhapHang;
        private System.Windows.Forms.Panel pnlSearchNhap;
        private System.Windows.Forms.TextBox txtTimKiemNhap;
        private System.Windows.Forms.Label lblTimKiemNhap;
        private System.Windows.Forms.Button btnTimKiemNhap;
        private System.Windows.Forms.Button btnLamMoiNhap;
        private System.Windows.Forms.DataGridView dgvHoaDonNhap;
        private System.Windows.Forms.GroupBox grpChiTietNhap;
        private System.Windows.Forms.DataGridView dgvChiTietNhap;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Label lblThongTin;
        private System.Windows.Forms.Button btnDong;
    }
}
