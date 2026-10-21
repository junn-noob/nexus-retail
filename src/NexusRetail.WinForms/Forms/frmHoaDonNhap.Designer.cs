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
            this.lblSubTitle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlTopInfo = new System.Windows.Forms.Panel();
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
            this.btnXoaDong = new System.Windows.Forms.Button();
            this.lblTongTien = new System.Windows.Forms.Label();
            this.lblTongTienLabel = new System.Windows.Forms.Label();
            this.lblSoMatHang = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlTopInfo.SuspendLayout();
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
            this.lblSubTitle.Size = new System.Drawing.Size(650, 17);
            this.lblSubTitle.TabIndex = 1;
            this.lblSubTitle.Text = "Hệ thống tự động thực thi BR-01 (Tăng tồn), BR-02 (Cập nhật giá vốn), BR-03 (Giá bán = Giá vốn x 110%)";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(22, 11);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(370, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "LẬP HÓA ĐƠN NHẬP KHO THIẾT BỊ";
            // 
            // pnlTopInfo
            // 
            this.pnlTopInfo.BackColor = System.Drawing.Color.White;
            this.pnlTopInfo.Controls.Add(this.grpThongTinChung);
            this.pnlTopInfo.Controls.Add(this.grpThemSanPham);
            this.pnlTopInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopInfo.Location = new System.Drawing.Point(0, 75);
            this.pnlTopInfo.Name = "pnlTopInfo";
            this.pnlTopInfo.Padding = new System.Windows.Forms.Padding(16, 12, 16, 8);
            this.pnlTopInfo.Size = new System.Drawing.Size(1008, 175);
            this.pnlTopInfo.TabIndex = 1;
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
            this.grpThongTinChung.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grpThongTinChung.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.grpThongTinChung.Location = new System.Drawing.Point(16, 10);
            this.grpThongTinChung.Name = "grpThongTinChung";
            this.grpThongTinChung.Size = new System.Drawing.Size(460, 155);
            this.grpThongTinChung.TabIndex = 0;
            this.grpThongTinChung.TabStop = false;
            this.grpThongTinChung.Text = "1. Thông tin phiếu nhập";
            // 
            // cboNhanVien
            // 
            this.cboNhanVien.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNhanVien.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cboNhanVien.FormattingEnabled = true;
            this.cboNhanVien.Location = new System.Drawing.Point(120, 115);
            this.cboNhanVien.Name = "cboNhanVien";
            this.cboNhanVien.Size = new System.Drawing.Size(325, 25);
            this.cboNhanVien.TabIndex = 7;
            // 
            // lblNhanVien
            // 
            this.lblNhanVien.AutoSize = true;
            this.lblNhanVien.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblNhanVien.Location = new System.Drawing.Point(15, 120);
            this.lblNhanVien.Name = "lblNhanVien";
            this.lblNhanVien.Size = new System.Drawing.Size(92, 15);
            this.lblNhanVien.TabIndex = 6;
            this.lblNhanVien.Text = "Nhân viên nhập:";
            // 
            // cboNhaCungCap
            // 
            this.cboNhaCungCap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNhaCungCap.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cboNhaCungCap.FormattingEnabled = true;
            this.cboNhaCungCap.Location = new System.Drawing.Point(120, 85);
            this.cboNhaCungCap.Name = "cboNhaCungCap";
            this.cboNhaCungCap.Size = new System.Drawing.Size(325, 25);
            this.cboNhaCungCap.TabIndex = 5;
            // 
            // lblNhaCungCap
            // 
            this.lblNhaCungCap.AutoSize = true;
            this.lblNhaCungCap.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblNhaCungCap.Location = new System.Drawing.Point(15, 90);
            this.lblNhaCungCap.Name = "lblNhaCungCap";
            this.lblNhaCungCap.Size = new System.Drawing.Size(84, 15);
            this.lblNhaCungCap.TabIndex = 4;
            this.lblNhaCungCap.Text = "Nhà cung cấp:";
            // 
            // dtpNgayNhap
            // 
            this.dtpNgayNhap.CustomFormat = "dd/MM/yyyy HH:mm";
            this.dtpNgayNhap.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.dtpNgayNhap.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpNgayNhap.Location = new System.Drawing.Point(120, 55);
            this.dtpNgayNhap.Name = "dtpNgayNhap";
            this.dtpNgayNhap.Size = new System.Drawing.Size(325, 24);
            this.dtpNgayNhap.TabIndex = 3;
            // 
            // lblNgayNhap
            // 
            this.lblNgayNhap.AutoSize = true;
            this.lblNgayNhap.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblNgayNhap.Location = new System.Drawing.Point(15, 60);
            this.lblNgayNhap.Name = "lblNgayNhap";
            this.lblNgayNhap.Size = new System.Drawing.Size(68, 15);
            this.lblNgayNhap.TabIndex = 2;
            this.lblNgayNhap.Text = "Ngày nhập:";
            // 
            // txtMaHdn
            // 
            this.txtMaHdn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.txtMaHdn.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.txtMaHdn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.txtMaHdn.Location = new System.Drawing.Point(120, 25);
            this.txtMaHdn.Name = "txtMaHdn";
            this.txtMaHdn.ReadOnly = true;
            this.txtMaHdn.Size = new System.Drawing.Size(325, 24);
            this.txtMaHdn.TabIndex = 1;
            this.txtMaHdn.Text = "(Tự động sinh khi lưu)";
            // 
            // lblMaHdn
            // 
            this.lblMaHdn.AutoSize = true;
            this.lblMaHdn.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMaHdn.Location = new System.Drawing.Point(15, 30);
            this.lblMaHdn.Name = "lblMaHdn";
            this.lblMaHdn.Size = new System.Drawing.Size(73, 15);
            this.lblMaHdn.TabIndex = 0;
            this.lblMaHdn.Text = "Mã hóa đơn:";
            // 
            // grpThemSanPham
            // 
            this.grpThemSanPham.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpThemSanPham.Controls.Add(this.btnThemSp);
            this.grpThemSanPham.Controls.Add(this.numKhuyenMai);
            this.grpThemSanPham.Controls.Add(this.lblKhuyenMai);
            this.grpThemSanPham.Controls.Add(this.txtDonGia);
            this.grpThemSanPham.Controls.Add(this.lblDonGia);
            this.grpThemSanPham.Controls.Add(this.numSoLuong);
            this.grpThemSanPham.Controls.Add(this.lblSoLuong);
            this.grpThemSanPham.Controls.Add(this.cboSanPham);
            this.grpThemSanPham.Controls.Add(this.lblSanPham);
            this.grpThemSanPham.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grpThemSanPham.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.grpThemSanPham.Location = new System.Drawing.Point(490, 10);
            this.grpThemSanPham.Name = "grpThemSanPham";
            this.grpThemSanPham.Size = new System.Drawing.Size(502, 155);
            this.grpThemSanPham.TabIndex = 1;
            this.grpThemSanPham.TabStop = false;
            this.grpThemSanPham.Text = "2. Chọn mặt hàng nhập kho";
            // 
            // btnThemSp
            // 
            this.btnThemSp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnThemSp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(70)))), ((int)(((byte)(229)))));
            this.btnThemSp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnThemSp.FlatAppearance.BorderSize = 0;
            this.btnThemSp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThemSp.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnThemSp.ForeColor = System.Drawing.Color.White;
            this.btnThemSp.Location = new System.Drawing.Point(315, 110);
            this.btnThemSp.Name = "btnThemSp";
            this.btnThemSp.Size = new System.Drawing.Size(170, 32);
            this.btnThemSp.TabIndex = 8;
            this.btnThemSp.Text = "➕ Thêm vào phiếu";
            this.btnThemSp.UseVisualStyleBackColor = false;
            this.btnThemSp.Click += new System.EventHandler(this.btnThemSp_Click);
            // 
            // numKhuyenMai
            // 
            this.numKhuyenMai.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.numKhuyenMai.Location = new System.Drawing.Point(110, 115);
            this.numKhuyenMai.Name = "numKhuyenMai";
            this.numKhuyenMai.Size = new System.Drawing.Size(120, 24);
            this.numKhuyenMai.TabIndex = 7;
            // 
            // lblKhuyenMai
            // 
            this.lblKhuyenMai.AutoSize = true;
            this.lblKhuyenMai.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblKhuyenMai.Location = new System.Drawing.Point(15, 120);
            this.lblKhuyenMai.Name = "lblKhuyenMai";
            this.lblKhuyenMai.Size = new System.Drawing.Size(92, 15);
            this.lblKhuyenMai.TabIndex = 6;
            this.lblKhuyenMai.Text = "Chiết khấu (%):";
            // 
            // txtDonGia
            // 
            this.txtDonGia.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDonGia.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtDonGia.Location = new System.Drawing.Point(110, 85);
            this.txtDonGia.Name = "txtDonGia";
            this.txtDonGia.Size = new System.Drawing.Size(375, 24);
            this.txtDonGia.TabIndex = 5;
            // 
            // lblDonGia
            // 
            this.lblDonGia.AutoSize = true;
            this.lblDonGia.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDonGia.Location = new System.Drawing.Point(15, 90);
            this.lblDonGia.Name = "lblDonGia";
            this.lblDonGia.Size = new System.Drawing.Size(81, 15);
            this.lblDonGia.TabIndex = 4;
            this.lblDonGia.Text = "Đơn giá nhập:";
            // 
            // numSoLuong
            // 
            this.numSoLuong.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.numSoLuong.Location = new System.Drawing.Point(110, 55);
            this.numSoLuong.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numSoLuong.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSoLuong.Name = "numSoLuong";
            this.numSoLuong.Size = new System.Drawing.Size(120, 24);
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
            this.lblSoLuong.Location = new System.Drawing.Point(15, 60);
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
            this.cboSanPham.Location = new System.Drawing.Point(110, 25);
            this.cboSanPham.Name = "cboSanPham";
            this.cboSanPham.Size = new System.Drawing.Size(375, 25);
            this.cboSanPham.TabIndex = 1;
            this.cboSanPham.SelectedIndexChanged += new System.EventHandler(this.cboSanPham_SelectedIndexChanged);
            // 
            // lblSanPham
            // 
            this.lblSanPham.AutoSize = true;
            this.lblSanPham.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSanPham.Location = new System.Drawing.Point(15, 30);
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
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvChiTiet.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvChiTiet.ColumnHeadersHeight = 35;
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
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(231)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvChiTiet.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvChiTiet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvChiTiet.EnableHeadersVisualStyles = false;
            this.dgvChiTiet.Location = new System.Drawing.Point(0, 250);
            this.dgvChiTiet.MultiSelect = false;
            this.dgvChiTiet.Name = "dgvChiTiet";
            this.dgvChiTiet.ReadOnly = true;
            this.dgvChiTiet.RowHeadersVisible = false;
            this.dgvChiTiet.RowTemplate.Height = 30;
            this.dgvChiTiet.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvChiTiet.Size = new System.Drawing.Size(1008, 350);
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
            this.colTenSp.HeaderText = "Tên Thiết Bị / Linh Kiện";
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
            this.pnlFooter.Controls.Add(this.btnXoaDong);
            this.pnlFooter.Controls.Add(this.lblTongTien);
            this.pnlFooter.Controls.Add(this.lblTongTienLabel);
            this.pnlFooter.Controls.Add(this.lblSoMatHang);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 600);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Padding = new System.Windows.Forms.Padding(16, 12, 16, 12);
            this.pnlFooter.Size = new System.Drawing.Size(1008, 80);
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
            this.btnDong.Location = new System.Drawing.Point(895, 20);
            this.btnDong.Name = "btnDong";
            this.btnDong.Size = new System.Drawing.Size(95, 38);
            this.btnDong.TabIndex = 5;
            this.btnDong.Text = "Đóng";
            this.btnDong.UseVisualStyleBackColor = false;
            this.btnDong.Click += new System.EventHandler(this.btnDong_Click);
            // 
            // btnLuuNhapKho
            // 
            this.btnLuuNhapKho.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLuuNhapKho.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.btnLuuNhapKho.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLuuNhapKho.FlatAppearance.BorderSize = 0;
            this.btnLuuNhapKho.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLuuNhapKho.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLuuNhapKho.ForeColor = System.Drawing.Color.White;
            this.btnLuuNhapKho.Location = new System.Drawing.Point(670, 20);
            this.btnLuuNhapKho.Name = "btnLuuNhapKho";
            this.btnLuuNhapKho.Size = new System.Drawing.Size(215, 38);
            this.btnLuuNhapKho.TabIndex = 4;
            this.btnLuuNhapKho.Text = "💾 Lưu & Hoàn tất nhập kho";
            this.btnLuuNhapKho.UseVisualStyleBackColor = false;
            this.btnLuuNhapKho.Click += new System.EventHandler(this.btnLuuNhapKho_Click);
            // 
            // btnXoaDong
            // 
            this.btnXoaDong.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.btnXoaDong.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnXoaDong.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(165)))), ((int)(((byte)(165)))));
            this.btnXoaDong.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXoaDong.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnXoaDong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnXoaDong.Location = new System.Drawing.Point(16, 20);
            this.btnXoaDong.Name = "btnXoaDong";
            this.btnXoaDong.Size = new System.Drawing.Size(120, 38);
            this.btnXoaDong.TabIndex = 3;
            this.btnXoaDong.Text = "🗑️ Xóa dòng chọn";
            this.btnXoaDong.UseVisualStyleBackColor = false;
            this.btnXoaDong.Click += new System.EventHandler(this.btnXoaDong_Click);
            // 
            // lblTongTien
            // 
            this.lblTongTien.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTongTien.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTongTien.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.lblTongTien.Location = new System.Drawing.Point(400, 24);
            this.lblTongTien.Name = "lblTongTien";
            this.lblTongTien.Size = new System.Drawing.Size(250, 30);
            this.lblTongTien.TabIndex = 2;
            this.lblTongTien.Text = "0 đ";
            this.lblTongTien.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTongTienLabel
            // 
            this.lblTongTienLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTongTienLabel.AutoSize = true;
            this.lblTongTienLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTongTienLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblTongTienLabel.Location = new System.Drawing.Point(280, 30);
            this.lblTongTienLabel.Name = "lblTongTienLabel";
            this.lblTongTienLabel.Size = new System.Drawing.Size(117, 19);
            this.lblTongTienLabel.TabIndex = 1;
            this.lblTongTienLabel.Text = "Tổng tiền nhập:";
            // 
            // lblSoMatHang
            // 
            this.lblSoMatHang.AutoSize = true;
            this.lblSoMatHang.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblSoMatHang.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblSoMatHang.Location = new System.Drawing.Point(150, 30);
            this.lblSoMatHang.Name = "lblSoMatHang";
            this.lblSoMatHang.Size = new System.Drawing.Size(126, 17);
            this.lblSoMatHang.TabIndex = 0;
            this.lblSoMatHang.Text = "Số mặt hàng nhập: 0";
            // 
            // frmHoaDonNhap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 680);
            this.Controls.Add(this.dgvChiTiet);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlTopInfo);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(950, 600);
            this.Name = "frmHoaDonNhap";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nexus Retail - Lập Hóa Đơn Nhập Hàng";
            this.Load += new System.EventHandler(this.frmHoaDonNhap_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlTopInfo.ResumeLayout(false);
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
        private System.Windows.Forms.Label lblSubTitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlTopInfo;
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
        private System.Windows.Forms.Button btnDong;
        private System.Windows.Forms.Button btnLuuNhapKho;
        private System.Windows.Forms.Button btnXoaDong;
        private System.Windows.Forms.Label lblTongTien;
        private System.Windows.Forms.Label lblTongTienLabel;
        private System.Windows.Forms.Label lblSoMatHang;
    }
}
