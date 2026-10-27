namespace NexusRetail.WinForms.Forms
{
    partial class frmMain
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
            this.btnDangXuat = new System.Windows.Forms.Button();
            this.lblUserDisplay = new System.Windows.Forms.Label();
            this.lblSub = new System.Windows.Forms.Label();
            this.lblBrand = new System.Windows.Forms.Label();
            this.pnlNavRibbon = new System.Windows.Forms.Panel();
            this.btnBanHang = new System.Windows.Forms.Button();
            this.btnSanPham = new System.Windows.Forms.Button();
            this.btnNhapHang = new System.Windows.Forms.Button();
            this.btnTraCuu = new System.Windows.Forms.Button();
            this.btnBaoCao = new System.Windows.Forms.Button();
            this.pnlCenter = new System.Windows.Forms.Panel();
            this.pnlCardsContainer = new System.Windows.Forms.TableLayoutPanel();
            this.pnlCard1 = new System.Windows.Forms.Panel();
            this.lblCard1Val = new System.Windows.Forms.Label();
            this.lblCard1Title = new System.Windows.Forms.Label();
            this.pnlCard2 = new System.Windows.Forms.Panel();
            this.lblCard2Val = new System.Windows.Forms.Label();
            this.lblCard2Title = new System.Windows.Forms.Label();
            this.pnlCard3 = new System.Windows.Forms.Panel();
            this.lblCard3Val = new System.Windows.Forms.Label();
            this.lblCard3Title = new System.Windows.Forms.Label();
            this.pnlCard4 = new System.Windows.Forms.Panel();
            this.lblCard4Val = new System.Windows.Forms.Label();
            this.lblCard4Title = new System.Windows.Forms.Label();
            this.pnlRecentActivity = new System.Windows.Forms.Panel();
            this.pnlRecentHeader = new System.Windows.Forms.Panel();
            this.lblRecentTitle = new System.Windows.Forms.Label();
            this.lblRecentSub = new System.Windows.Forms.Label();
            this.dgvRecentSales = new System.Windows.Forms.DataGridView();
            this.colHdb = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNgay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colKhach = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSdt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colThuNgan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTongTien = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTrangThai = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlTop.SuspendLayout();
            this.pnlNavRibbon.SuspendLayout();
            this.pnlCenter.SuspendLayout();
            this.pnlCardsContainer.SuspendLayout();
            this.pnlCard1.SuspendLayout();
            this.pnlCard2.SuspendLayout();
            this.pnlCard3.SuspendLayout();
            this.pnlCard4.SuspendLayout();
            this.pnlRecentActivity.SuspendLayout();
            this.pnlRecentHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecentSales)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            this.pnlTop.Controls.Add(this.pnlHeaderAccent);
            this.pnlTop.Controls.Add(this.btnDangXuat);
            this.pnlTop.Controls.Add(this.lblUserDisplay);
            this.pnlTop.Controls.Add(this.lblSub);
            this.pnlTop.Controls.Add(this.lblBrand);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1264, 76);
            this.pnlTop.TabIndex = 0;
            // 
            // pnlHeaderAccent
            // 
            this.pnlHeaderAccent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(161)))), ((int)(((byte)(55)))));
            this.pnlHeaderAccent.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlHeaderAccent.Location = new System.Drawing.Point(0, 72);
            this.pnlHeaderAccent.Name = "pnlHeaderAccent";
            this.pnlHeaderAccent.Size = new System.Drawing.Size(1264, 4);
            this.pnlHeaderAccent.TabIndex = 4;
            // 
            // btnDangXuat
            // 
            this.btnDangXuat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDangXuat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnDangXuat.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDangXuat.FlatAppearance.BorderSize = 0;
            this.btnDangXuat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDangXuat.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnDangXuat.ForeColor = System.Drawing.Color.White;
            this.btnDangXuat.Location = new System.Drawing.Point(1124, 18);
            this.btnDangXuat.Name = "btnDangXuat";
            this.btnDangXuat.Size = new System.Drawing.Size(120, 36);
            this.btnDangXuat.TabIndex = 3;
            this.btnDangXuat.Text = "🚪 Đăng xuất";
            this.btnDangXuat.UseVisualStyleBackColor = false;
            this.btnDangXuat.Click += new System.EventHandler(this.btnDangXuat_Click);
            // 
            // lblUserDisplay
            // 
            this.lblUserDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUserDisplay.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblUserDisplay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(231)))), ((int)(((byte)(255)))));
            this.lblUserDisplay.Location = new System.Drawing.Point(620, 18);
            this.lblUserDisplay.Name = "lblUserDisplay";
            this.lblUserDisplay.Size = new System.Drawing.Size(488, 36);
            this.lblUserDisplay.TabIndex = 2;
            this.lblUserDisplay.Text = "👤 Người dùng: admin  |  🛡️ Vai trò: Admin  |  🟢 POS Online";
            this.lblUserDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSub
            // 
            this.lblSub.AutoSize = true;
            this.lblSub.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblSub.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblSub.Location = new System.Drawing.Point(22, 45);
            this.lblSub.Name = "lblSub";
            this.lblSub.Size = new System.Drawing.Size(280, 15);
            this.lblSub.TabIndex = 1;
            this.lblSub.Text = "Hệ thống Quản lý Bán lẻ & Kho Hàng Chuẩn KiotViet";
            // 
            // lblBrand
            // 
            this.lblBrand.AutoSize = true;
            this.lblBrand.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblBrand.ForeColor = System.Drawing.Color.White;
            this.lblBrand.Location = new System.Drawing.Point(20, 12);
            this.lblBrand.Name = "lblBrand";
            this.lblBrand.Size = new System.Drawing.Size(325, 30);
            this.lblBrand.TabIndex = 0;
            this.lblBrand.Text = "KIOTVIET • DUY MOBILE POS";
            // 
            // pnlNavRibbon
            // 
            this.pnlNavRibbon.BackColor = System.Drawing.Color.White;
            this.pnlNavRibbon.Controls.Add(this.btnBanHang);
            this.pnlNavRibbon.Controls.Add(this.btnSanPham);
            this.pnlNavRibbon.Controls.Add(this.btnNhapHang);
            this.pnlNavRibbon.Controls.Add(this.btnTraCuu);
            this.pnlNavRibbon.Controls.Add(this.btnBaoCao);
            this.pnlNavRibbon.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlNavRibbon.Location = new System.Drawing.Point(0, 76);
            this.pnlNavRibbon.Name = "pnlNavRibbon";
            this.pnlNavRibbon.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            this.pnlNavRibbon.Size = new System.Drawing.Size(1264, 58);
            this.pnlNavRibbon.TabIndex = 1;
            // 
            // btnBanHang
            // 
            this.btnBanHang.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(161)))), ((int)(((byte)(55)))));
            this.btnBanHang.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBanHang.FlatAppearance.BorderSize = 0;
            this.btnBanHang.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBanHang.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnBanHang.ForeColor = System.Drawing.Color.White;
            this.btnBanHang.Location = new System.Drawing.Point(16, 8);
            this.btnBanHang.Name = "btnBanHang";
            this.btnBanHang.Size = new System.Drawing.Size(220, 42);
            this.btnBanHang.TabIndex = 0;
            this.btnBanHang.Text = "⚡ BÁN HÀNG POS (F9)";
            this.btnBanHang.UseVisualStyleBackColor = false;
            this.btnBanHang.Click += new System.EventHandler(this.btnBanHang_Click);
            // 
            // btnSanPham
            // 
            this.btnSanPham.BackColor = System.Drawing.Color.White;
            this.btnSanPham.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSanPham.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnSanPham.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSanPham.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSanPham.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btnSanPham.Location = new System.Drawing.Point(244, 8);
            this.btnSanPham.Name = "btnSanPham";
            this.btnSanPham.Size = new System.Drawing.Size(190, 42);
            this.btnSanPham.TabIndex = 1;
            this.btnSanPham.Text = "📱 Quản Lý Sản Phẩm";
            this.btnSanPham.UseVisualStyleBackColor = false;
            this.btnSanPham.Click += new System.EventHandler(this.btnSanPham_Click);
            // 
            // btnNhapHang
            // 
            this.btnNhapHang.BackColor = System.Drawing.Color.White;
            this.btnNhapHang.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNhapHang.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnNhapHang.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNhapHang.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnNhapHang.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btnNhapHang.Location = new System.Drawing.Point(442, 8);
            this.btnNhapHang.Name = "btnNhapHang";
            this.btnNhapHang.Size = new System.Drawing.Size(190, 42);
            this.btnNhapHang.TabIndex = 2;
            this.btnNhapHang.Text = "📦 Hóa Đơn Nhập Kho";
            this.btnNhapHang.UseVisualStyleBackColor = false;
            this.btnNhapHang.Click += new System.EventHandler(this.btnNhapHang_Click);
            // 
            // btnTraCuu
            // 
            this.btnTraCuu.BackColor = System.Drawing.Color.White;
            this.btnTraCuu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTraCuu.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnTraCuu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTraCuu.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnTraCuu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btnTraCuu.Location = new System.Drawing.Point(640, 8);
            this.btnTraCuu.Name = "btnTraCuu";
            this.btnTraCuu.Size = new System.Drawing.Size(210, 42);
            this.btnTraCuu.TabIndex = 3;
            this.btnTraCuu.Text = "🔍 Tra Cứu Hóa Đơn & Sổ";
            this.btnTraCuu.UseVisualStyleBackColor = false;
            this.btnTraCuu.Click += new System.EventHandler(this.btnTraCuu_Click);
            // 
            // btnBaoCao
            // 
            this.btnBaoCao.BackColor = System.Drawing.Color.White;
            this.btnBaoCao.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBaoCao.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnBaoCao.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBaoCao.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnBaoCao.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btnBaoCao.Location = new System.Drawing.Point(858, 8);
            this.btnBaoCao.Name = "btnBaoCao";
            this.btnBaoCao.Size = new System.Drawing.Size(220, 42);
            this.btnBaoCao.TabIndex = 4;
            this.btnBaoCao.Text = "📊 Báo Cáo Doanh Thu (Admin)";
            this.btnBaoCao.UseVisualStyleBackColor = false;
            this.btnBaoCao.Click += new System.EventHandler(this.btnBaoCao_Click);
            // 
            // pnlCenter
            // 
            this.pnlCenter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlCenter.Controls.Add(this.pnlRecentActivity);
            this.pnlCenter.Controls.Add(this.pnlCardsContainer);
            this.pnlCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCenter.Location = new System.Drawing.Point(0, 134);
            this.pnlCenter.Name = "pnlCenter";
            this.pnlCenter.Padding = new System.Windows.Forms.Padding(24);
            this.pnlCenter.Size = new System.Drawing.Size(1264, 616);
            this.pnlCenter.TabIndex = 2;
            // 
            // pnlCardsContainer
            // 
            this.pnlCardsContainer.ColumnCount = 4;
            this.pnlCardsContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnlCardsContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnlCardsContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnlCardsContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnlCardsContainer.Controls.Add(this.pnlCard1, 0, 0);
            this.pnlCardsContainer.Controls.Add(this.pnlCard2, 1, 0);
            this.pnlCardsContainer.Controls.Add(this.pnlCard3, 2, 0);
            this.pnlCardsContainer.Controls.Add(this.pnlCard4, 3, 0);
            this.pnlCardsContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCardsContainer.Location = new System.Drawing.Point(24, 24);
            this.pnlCardsContainer.Name = "pnlCardsContainer";
            this.pnlCardsContainer.RowCount = 1;
            this.pnlCardsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlCardsContainer.Size = new System.Drawing.Size(1216, 120);
            this.pnlCardsContainer.TabIndex = 0;
            // 
            // pnlCard1
            // 
            this.pnlCard1.BackColor = System.Drawing.Color.White;
            this.pnlCard1.Controls.Add(this.lblCard1Val);
            this.pnlCard1.Controls.Add(this.lblCard1Title);
            this.pnlCard1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCard1.Location = new System.Drawing.Point(3, 3);
            this.pnlCard1.Name = "pnlCard1";
            this.pnlCard1.Padding = new System.Windows.Forms.Padding(16);
            this.pnlCard1.Size = new System.Drawing.Size(298, 114);
            this.pnlCard1.TabIndex = 0;
            // 
            // lblCard1Val
            // 
            this.lblCard1Val.AutoSize = true;
            this.lblCard1Val.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCard1Val.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(161)))), ((int)(((byte)(55)))));
            this.lblCard1Val.Location = new System.Drawing.Point(14, 48);
            this.lblCard1Val.Name = "lblCard1Val";
            this.lblCard1Val.Size = new System.Drawing.Size(176, 30);
            this.lblCard1Val.TabIndex = 1;
            this.lblCard1Val.Text = "128,500,000 ₫";
            // 
            // lblCard1Title
            // 
            this.lblCard1Title.AutoSize = true;
            this.lblCard1Title.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCard1Title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblCard1Title.Location = new System.Drawing.Point(14, 16);
            this.lblCard1Title.Name = "lblCard1Title";
            this.lblCard1Title.Size = new System.Drawing.Size(161, 17);
            this.lblCard1Title.TabIndex = 0;
            this.lblCard1Title.Text = "📈 DOANH THU THÁNG";
            // 
            // pnlCard2
            // 
            this.pnlCard2.BackColor = System.Drawing.Color.White;
            this.pnlCard2.Controls.Add(this.lblCard2Val);
            this.pnlCard2.Controls.Add(this.lblCard2Title);
            this.pnlCard2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCard2.Location = new System.Drawing.Point(307, 3);
            this.pnlCard2.Name = "pnlCard2";
            this.pnlCard2.Padding = new System.Windows.Forms.Padding(16);
            this.pnlCard2.Size = new System.Drawing.Size(298, 114);
            this.pnlCard2.TabIndex = 1;
            // 
            // lblCard2Val
            // 
            this.lblCard2Val.AutoSize = true;
            this.lblCard2Val.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCard2Val.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(110)))), ((int)(((byte)(253)))));
            this.lblCard2Val.Location = new System.Drawing.Point(14, 48);
            this.lblCard2Val.Name = "lblCard2Val";
            this.lblCard2Val.Size = new System.Drawing.Size(126, 30);
            this.lblCard2Val.TabIndex = 1;
            this.lblCard2Val.Text = "12 Hóa Đơn";
            // 
            // lblCard2Title
            // 
            this.lblCard2Title.AutoSize = true;
            this.lblCard2Title.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCard2Title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblCard2Title.Location = new System.Drawing.Point(14, 16);
            this.lblCard2Title.Name = "lblCard2Title";
            this.lblCard2Title.Size = new System.Drawing.Size(155, 17);
            this.lblCard2Title.TabIndex = 0;
            this.lblCard2Title.Text = "🧾 GIAO DỊCH BÁN LẺ";
            // 
            // pnlCard3
            // 
            this.pnlCard3.BackColor = System.Drawing.Color.White;
            this.pnlCard3.Controls.Add(this.lblCard3Val);
            this.pnlCard3.Controls.Add(this.lblCard3Title);
            this.pnlCard3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCard3.Location = new System.Drawing.Point(611, 3);
            this.pnlCard3.Name = "pnlCard3";
            this.pnlCard3.Padding = new System.Windows.Forms.Padding(16);
            this.pnlCard3.Size = new System.Drawing.Size(298, 114);
            this.pnlCard3.TabIndex = 2;
            // 
            // lblCard3Val
            // 
            this.lblCard3Val.AutoSize = true;
            this.lblCard3Val.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCard3Val.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(115)))), ((int)(((byte)(22)))));
            this.lblCard3Val.Location = new System.Drawing.Point(14, 48);
            this.lblCard3Val.Name = "lblCard3Val";
            this.lblCard3Val.Size = new System.Drawing.Size(148, 30);
            this.lblCard3Val.TabIndex = 1;
            this.lblCard3Val.Text = "24 Sản Phẩm";
            // 
            // lblCard3Title
            // 
            this.lblCard3Title.AutoSize = true;
            this.lblCard3Title.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCard3Title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblCard3Title.Location = new System.Drawing.Point(14, 16);
            this.lblCard3Title.Name = "lblCard3Title";
            this.lblCard3Title.Size = new System.Drawing.Size(176, 17);
            this.lblCard3Title.TabIndex = 0;
            this.lblCard3Title.Text = "📱 DANH MỤC KINH DOANH";
            // 
            // pnlCard4
            // 
            this.pnlCard4.BackColor = System.Drawing.Color.White;
            this.pnlCard4.Controls.Add(this.lblCard4Val);
            this.pnlCard4.Controls.Add(this.lblCard4Title);
            this.pnlCard4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCard4.Location = new System.Drawing.Point(915, 3);
            this.pnlCard4.Name = "pnlCard4";
            this.pnlCard4.Padding = new System.Windows.Forms.Padding(16);
            this.pnlCard4.Size = new System.Drawing.Size(298, 114);
            this.pnlCard4.TabIndex = 3;
            // 
            // lblCard4Val
            // 
            this.lblCard4Val.AutoSize = true;
            this.lblCard4Val.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCard4Val.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.lblCard4Val.Location = new System.Drawing.Point(14, 48);
            this.lblCard4Val.Name = "lblCard4Val";
            this.lblCard4Val.Size = new System.Drawing.Size(155, 30);
            this.lblCard4Val.TabIndex = 1;
            this.lblCard4Val.Text = "Tồn kho: Ổn định";
            // 
            // lblCard4Title
            // 
            this.lblCard4Title.AutoSize = true;
            this.lblCard4Title.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCard4Title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblCard4Title.Location = new System.Drawing.Point(14, 16);
            this.lblCard4Title.Name = "lblCard4Title";
            this.lblCard4Title.Size = new System.Drawing.Size(158, 17);
            this.lblCard4Title.TabIndex = 0;
            this.lblCard4Title.Text = "🛡️ TRẠNG THÁI KHO";
            // 
            // 
            // pnlRecentActivity
            // 
            this.pnlRecentActivity.BackColor = System.Drawing.Color.White;
            this.pnlRecentActivity.Controls.Add(this.dgvRecentSales);
            this.pnlRecentActivity.Controls.Add(this.pnlRecentHeader);
            this.pnlRecentActivity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRecentActivity.Location = new System.Drawing.Point(24, 144);
            this.pnlRecentActivity.Name = "pnlRecentActivity";
            this.pnlRecentActivity.Padding = new System.Windows.Forms.Padding(16, 12, 16, 12);
            this.pnlRecentActivity.Size = new System.Drawing.Size(1216, 448);
            this.pnlRecentActivity.TabIndex = 1;
            // 
            // pnlRecentHeader
            // 
            this.pnlRecentHeader.Controls.Add(this.lblRecentSub);
            this.pnlRecentHeader.Controls.Add(this.lblRecentTitle);
            this.pnlRecentHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlRecentHeader.Location = new System.Drawing.Point(16, 12);
            this.pnlRecentHeader.Name = "pnlRecentHeader";
            this.pnlRecentHeader.Size = new System.Drawing.Size(1184, 40);
            this.pnlRecentHeader.TabIndex = 0;
            // 
            // lblRecentTitle
            // 
            this.lblRecentTitle.AutoSize = true;
            this.lblRecentTitle.Font = new System.Drawing.Font("Segoe UI", 11.5F, System.Drawing.FontStyle.Bold);
            this.lblRecentTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(34)))), ((int)(((byte)(64)))));
            this.lblRecentTitle.Location = new System.Drawing.Point(0, 8);
            this.lblRecentTitle.Name = "lblRecentTitle";
            this.lblRecentTitle.Size = new System.Drawing.Size(320, 21);
            this.lblRecentTitle.TabIndex = 0;
            this.lblRecentTitle.Text = "📋 HOẠT ĐỘNG GIAO DỊCH BÁN HÀNG GẦN ĐÂY";
            // 
            // lblRecentSub
            // 
            this.lblRecentSub.AutoSize = true;
            this.lblRecentSub.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblRecentSub.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblRecentSub.Location = new System.Drawing.Point(340, 11);
            this.lblRecentSub.Name = "lblRecentSub";
            this.lblRecentSub.Size = new System.Drawing.Size(350, 17);
            this.lblRecentSub.TabIndex = 1;
            this.lblRecentSub.Text = "Theo dõi đơn hàng thu ngân phát sinh thời gian thực tại quầy";
            // 
            // dgvRecentSales
            // 
            this.dgvRecentSales.AllowUserToAddRows = false;
            this.dgvRecentSales.AllowUserToDeleteRows = false;
            this.dgvRecentSales.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRecentSales.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.dgvRecentSales.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvRecentSales.ColumnHeadersHeight = 36;
            this.dgvRecentSales.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colHdb,
            this.colNgay,
            this.colKhach,
            this.colSdt,
            this.colThuNgan,
            this.colTongTien,
            this.colTrangThai});
            this.dgvRecentSales.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRecentSales.EnableHeadersVisualStyles = false;
            this.dgvRecentSales.Location = new System.Drawing.Point(16, 52);
            this.dgvRecentSales.MultiSelect = false;
            this.dgvRecentSales.Name = "dgvRecentSales";
            this.dgvRecentSales.ReadOnly = true;
            this.dgvRecentSales.RowHeadersVisible = false;
            this.dgvRecentSales.RowTemplate.Height = 36;
            this.dgvRecentSales.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRecentSales.Size = new System.Drawing.Size(1184, 384);
            this.dgvRecentSales.TabIndex = 1;
            // 
            // colHdb
            // 
            this.colHdb.FillWeight = 85F;
            this.colHdb.HeaderText = "Mã Hóa Đơn";
            this.colHdb.Name = "colHdb";
            this.colHdb.ReadOnly = true;
            // 
            // colNgay
            // 
            this.colNgay.FillWeight = 110F;
            this.colNgay.HeaderText = "Thời Gian Bán";
            this.colNgay.Name = "colNgay";
            this.colNgay.ReadOnly = true;
            // 
            // colKhach
            // 
            this.colKhach.FillWeight = 140F;
            this.colKhach.HeaderText = "Khách Hàng";
            this.colKhach.Name = "colKhach";
            this.colKhach.ReadOnly = true;
            // 
            // colSdt
            // 
            this.colSdt.FillWeight = 95F;
            this.colSdt.HeaderText = "Số Điện Thoại";
            this.colSdt.Name = "colSdt";
            this.colSdt.ReadOnly = true;
            // 
            // colThuNgan
            // 
            this.colThuNgan.FillWeight = 110F;
            this.colThuNgan.HeaderText = "Thu Ngân";
            this.colThuNgan.Name = "colThuNgan";
            this.colThuNgan.ReadOnly = true;
            // 
            // colTongTien
            // 
            this.colTongTien.FillWeight = 120F;
            this.colTongTien.HeaderText = "Tổng Tiền";
            this.colTongTien.Name = "colTongTien";
            this.colTongTien.ReadOnly = true;
            // 
            // colTrangThai
            // 
            this.colTrangThai.FillWeight = 90F;
            this.colTrangThai.HeaderText = "Trạng Thái";
            this.colTrangThai.Name = "colTrangThai";
            this.colTrangThai.ReadOnly = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.ClientSize = new System.Drawing.Size(1264, 750);
            this.Controls.Add(this.pnlCenter);
            this.Controls.Add(this.pnlNavRibbon);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.MinimumSize = new System.Drawing.Size(1100, 680);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "KiotViet • Phần Mềm Quản Lý Bán Lẻ & POS Thu Ngân - Duy Mobile";
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlNavRibbon.ResumeLayout(false);
            this.pnlCenter.ResumeLayout(false);
            this.pnlCardsContainer.ResumeLayout(false);
            this.pnlCard1.ResumeLayout(false);
            this.pnlCard1.PerformLayout();
            this.pnlCard2.ResumeLayout(false);
            this.pnlCard2.PerformLayout();
            this.pnlCard3.ResumeLayout(false);
            this.pnlCard3.PerformLayout();
            this.pnlCard4.ResumeLayout(false);
            this.pnlCard4.PerformLayout();
            this.pnlRecentActivity.ResumeLayout(false);
            this.pnlRecentHeader.ResumeLayout(false);
            this.pnlRecentHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecentSales)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel pnlHeaderAccent;
        private System.Windows.Forms.Label lblBrand;
        private System.Windows.Forms.Label lblSub;
        private System.Windows.Forms.Label lblUserDisplay;
        private System.Windows.Forms.Button btnDangXuat;
        private System.Windows.Forms.Panel pnlNavRibbon;
        private System.Windows.Forms.Button btnBanHang;
        private System.Windows.Forms.Button btnSanPham;
        private System.Windows.Forms.Button btnNhapHang;
        private System.Windows.Forms.Button btnTraCuu;
        private System.Windows.Forms.Button btnBaoCao;
        private System.Windows.Forms.Panel pnlCenter;
        private System.Windows.Forms.TableLayoutPanel pnlCardsContainer;
        private System.Windows.Forms.Panel pnlCard1;
        private System.Windows.Forms.Label lblCard1Title;
        private System.Windows.Forms.Label lblCard1Val;
        private System.Windows.Forms.Panel pnlCard2;
        private System.Windows.Forms.Label lblCard2Title;
        private System.Windows.Forms.Label lblCard2Val;
        private System.Windows.Forms.Panel pnlCard3;
        private System.Windows.Forms.Label lblCard3Title;
        private System.Windows.Forms.Label lblCard3Val;
        private System.Windows.Forms.Panel pnlCard4;
        private System.Windows.Forms.Label lblCard4Title;
        private System.Windows.Forms.Label lblCard4Val;
        private System.Windows.Forms.Panel pnlRecentActivity;
        private System.Windows.Forms.Panel pnlRecentHeader;
        private System.Windows.Forms.Label lblRecentTitle;
        private System.Windows.Forms.Label lblRecentSub;
        private System.Windows.Forms.DataGridView dgvRecentSales;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHdb;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNgay;
        private System.Windows.Forms.DataGridViewTextBoxColumn colKhach;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSdt;
        private System.Windows.Forms.DataGridViewTextBoxColumn colThuNgan;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTongTien;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTrangThai;
    }
}
