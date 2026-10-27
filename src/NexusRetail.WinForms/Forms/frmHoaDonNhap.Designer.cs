namespace NexusRetail.WinForms.Forms
{
    partial class frmHoaDonNhap
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
            this.pnlTopInfo = new System.Windows.Forms.Panel();
            this.tblTopSplit = new System.Windows.Forms.TableLayoutPanel();
            this.grpThongTinChung = new System.Windows.Forms.GroupBox();
            this.cboNhanVien = new System.Windows.Forms.ComboBox();
            this.lblNhanVien = new System.Windows.Forms.Label();
            this.cboNhaCungCap = new System.Windows.Forms.ComboBox();
            this.lblNhaCungCap = new System.Windows.Forms.Label();
            this.dtpNgayNhap = new System.Windows.Forms.DateTimePicker();
            this.lblNgayNhap = new System.Windows.Forms.Label();
            this.txtMaHdn = new System.Windows.Forms.TextBox();
            this.lblMaHdn = new System.Windows.Forms.Label();
            this.grpThemSanPham = new System.Windows.Forms.GroupBox();
            this.btnThemSp = new System.Windows.Forms.Button();
            this.numKhuyenMai = new System.Windows.Forms.NumericUpDown();
            this.lblKhuyenMai = new System.Windows.Forms.Label();
            this.txtDonGia = new System.Windows.Forms.TextBox();
            this.lblDonGia = new System.Windows.Forms.Label();
            this.numSoLuong = new System.Windows.Forms.NumericUpDown();
            this.lblSoLuong = new System.Windows.Forms.Label();
            this.cboSanPham = new System.Windows.Forms.ComboBox();
            this.lblSanPham = new System.Windows.Forms.Label();
            this.dgvChiTiet = new System.Windows.Forms.DataGridView();
            this.colStt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMaSp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTenSp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDonGia = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSoLuong = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colKhuyenMai = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colThanhTien = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.btnDong = new System.Windows.Forms.Button();
            this.btnLuuNhapKho = new System.Windows.Forms.Button();
            this.lblTongTien = new System.Windows.Forms.Label();
            this.lblTongTienLabel = new System.Windows.Forms.Label();
            this.btnXoaDong = new System.Windows.Forms.Button();
            this.lblSoMatHang = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlTopInfo.SuspendLayout();
            this.tblTopSplit.SuspendLayout();
            this.grpThongTinChung.SuspendLayout();
            this.grpThemSanPham.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numKhuyenMai)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSoLuong)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiTiet)).BeginInit();
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
            this.lblSubTitle.Size = new System.Drawing.Size(585, 17);
            this.lblSubTitle.TabIndex = 1;
            this.lblSubTitle.Text = "Nhập kho hàng hóa & thiết bị • Quản lý giá vốn và tồn kho doanh nghiệp";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(22, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(430, 28);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "KIOTVIET • LẬP PHIẾU NHẬP KHO THIẾT BỊ";
            // 
            // pnlTopInfo
            // 
            this.pnlTopInfo.BackColor = System.Drawing.Color.White;
            this.pnlTopInfo.Controls.Add(this.tblTopSplit);
            this.pnlTopInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopInfo.Location = new System.Drawing.Point(0, 72);
            this.pnlTopInfo.Name = "pnlTopInfo";
            this.pnlTopInfo.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            this.pnlTopInfo.Size = new System.Drawing.Size(1240, 185);
            this.pnlTopInfo.TabIndex = 1;
            // 
            // tblTopSplit
            // 
            this.tblTopSplit.ColumnCount = 2;
            this.tblTopSplit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 46F));
            this.tblTopSplit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 54F));
            this.tblTopSplit.Controls.Add(this.grpThongTinChung, 0, 0);
            this.tblTopSplit.Controls.Add(this.grpThemSanPham, 1, 0);
            this.tblTopSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblTopSplit.Location = new System.Drawing.Point(16, 8);
            this.tblTopSplit.Name = "tblTopSplit";
            this.tblTopSplit.RowCount = 1;
            this.tblTopSplit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblTopSplit.Size = new System.Drawing.Size(1208, 169);
            this.tblTopSplit.TabIndex = 0;
            // 
            // grpThongTinChung
            // 
            this.grpThongTinChung.Controls.Add(this.cboNhanVien);
            this.grpThongTinChung.Controls.Add(this.lblNhanVien);
            this.grpThongTinChung.Controls.Add(this.cboNhaCungCap);
            this.grpThongTinChung.Controls.Add(this.lblNhaCungCap);
            this.grpThongTinChung.Controls.Add(this.dtpNgayNhap);
            this.grpThongTinChung.Controls.Add(this.lblNgayNhap);
            this.grpThongTinChung.Controls.Add(this.txtMaHdn);
            this.grpThongTinChung.Controls.Add(this.lblMaHdn);
            this.grpThongTinChung.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpThongTinChung.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpThongTinChung.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            this.grpThongTinChung.Location = new System.Drawing.Point(3, 3);
            this.grpThongTinChung.Name = "grpThongTinChung";
            this.grpThongTinChung.Padding = new System.Windows.Forms.Padding(12, 6, 12, 6);
            this.grpThongTinChung.Size = new System.Drawing.Size(549, 163);
            this.grpThongTinChung.TabIndex = 0;
            this.grpThongTinChung.TabStop = false;
            this.grpThongTinChung.Text = "1. Thông Tin Phiếu Nhập";
            // 
            // cboNhanVien
            // 
            this.cboNhanVien.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboNhanVien.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNhanVien.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cboNhanVien.FormattingEnabled = true;
            this.cboNhanVien.Location = new System.Drawing.Point(125, 122);
            this.cboNhanVien.Name = "cboNhanVien";
            this.cboNhanVien.Size = new System.Drawing.Size(405, 24);
            this.cboNhanVien.TabIndex = 7;
            // 
            // lblNhanVien
            // 
            this.lblNhanVien.AutoSize = true;
            this.lblNhanVien.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblNhanVien.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblNhanVien.Location = new System.Drawing.Point(14, 126);
            this.lblNhanVien.Name = "lblNhanVien";
            this.lblNhanVien.Size = new System.Drawing.Size(92, 15);
            this.lblNhanVien.TabIndex = 6;
            this.lblNhanVien.Text = "Nhân viên nhập:";
            // 
            // cboNhaCungCap
            // 
            this.cboNhaCungCap.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboNhaCungCap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNhaCungCap.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cboNhaCungCap.FormattingEnabled = true;
            this.cboNhaCungCap.Location = new System.Drawing.Point(125, 90);
            this.cboNhaCungCap.Name = "cboNhaCungCap";
            this.cboNhaCungCap.Size = new System.Drawing.Size(405, 24);
            this.cboNhaCungCap.TabIndex = 5;
            // 
            // lblNhaCungCap
            // 
            this.lblNhaCungCap.AutoSize = true;
            this.lblNhaCungCap.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblNhaCungCap.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblNhaCungCap.Location = new System.Drawing.Point(14, 94);
            this.lblNhaCungCap.Name = "lblNhaCungCap";
            this.lblNhaCungCap.Size = new System.Drawing.Size(84, 15);
            this.lblNhaCungCap.TabIndex = 4;
            this.lblNhaCungCap.Text = "Nhà cung cấp:";
            // 
            // dtpNgayNhap
            // 
            this.dtpNgayNhap.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpNgayNhap.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.dtpNgayNhap.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.dtpNgayNhap.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpNgayNhap.Location = new System.Drawing.Point(125, 58);
            this.dtpNgayNhap.Name = "dtpNgayNhap";
            this.dtpNgayNhap.Size = new System.Drawing.Size(405, 24);
            this.dtpNgayNhap.TabIndex = 3;
            // 
            // lblNgayNhap
            // 
            this.lblNgayNhap.AutoSize = true;
            this.lblNgayNhap.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblNgayNhap.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblNgayNhap.Location = new System.Drawing.Point(14, 62);
            this.lblNgayNhap.Name = "lblNgayNhap";
            this.lblNgayNhap.Size = new System.Drawing.Size(68, 15);
            this.lblNgayNhap.TabIndex = 2;
            this.lblNgayNhap.Text = "Ngày nhập:";
            // 
            // txtMaHdn
            // 
            this.txtMaHdn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMaHdn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.txtMaHdn.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.txtMaHdn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            this.txtMaHdn.Location = new System.Drawing.Point(125, 26);
            this.txtMaHdn.Name = "txtMaHdn";
            this.txtMaHdn.ReadOnly = true;
            this.txtMaHdn.Size = new System.Drawing.Size(405, 24);
            this.txtMaHdn.TabIndex = 1;
            this.txtMaHdn.Text = "(Tự động sinh khi lưu phiếu)";
            // 
            // lblMaHdn
            // 
            this.lblMaHdn.AutoSize = true;
            this.lblMaHdn.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMaHdn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblMaHdn.Location = new System.Drawing.Point(14, 30);
            this.lblMaHdn.Name = "lblMaHdn";
            this.lblMaHdn.Size = new System.Drawing.Size(73, 15);
            this.lblMaHdn.TabIndex = 0;
            this.lblMaHdn.Text = "Mã hóa đơn:";
            // 
            // grpThemSanPham
            // 
            this.grpThemSanPham.Controls.Add(this.btnThemSp);
            this.grpThemSanPham.Controls.Add(this.numKhuyenMai);
            this.grpThemSanPham.Controls.Add(this.lblKhuyenMai);
            this.grpThemSanPham.Controls.Add(this.txtDonGia);
            this.grpThemSanPham.Controls.Add(this.lblDonGia);
            this.grpThemSanPham.Controls.Add(this.numSoLuong);
            this.grpThemSanPham.Controls.Add(this.lblSoLuong);
            this.grpThemSanPham.Controls.Add(this.cboSanPham);
            this.grpThemSanPham.Controls.Add(this.lblSanPham);
            this.grpThemSanPham.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpThemSanPham.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpThemSanPham.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            this.grpThemSanPham.Location = new System.Drawing.Point(558, 3);
            this.grpThemSanPham.Name = "grpThemSanPham";
            this.grpThemSanPham.Padding = new System.Windows.Forms.Padding(12, 6, 12, 6);
            this.grpThemSanPham.Size = new System.Drawing.Size(647, 163);
            this.grpThemSanPham.TabIndex = 1;
            this.grpThemSanPham.TabStop = false;
            this.grpThemSanPham.Text = "2. Chọn Hàng Hóa Nhập Kho";
            // 
            // btnThemSp
            // 
            this.btnThemSp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnThemSp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(161)))), ((int)(((byte)(55)))));
            this.btnThemSp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnThemSp.FlatAppearance.BorderSize = 0;
            this.btnThemSp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThemSp.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnThemSp.ForeColor = System.Drawing.Color.White;
            this.btnThemSp.Location = new System.Drawing.Point(445, 110);
            this.btnThemSp.Name = "btnThemSp";
            this.btnThemSp.Size = new System.Drawing.Size(185, 38);
            this.btnThemSp.TabIndex = 8;
            this.btnThemSp.Text = "➕ Thêm Vào Phiếu";
            this.btnThemSp.UseVisualStyleBackColor = false;
            this.btnThemSp.Click += new System.EventHandler(this.btnThemSp_Click);
            // 
            // numKhuyenMai
            // 
            this.numKhuyenMai.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.numKhuyenMai.Location = new System.Drawing.Point(315, 118);
            this.numKhuyenMai.Name = "numKhuyenMai";
            this.numKhuyenMai.Size = new System.Drawing.Size(100, 24);
            this.numKhuyenMai.TabIndex = 7;
            // 
            // lblKhuyenMai
            // 
            this.lblKhuyenMai.AutoSize = true;
            this.lblKhuyenMai.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblKhuyenMai.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblKhuyenMai.Location = new System.Drawing.Point(225, 122);
            this.lblKhuyenMai.Name = "lblKhuyenMai";
            this.lblKhuyenMai.Size = new System.Drawing.Size(86, 15);
            this.lblKhuyenMai.TabIndex = 6;
            this.lblKhuyenMai.Text = "Chiết khấu (%):";
            // 
            // txtDonGia
            // 
            this.txtDonGia.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDonGia.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtDonGia.Location = new System.Drawing.Point(105, 75);
            this.txtDonGia.Name = "txtDonGia";
            this.txtDonGia.Size = new System.Drawing.Size(525, 24);
            this.txtDonGia.TabIndex = 5;
            // 
            // lblDonGia
            // 
            this.lblDonGia.AutoSize = true;
            this.lblDonGia.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDonGia.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblDonGia.Location = new System.Drawing.Point(14, 78);
            this.lblDonGia.Name = "lblDonGia";
            this.lblDonGia.Size = new System.Drawing.Size(81, 15);
            this.lblDonGia.TabIndex = 4;
            this.lblDonGia.Text = "Đơn giá nhập:";
            // 
            // numSoLuong
            // 
            this.numSoLuong.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.numSoLuong.Location = new System.Drawing.Point(105, 118);
            this.numSoLuong.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numSoLuong.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSoLuong.Name = "numSoLuong";
            this.numSoLuong.Size = new System.Drawing.Size(100, 24);
            this.numSoLuong.TabIndex = 3;
            this.numSoLuong.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblSoLuong
            // 
            this.lblSoLuong.AutoSize = true;
            this.lblSoLuong.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSoLuong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblSoLuong.Location = new System.Drawing.Point(14, 122);
            this.lblSoLuong.Name = "lblSoLuong";
            this.lblSoLuong.Size = new System.Drawing.Size(87, 15);
            this.lblSoLuong.TabIndex = 2;
            this.lblSoLuong.Text = "Số lượng nhập:";
            // 
            // cboSanPham
            // 
            this.cboSanPham.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSanPham.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSanPham.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cboSanPham.FormattingEnabled = true;
            this.cboSanPham.Location = new System.Drawing.Point(105, 26);
            this.cboSanPham.Name = "cboSanPham";
            this.cboSanPham.Size = new System.Drawing.Size(525, 24);
            this.cboSanPham.TabIndex = 1;
            this.cboSanPham.SelectedIndexChanged += new System.EventHandler(this.cboSanPham_SelectedIndexChanged);
            // 
            // lblSanPham
            // 
            this.lblSanPham.AutoSize = true;
            this.lblSanPham.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSanPham.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblSanPham.Location = new System.Drawing.Point(14, 30);
            this.lblSanPham.Name = "lblSanPham";
            this.lblSanPham.Size = new System.Drawing.Size(63, 15);
            this.lblSanPham.TabIndex = 0;
            this.lblSanPham.Text = "Sản phẩm:";
            // 
            // dgvChiTiet
            // 
            this.dgvChiTiet.AllowUserToAddRows = false;
            this.dgvChiTiet.AllowUserToDeleteRows = false;
            this.dgvChiTiet.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvChiTiet.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.dgvChiTiet.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvChiTiet.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvChiTiet.ColumnHeadersHeight = 36;
            this.dgvChiTiet.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colStt,
            this.colMaSp,
            this.colTenSp,
            this.colDonGia,
            this.colSoLuong,
            this.colKhuyenMai,
            this.colThanhTien});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(248)))), ((int)(((byte)(238)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvChiTiet.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvChiTiet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvChiTiet.EnableHeadersVisualStyles = false;
            this.dgvChiTiet.Location = new System.Drawing.Point(0, 257);
            this.dgvChiTiet.MultiSelect = false;
            this.dgvChiTiet.Name = "dgvChiTiet";
            this.dgvChiTiet.ReadOnly = true;
            this.dgvChiTiet.RowHeadersVisible = false;
            this.dgvChiTiet.RowTemplate.Height = 36;
            this.dgvChiTiet.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvChiTiet.Size = new System.Drawing.Size(1240, 415);
            this.dgvChiTiet.TabIndex = 2;
            // 
            // colStt
            // 
            this.colStt.FillWeight = 40F;
            this.colStt.HeaderText = "STT";
            this.colStt.Name = "colStt";
            this.colStt.ReadOnly = true;
            // 
            // colMaSp
            // 
            this.colMaSp.FillWeight = 80F;
            this.colMaSp.HeaderText = "Mã SP";
            this.colMaSp.Name = "colMaSp";
            this.colMaSp.ReadOnly = true;
            // 
            // colTenSp
            // 
            this.colTenSp.FillWeight = 220F;
            this.colTenSp.HeaderText = "Tên Thiết Bị / Hàng Hóa";
            this.colTenSp.Name = "colTenSp";
            this.colTenSp.ReadOnly = true;
            // 
            // colDonGia
            // 
            this.colDonGia.FillWeight = 90F;
            this.colDonGia.HeaderText = "Đơn Giá Nhập";
            this.colDonGia.Name = "colDonGia";
            this.colDonGia.ReadOnly = true;
            // 
            // colSoLuong
            // 
            this.colSoLuong.FillWeight = 60F;
            this.colSoLuong.HeaderText = "Số Lượng";
            this.colSoLuong.Name = "colSoLuong";
            this.colSoLuong.ReadOnly = true;
            // 
            // colKhuyenMai
            // 
            this.colKhuyenMai.FillWeight = 60F;
            this.colKhuyenMai.HeaderText = "CK (%)";
            this.colKhuyenMai.Name = "colKhuyenMai";
            this.colKhuyenMai.ReadOnly = true;
            // 
            // colThanhTien
            // 
            this.colThanhTien.FillWeight = 110F;
            this.colThanhTien.HeaderText = "Thành Tiền";
            this.colThanhTien.Name = "colThanhTien";
            this.colThanhTien.ReadOnly = true;
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.White;
            this.pnlFooter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFooter.Controls.Add(this.btnDong);
            this.pnlFooter.Controls.Add(this.btnLuuNhapKho);
            this.pnlFooter.Controls.Add(this.lblTongTien);
            this.pnlFooter.Controls.Add(this.lblTongTienLabel);
            this.pnlFooter.Controls.Add(this.btnXoaDong);
            this.pnlFooter.Controls.Add(this.lblSoMatHang);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 672);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Padding = new System.Windows.Forms.Padding(16, 12, 16, 12);
            this.pnlFooter.Size = new System.Drawing.Size(1240, 78);
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
            this.btnDong.Location = new System.Drawing.Point(1125, 16);
            this.btnDong.Name = "btnDong";
            this.btnDong.Size = new System.Drawing.Size(95, 44);
            this.btnDong.TabIndex = 5;
            this.btnDong.Text = "Đóng (Esc)";
            this.btnDong.UseVisualStyleBackColor = false;
            this.btnDong.Click += new System.EventHandler(this.btnDong_Click);
            // 
            // btnLuuNhapKho
            // 
            this.btnLuuNhapKho.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLuuNhapKho.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(161)))), ((int)(((byte)(55)))));
            this.btnLuuNhapKho.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLuuNhapKho.FlatAppearance.BorderSize = 0;
            this.btnLuuNhapKho.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLuuNhapKho.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnLuuNhapKho.ForeColor = System.Drawing.Color.White;
            this.btnLuuNhapKho.Location = new System.Drawing.Point(865, 16);
            this.btnLuuNhapKho.Name = "btnLuuNhapKho";
            this.btnLuuNhapKho.Size = new System.Drawing.Size(250, 44);
            this.btnLuuNhapKho.TabIndex = 4;
            this.btnLuuNhapKho.Text = "💾 LƯU PHIẾU NHẬP (F8)";
            this.btnLuuNhapKho.UseVisualStyleBackColor = false;
            this.btnLuuNhapKho.Click += new System.EventHandler(this.btnLuuNhapKho_Click);
            // 
            // lblTongTien
            // 
            this.lblTongTien.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTongTien.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTongTien.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(161)))), ((int)(((byte)(55)))));
            this.lblTongTien.Location = new System.Drawing.Point(580, 20);
            this.lblTongTien.Name = "lblTongTien";
            this.lblTongTien.Size = new System.Drawing.Size(270, 36);
            this.lblTongTien.TabIndex = 2;
            this.lblTongTien.Text = "0 ₫";
            this.lblTongTien.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTongTienLabel
            // 
            this.lblTongTienLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTongTienLabel.AutoSize = true;
            this.lblTongTienLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTongTienLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblTongTienLabel.Location = new System.Drawing.Point(465, 30);
            this.lblTongTienLabel.Name = "lblTongTienLabel";
            this.lblTongTienLabel.Size = new System.Drawing.Size(115, 19);
            this.lblTongTienLabel.TabIndex = 1;
            this.lblTongTienLabel.Text = "Tổng tiền nhập:";
            // 
            // btnXoaDong
            // 
            this.btnXoaDong.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.btnXoaDong.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnXoaDong.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(165)))), ((int)(((byte)(165)))));
            this.btnXoaDong.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXoaDong.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnXoaDong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnXoaDong.Location = new System.Drawing.Point(16, 18);
            this.btnXoaDong.Name = "btnXoaDong";
            this.btnXoaDong.Size = new System.Drawing.Size(155, 40);
            this.btnXoaDong.TabIndex = 3;
            this.btnXoaDong.Text = "🗑️ Xóa Mặt Hàng";
            this.btnXoaDong.UseVisualStyleBackColor = false;
            this.btnXoaDong.Click += new System.EventHandler(this.btnXoaDong_Click);
            // 
            // lblSoMatHang
            // 
            this.lblSoMatHang.AutoSize = true;
            this.lblSoMatHang.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblSoMatHang.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblSoMatHang.Location = new System.Drawing.Point(185, 30);
            this.lblSoMatHang.Name = "lblSoMatHang";
            this.lblSoMatHang.Size = new System.Drawing.Size(161, 17);
            this.lblSoMatHang.TabIndex = 0;
            this.lblSoMatHang.Text = "Đang chọn: 0 mặt hàng";
            // 
            // frmHoaDonNhap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1240, 750);
            this.Controls.Add(this.dgvChiTiet);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlTopInfo);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimumSize = new System.Drawing.Size(1024, 620);
            this.Name = "frmHoaDonNhap";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "KiotViet POS • Lập Hóa Đơn Nhập Kho Thiết Bị";
            this.Load += new System.EventHandler(this.frmHoaDonNhap_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlTopInfo.ResumeLayout(false);
            this.tblTopSplit.ResumeLayout(false);
            this.grpThongTinChung.ResumeLayout(false);
            this.grpThongTinChung.PerformLayout();
            this.grpThemSanPham.ResumeLayout(false);
            this.grpThemSanPham.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numKhuyenMai)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSoLuong)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiTiet)).EndInit();
            this.pnlFooter.ResumeLayout(false);
            this.pnlFooter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Panel pnlHeaderAccent;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubTitle;
        private System.Windows.Forms.Panel pnlTopInfo;
        private System.Windows.Forms.TableLayoutPanel tblTopSplit;
        private System.Windows.Forms.GroupBox grpThongTinChung;
        private System.Windows.Forms.TextBox txtMaHdn;
        private System.Windows.Forms.Label lblMaHdn;
        private System.Windows.Forms.DateTimePicker dtpNgayNhap;
        private System.Windows.Forms.Label lblNgayNhap;
        private System.Windows.Forms.ComboBox cboNhaCungCap;
        private System.Windows.Forms.Label lblNhaCungCap;
        private System.Windows.Forms.ComboBox cboNhanVien;
        private System.Windows.Forms.Label lblNhanVien;
        private System.Windows.Forms.GroupBox grpThemSanPham;
        private System.Windows.Forms.ComboBox cboSanPham;
        private System.Windows.Forms.Label lblSanPham;
        private System.Windows.Forms.NumericUpDown numSoLuong;
        private System.Windows.Forms.Label lblSoLuong;
        private System.Windows.Forms.TextBox txtDonGia;
        private System.Windows.Forms.Label lblDonGia;
        private System.Windows.Forms.NumericUpDown numKhuyenMai;
        private System.Windows.Forms.Label lblKhuyenMai;
        private System.Windows.Forms.Button btnThemSp;
        private System.Windows.Forms.DataGridView dgvChiTiet;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStt;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMaSp;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTenSp;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDonGia;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSoLuong;
        private System.Windows.Forms.DataGridViewTextBoxColumn colKhuyenMai;
        private System.Windows.Forms.DataGridViewTextBoxColumn colThanhTien;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Label lblSoMatHang;
        private System.Windows.Forms.Label lblTongTienLabel;
        private System.Windows.Forms.Label lblTongTien;
        private System.Windows.Forms.Button btnXoaDong;
        private System.Windows.Forms.Button btnLuuNhapKho;
        private System.Windows.Forms.Button btnDong;
    }
}
