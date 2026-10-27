namespace NexusRetail.WinForms.Forms
{
    partial class frmHoaDonBan
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

        private void InitializeComponent()
        {
            this.pnlTop = new System.Windows.Forms.Panel();
            this.pnlHeaderAccent = new System.Windows.Forms.Panel();
            this.lblSub = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.pnlGridContainer = new System.Windows.Forms.Panel();
            this.dgvChiTiet = new System.Windows.Forms.DataGridView();
            this.pnlProductPicker = new System.Windows.Forms.Panel();
            this.btnXoaMon = new System.Windows.Forms.Button();
            this.btnThemMon = new System.Windows.Forms.Button();
            this.lblTonKho = new System.Windows.Forms.Label();
            this.numKhuyenMai = new System.Windows.Forms.NumericUpDown();
            this.lblKhuyenMai = new System.Windows.Forms.Label();
            this.numSoLuong = new System.Windows.Forms.NumericUpDown();
            this.lblSoLuong = new System.Windows.Forms.Label();
            this.txtDonGia = new System.Windows.Forms.TextBox();
            this.lblDonGia = new System.Windows.Forms.Label();
            this.cboSanPham = new System.Windows.Forms.ComboBox();
            this.lblChonSp = new System.Windows.Forms.Label();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.pnlPaymentActions = new System.Windows.Forms.Panel();
            this.btnThanhToanQR = new System.Windows.Forms.Button();
            this.btnThanhToanTienMat = new System.Windows.Forms.Button();
            this.pnlSummary = new System.Windows.Forms.Panel();
            this.lblTongTien = new System.Windows.Forms.Label();
            this.lblLabelTong = new System.Windows.Forms.Label();
            this.grpCustomer = new System.Windows.Forms.GroupBox();
            this.txtDienThoai = new System.Windows.Forms.TextBox();
            this.lblDienThoai = new System.Windows.Forms.Label();
            this.txtTenKhach = new System.Windows.Forms.TextBox();
            this.lblTenKhach = new System.Windows.Forms.Label();
            this.dtpNgayBan = new System.Windows.Forms.DateTimePicker();
            this.lblNgayBan = new System.Windows.Forms.Label();
            this.txtNhanVien = new System.Windows.Forms.TextBox();
            this.lblNhanVien = new System.Windows.Forms.Label();
            this.pnlTop.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.pnlGridContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiTiet)).BeginInit();
            this.pnlProductPicker.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numKhuyenMai)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSoLuong)).BeginInit();
            this.pnlRight.SuspendLayout();
            this.pnlPaymentActions.SuspendLayout();
            this.pnlSummary.SuspendLayout();
            this.grpCustomer.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            this.pnlTop.Controls.Add(this.pnlHeaderAccent);
            this.pnlTop.Controls.Add(this.lblSub);
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1244, 70);
            this.pnlTop.TabIndex = 0;
            // 
            // pnlHeaderAccent
            // 
            this.pnlHeaderAccent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(161)))), ((int)(((byte)(55)))));
            this.pnlHeaderAccent.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlHeaderAccent.Location = new System.Drawing.Point(0, 66);
            this.pnlHeaderAccent.Name = "pnlHeaderAccent";
            this.pnlHeaderAccent.Size = new System.Drawing.Size(1244, 4);
            this.pnlHeaderAccent.TabIndex = 2;
            // 
            // lblSub
            // 
            this.lblSub.AutoSize = true;
            this.lblSub.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblSub.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.lblSub.Location = new System.Drawing.Point(20, 40);
            this.lblSub.Name = "lblSub";
            this.lblSub.Size = new System.Drawing.Size(430, 17);
            this.lblSub.TabIndex = 1;
            this.lblSub.Text = "Hệ thống Bán Lẻ Thu Ngân KiotViet • Lập hóa đơn và thanh toán tức thì";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(18, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(438, 28);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "LẬP HÓA ĐƠN BÁN HÀNG TẠI QUẦY (POS)";
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.pnlLeft);
            this.pnlMain.Controls.Add(this.pnlRight);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 70);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(12);
            this.pnlMain.Size = new System.Drawing.Size(1244, 650);
            this.pnlMain.TabIndex = 1;
            // 
            // pnlLeft
            // 
            this.pnlLeft.Controls.Add(this.pnlGridContainer);
            this.pnlLeft.Controls.Add(this.pnlProductPicker);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLeft.Location = new System.Drawing.Point(12, 12);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(850, 626);
            this.pnlLeft.TabIndex = 0;
            // 
            // pnlGridContainer
            // 
            this.pnlGridContainer.BackColor = System.Drawing.Color.White;
            this.pnlGridContainer.Controls.Add(this.dgvChiTiet);
            this.pnlGridContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGridContainer.Location = new System.Drawing.Point(0, 96);
            this.pnlGridContainer.Name = "pnlGridContainer";
            this.pnlGridContainer.Size = new System.Drawing.Size(850, 530);
            this.pnlGridContainer.TabIndex = 1;
            // 
            // dgvChiTiet
            // 
            this.dgvChiTiet.AllowUserToAddRows = false;
            this.dgvChiTiet.BackgroundColor = System.Drawing.Color.White;
            this.dgvChiTiet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvChiTiet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvChiTiet.Location = new System.Drawing.Point(0, 0);
            this.dgvChiTiet.Name = "dgvChiTiet";
            this.dgvChiTiet.ReadOnly = true;
            this.dgvChiTiet.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvChiTiet.Size = new System.Drawing.Size(850, 530);
            this.dgvChiTiet.TabIndex = 0;
            // 
            // pnlProductPicker
            // 
            this.pnlProductPicker.BackColor = System.Drawing.Color.White;
            this.pnlProductPicker.Controls.Add(this.btnXoaMon);
            this.pnlProductPicker.Controls.Add(this.btnThemMon);
            this.pnlProductPicker.Controls.Add(this.lblTonKho);
            this.pnlProductPicker.Controls.Add(this.numKhuyenMai);
            this.pnlProductPicker.Controls.Add(this.lblKhuyenMai);
            this.pnlProductPicker.Controls.Add(this.numSoLuong);
            this.pnlProductPicker.Controls.Add(this.lblSoLuong);
            this.pnlProductPicker.Controls.Add(this.txtDonGia);
            this.pnlProductPicker.Controls.Add(this.lblDonGia);
            this.pnlProductPicker.Controls.Add(this.cboSanPham);
            this.pnlProductPicker.Controls.Add(this.lblChonSp);
            this.pnlProductPicker.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlProductPicker.Location = new System.Drawing.Point(0, 0);
            this.pnlProductPicker.Name = "pnlProductPicker";
            this.pnlProductPicker.Padding = new System.Windows.Forms.Padding(12);
            this.pnlProductPicker.Size = new System.Drawing.Size(850, 96);
            this.pnlProductPicker.TabIndex = 0;
            // 
            // btnXoaMon
            // 
            this.btnXoaMon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnXoaMon.BackColor = System.Drawing.Color.White;
            this.btnXoaMon.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnXoaMon.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(202)))), ((int)(((byte)(202)))));
            this.btnXoaMon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXoaMon.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnXoaMon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnXoaMon.Location = new System.Drawing.Point(710, 48);
            this.btnXoaMon.Name = "btnXoaMon";
            this.btnXoaMon.Size = new System.Drawing.Size(128, 36);
            this.btnXoaMon.TabIndex = 10;
            this.btnXoaMon.Text = "🗑️ Xóa món";
            this.btnXoaMon.UseVisualStyleBackColor = false;
            this.btnXoaMon.Click += new System.EventHandler(this.btnXoaMon_Click);
            // 
            // btnThemMon
            // 
            this.btnThemMon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnThemMon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(161)))), ((int)(((byte)(55)))));
            this.btnThemMon.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnThemMon.FlatAppearance.BorderSize = 0;
            this.btnThemMon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThemMon.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnThemMon.ForeColor = System.Drawing.Color.White;
            this.btnThemMon.Location = new System.Drawing.Point(560, 48);
            this.btnThemMon.Name = "btnThemMon";
            this.btnThemMon.Size = new System.Drawing.Size(140, 36);
            this.btnThemMon.TabIndex = 9;
            this.btnThemMon.Text = "➕ Thêm vào đơn";
            this.btnThemMon.UseVisualStyleBackColor = false;
            this.btnThemMon.Click += new System.EventHandler(this.btnThemMon_Click);
            // 
            // lblTonKho
            // 
            this.lblTonKho.AutoSize = true;
            this.lblTonKho.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTonKho.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(161)))), ((int)(((byte)(55)))));
            this.lblTonKho.Location = new System.Drawing.Point(92, 60);
            this.lblTonKho.Name = "lblTonKho";
            this.lblTonKho.Size = new System.Drawing.Size(112, 17);
            this.lblTonKho.TabIndex = 8;
            this.lblTonKho.Text = "Tồn kho: 10 máy";
            // 
            // numKhuyenMai
            // 
            this.numKhuyenMai.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.numKhuyenMai.Location = new System.Drawing.Point(680, 12);
            this.numKhuyenMai.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            this.numKhuyenMai.Name = "numKhuyenMai";
            this.numKhuyenMai.Size = new System.Drawing.Size(130, 26);
            this.numKhuyenMai.TabIndex = 7;
            // 
            // lblKhuyenMai
            // 
            this.lblKhuyenMai.AutoSize = true;
            this.lblKhuyenMai.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblKhuyenMai.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblKhuyenMai.Location = new System.Drawing.Point(600, 16);
            this.lblKhuyenMai.Name = "lblKhuyenMai";
            this.lblKhuyenMai.Size = new System.Drawing.Size(76, 17);
            this.lblKhuyenMai.TabIndex = 6;
            this.lblKhuyenMai.Text = "Giảm giá ₫:";
            // 
            // numSoLuong
            // 
            this.numSoLuong.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.numSoLuong.Location = new System.Drawing.Point(520, 12);
            this.numSoLuong.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numSoLuong.Name = "numSoLuong";
            this.numSoLuong.Size = new System.Drawing.Size(65, 26);
            this.numSoLuong.TabIndex = 5;
            this.numSoLuong.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblSoLuong
            // 
            this.lblSoLuong.AutoSize = true;
            this.lblSoLuong.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSoLuong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblSoLuong.Location = new System.Drawing.Point(485, 16);
            this.lblSoLuong.Name = "lblSoLuong";
            this.lblSoLuong.Size = new System.Drawing.Size(28, 17);
            this.lblSoLuong.TabIndex = 4;
            this.lblSoLuong.Text = "SL:";
            // 
            // txtDonGia
            // 
            this.txtDonGia.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.txtDonGia.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtDonGia.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(110)))), ((int)(((byte)(253)))));
            this.txtDonGia.Location = new System.Drawing.Point(340, 12);
            this.txtDonGia.Name = "txtDonGia";
            this.txtDonGia.ReadOnly = true;
            this.txtDonGia.Size = new System.Drawing.Size(130, 26);
            this.txtDonGia.TabIndex = 3;
            // 
            // lblDonGia
            // 
            this.lblDonGia.AutoSize = true;
            this.lblDonGia.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblDonGia.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblDonGia.Location = new System.Drawing.Point(275, 16);
            this.lblDonGia.Name = "lblDonGia";
            this.lblDonGia.Size = new System.Drawing.Size(60, 17);
            this.lblDonGia.TabIndex = 2;
            this.lblDonGia.Text = "Đơn giá:";
            // 
            // cboSanPham
            // 
            this.cboSanPham.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSanPham.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cboSanPham.FormattingEnabled = true;
            this.cboSanPham.Location = new System.Drawing.Point(92, 12);
            this.cboSanPham.Name = "cboSanPham";
            this.cboSanPham.Size = new System.Drawing.Size(170, 27);
            this.cboSanPham.TabIndex = 1;
            this.cboSanPham.SelectedIndexChanged += new System.EventHandler(this.cboSanPham_SelectedIndexChanged);
            // 
            // lblChonSp
            // 
            this.lblChonSp.AutoSize = true;
            this.lblChonSp.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblChonSp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblChonSp.Location = new System.Drawing.Point(12, 16);
            this.lblChonSp.Name = "lblChonSp";
            this.lblChonSp.Size = new System.Drawing.Size(76, 17);
            this.lblChonSp.TabIndex = 0;
            this.lblChonSp.Text = "Chọn máy:";
            // 
            // pnlRight
            // 
            this.pnlRight.BackColor = System.Drawing.Color.White;
            this.pnlRight.Controls.Add(this.pnlPaymentActions);
            this.pnlRight.Controls.Add(this.pnlSummary);
            this.pnlRight.Controls.Add(this.grpCustomer);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlRight.Location = new System.Drawing.Point(862, 12);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Padding = new System.Windows.Forms.Padding(16);
            this.pnlRight.Size = new System.Drawing.Size(370, 626);
            this.pnlRight.TabIndex = 1;
            // 
            // pnlPaymentActions
            // 
            this.pnlPaymentActions.Controls.Add(this.btnThanhToanQR);
            this.pnlPaymentActions.Controls.Add(this.btnThanhToanTienMat);
            this.pnlPaymentActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlPaymentActions.Location = new System.Drawing.Point(16, 486);
            this.pnlPaymentActions.Name = "pnlPaymentActions";
            this.pnlPaymentActions.Size = new System.Drawing.Size(338, 124);
            this.pnlPaymentActions.TabIndex = 2;
            // 
            // btnThanhToanQR
            // 
            this.btnThanhToanQR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(110)))), ((int)(((byte)(253)))));
            this.btnThanhToanQR.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnThanhToanQR.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnThanhToanQR.FlatAppearance.BorderSize = 0;
            this.btnThanhToanQR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThanhToanQR.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnThanhToanQR.ForeColor = System.Drawing.Color.White;
            this.btnThanhToanQR.Location = new System.Drawing.Point(0, 58);
            this.btnThanhToanQR.Name = "btnThanhToanQR";
            this.btnThanhToanQR.Size = new System.Drawing.Size(338, 52);
            this.btnThanhToanQR.TabIndex = 1;
            this.btnThanhToanQR.Text = "📱 THANH TOÁN VIETQR (F9)";
            this.btnThanhToanQR.UseVisualStyleBackColor = false;
            this.btnThanhToanQR.Click += new System.EventHandler(this.btnThanhToanQR_Click);
            // 
            // btnThanhToanTienMat
            // 
            this.btnThanhToanTienMat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(161)))), ((int)(((byte)(55)))));
            this.btnThanhToanTienMat.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnThanhToanTienMat.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnThanhToanTienMat.FlatAppearance.BorderSize = 0;
            this.btnThanhToanTienMat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThanhToanTienMat.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnThanhToanTienMat.ForeColor = System.Drawing.Color.White;
            this.btnThanhToanTienMat.Location = new System.Drawing.Point(0, 0);
            this.btnThanhToanTienMat.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.btnThanhToanTienMat.Name = "btnThanhToanTienMat";
            this.btnThanhToanTienMat.Size = new System.Drawing.Size(338, 52);
            this.btnThanhToanTienMat.TabIndex = 0;
            this.btnThanhToanTienMat.Text = "💵 THANH TOÁN TIỀN MẶT (F8)";
            this.btnThanhToanTienMat.UseVisualStyleBackColor = false;
            this.btnThanhToanTienMat.Click += new System.EventHandler(this.btnThanhToanTienMat_Click);
            // 
            // pnlSummary
            // 
            this.pnlSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.pnlSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSummary.Controls.Add(this.lblTongTien);
            this.pnlSummary.Controls.Add(this.lblLabelTong);
            this.pnlSummary.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSummary.Location = new System.Drawing.Point(16, 270);
            this.pnlSummary.Name = "pnlSummary";
            this.pnlSummary.Padding = new System.Windows.Forms.Padding(16);
            this.pnlSummary.Size = new System.Drawing.Size(338, 120);
            this.pnlSummary.TabIndex = 1;
            // 
            // lblTongTien
            // 
            this.lblTongTien.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTongTien.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTongTien.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(161)))), ((int)(((byte)(55)))));
            this.lblTongTien.Location = new System.Drawing.Point(16, 46);
            this.lblTongTien.Name = "lblTongTien";
            this.lblTongTien.Size = new System.Drawing.Size(304, 56);
            this.lblTongTien.TabIndex = 1;
            this.lblTongTien.Text = "0 ₫";
            this.lblTongTien.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblLabelTong
            // 
            this.lblLabelTong.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblLabelTong.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblLabelTong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblLabelTong.Location = new System.Drawing.Point(16, 16);
            this.lblLabelTong.Name = "lblLabelTong";
            this.lblLabelTong.Size = new System.Drawing.Size(304, 30);
            this.lblLabelTong.TabIndex = 0;
            this.lblLabelTong.Text = "KHÁCH CẦN TRẢ:";
            // 
            // grpCustomer
            // 
            this.grpCustomer.Controls.Add(this.txtDienThoai);
            this.grpCustomer.Controls.Add(this.lblDienThoai);
            this.grpCustomer.Controls.Add(this.txtTenKhach);
            this.grpCustomer.Controls.Add(this.lblTenKhach);
            this.grpCustomer.Controls.Add(this.dtpNgayBan);
            this.grpCustomer.Controls.Add(this.lblNgayBan);
            this.grpCustomer.Controls.Add(this.txtNhanVien);
            this.grpCustomer.Controls.Add(this.lblNhanVien);
            this.grpCustomer.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpCustomer.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.grpCustomer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            this.grpCustomer.Location = new System.Drawing.Point(16, 16);
            this.grpCustomer.Name = "grpCustomer";
            this.grpCustomer.Padding = new System.Windows.Forms.Padding(12);
            this.grpCustomer.Size = new System.Drawing.Size(338, 240);
            this.grpCustomer.TabIndex = 0;
            this.grpCustomer.TabStop = false;
            this.grpCustomer.Text = "Khách Hàng & Thu Ngân";
            // 
            // txtDienThoai
            // 
            this.txtDienThoai.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtDienThoai.Location = new System.Drawing.Point(120, 92);
            this.txtDienThoai.Name = "txtDienThoai";
            this.txtDienThoai.PlaceholderText = "0900000000";
            this.txtDienThoai.Size = new System.Drawing.Size(200, 25);
            this.txtDienThoai.TabIndex = 7;
            // 
            // lblDienThoai
            // 
            this.lblDienThoai.AutoSize = true;
            this.lblDienThoai.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblDienThoai.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblDienThoai.Location = new System.Drawing.Point(14, 96);
            this.lblDienThoai.Name = "lblDienThoai";
            this.lblDienThoai.Size = new System.Drawing.Size(89, 17);
            this.lblDienThoai.TabIndex = 6;
            this.lblDienThoai.Text = "Số điện thoại:";
            // 
            // txtTenKhach
            // 
            this.txtTenKhach.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtTenKhach.Location = new System.Drawing.Point(120, 48);
            this.txtTenKhach.Name = "txtTenKhach";
            this.txtTenKhach.PlaceholderText = "Khách mua tại quầy";
            this.txtTenKhach.Size = new System.Drawing.Size(200, 25);
            this.txtTenKhach.TabIndex = 5;
            // 
            // lblTenKhach
            // 
            this.lblTenKhach.AutoSize = true;
            this.lblTenKhach.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblTenKhach.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblTenKhach.Location = new System.Drawing.Point(14, 52);
            this.lblTenKhach.Name = "lblTenKhach";
            this.lblTenKhach.Size = new System.Drawing.Size(70, 17);
            this.lblTenKhach.TabIndex = 4;
            this.lblTenKhach.Text = "Tên khách:";
            // 
            // dtpNgayBan
            // 
            this.dtpNgayBan.CustomFormat = "dd/MM/yyyy HH:mm";
            this.dtpNgayBan.Enabled = false;
            this.dtpNgayBan.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.dtpNgayBan.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpNgayBan.Location = new System.Drawing.Point(120, 180);
            this.dtpNgayBan.Name = "dtpNgayBan";
            this.dtpNgayBan.Size = new System.Drawing.Size(200, 24);
            this.dtpNgayBan.TabIndex = 3;
            // 
            // lblNgayBan
            // 
            this.lblNgayBan.AutoSize = true;
            this.lblNgayBan.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblNgayBan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblNgayBan.Location = new System.Drawing.Point(14, 184);
            this.lblNgayBan.Name = "lblNgayBan";
            this.lblNgayBan.Size = new System.Drawing.Size(68, 17);
            this.lblNgayBan.TabIndex = 2;
            this.lblNgayBan.Text = "Thời điểm:";
            // 
            // txtNhanVien
            // 
            this.txtNhanVien.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.txtNhanVien.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtNhanVien.Location = new System.Drawing.Point(120, 136);
            this.txtNhanVien.Name = "txtNhanVien";
            this.txtNhanVien.ReadOnly = true;
            this.txtNhanVien.Size = new System.Drawing.Size(200, 25);
            this.txtNhanVien.TabIndex = 1;
            // 
            // lblNhanVien
            // 
            this.lblNhanVien.AutoSize = true;
            this.lblNhanVien.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblNhanVien.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblNhanVien.Location = new System.Drawing.Point(14, 140);
            this.lblNhanVien.Name = "lblNhanVien";
            this.lblNhanVien.Size = new System.Drawing.Size(69, 17);
            this.lblNhanVien.TabIndex = 0;
            this.lblNhanVien.Text = "Thu ngân:";
            // 
            // frmHoaDonBan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.ClientSize = new System.Drawing.Size(1244, 720);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.MinimumSize = new System.Drawing.Size(1024, 600);
            this.Name = "frmHoaDonBan";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "KiotViet Thu Ngân • Lập Hóa Đơn Bán Hàng Tại Quầy (POS)";
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.pnlLeft.ResumeLayout(false);
            this.pnlGridContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiTiet)).EndInit();
            this.pnlProductPicker.ResumeLayout(false);
            this.pnlProductPicker.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numKhuyenMai)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSoLuong)).EndInit();
            this.pnlRight.ResumeLayout(false);
            this.pnlPaymentActions.ResumeLayout(false);
            this.pnlSummary.ResumeLayout(false);
            this.grpCustomer.ResumeLayout(false);
            this.grpCustomer.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel pnlHeaderAccent;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSub;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Panel pnlProductPicker;
        private System.Windows.Forms.Label lblChonSp;
        private System.Windows.Forms.ComboBox cboSanPham;
        private System.Windows.Forms.Label lblDonGia;
        private System.Windows.Forms.TextBox txtDonGia;
        private System.Windows.Forms.Label lblSoLuong;
        private System.Windows.Forms.NumericUpDown numSoLuong;
        private System.Windows.Forms.Label lblKhuyenMai;
        private System.Windows.Forms.NumericUpDown numKhuyenMai;
        private System.Windows.Forms.Label lblTonKho;
        private System.Windows.Forms.Button btnThemMon;
        private System.Windows.Forms.Button btnXoaMon;
        private System.Windows.Forms.Panel pnlGridContainer;
        private System.Windows.Forms.DataGridView dgvChiTiet;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.GroupBox grpCustomer;
        private System.Windows.Forms.Label lblNhanVien;
        private System.Windows.Forms.TextBox txtNhanVien;
        private System.Windows.Forms.Label lblNgayBan;
        private System.Windows.Forms.DateTimePicker dtpNgayBan;
        private System.Windows.Forms.Label lblTenKhach;
        private System.Windows.Forms.TextBox txtTenKhach;
        private System.Windows.Forms.Label lblDienThoai;
        private System.Windows.Forms.TextBox txtDienThoai;
        private System.Windows.Forms.Panel pnlSummary;
        private System.Windows.Forms.Label lblLabelTong;
        private System.Windows.Forms.Label lblTongTien;
        private System.Windows.Forms.Panel pnlPaymentActions;
        private System.Windows.Forms.Button btnThanhToanTienMat;
        private System.Windows.Forms.Button btnThanhToanQR;
    }
}
