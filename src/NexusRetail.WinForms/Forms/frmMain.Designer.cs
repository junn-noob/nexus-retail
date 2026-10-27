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
            pnlTop = new Panel();
            lblUserDisplay = new Label();
            lblSub = new Label();
            lblBrand = new Label();
            pnlCenter = new Panel();
            btnDangXuat = new Button();
            btnTraCuu = new Button();
            btnBaoCao = new Button();
            btnNhapHang = new Button();
            btnSanPham = new Button();
            btnBanHang = new Button();
            pnlTop.SuspendLayout();
            pnlCenter.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTop
            // 
            pnlTop.BackColor = Color.FromArgb(15, 23, 42);
            pnlTop.Controls.Add(lblUserDisplay);
            pnlTop.Controls.Add(lblSub);
            pnlTop.Controls.Add(lblBrand);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Margin = new Padding(4, 5, 4, 5);
            pnlTop.Name = "pnlTop";
            pnlTop.Size = new Size(1229, 150);
            pnlTop.TabIndex = 0;
            // 
            // lblUserDisplay
            // 
            lblUserDisplay.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblUserDisplay.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblUserDisplay.ForeColor = Color.FromArgb(224, 231, 255);
            lblUserDisplay.Location = new Point(671, 50);
            lblUserDisplay.Margin = new Padding(4, 0, 4, 0);
            lblUserDisplay.Name = "lblUserDisplay";
            lblUserDisplay.Size = new Size(514, 42);
            lblUserDisplay.TabIndex = 2;
            lblUserDisplay.Text = "Xin chào: admin | Vai trò: Admin";
            lblUserDisplay.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblSub
            // 
            lblSub.AutoSize = true;
            lblSub.Font = new Font("Segoe UI", 9.5F);
            lblSub.ForeColor = Color.FromArgb(148, 163, 184);
            lblSub.Location = new Point(40, 87);
            lblSub.Margin = new Padding(4, 0, 4, 0);
            lblSub.Name = "lblSub";
            lblSub.Size = new Size(421, 25);
            lblSub.TabIndex = 1;
            lblSub.Text = "Phần mềm Bán Lẻ & Quản Lý Kho Thiết Bị Di Động";
            // 
            // lblBrand
            // 
            lblBrand.AutoSize = true;
            lblBrand.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblBrand.ForeColor = Color.White;
            lblBrand.Location = new Point(34, 27);
            lblBrand.Margin = new Padding(4, 0, 4, 0);
            lblBrand.Name = "lblBrand";
            lblBrand.Size = new Size(549, 48);
            lblBrand.TabIndex = 0;
            lblBrand.Text = "DUY MOBILE • BÁN LẺ THIẾT BỊ";
            // 
            // pnlCenter
            // 
            pnlCenter.BackColor = Color.FromArgb(248, 250, 252);
            pnlCenter.Controls.Add(btnDangXuat);
            pnlCenter.Controls.Add(btnTraCuu);
            pnlCenter.Controls.Add(btnBaoCao);
            pnlCenter.Controls.Add(btnNhapHang);
            pnlCenter.Controls.Add(btnSanPham);
            pnlCenter.Controls.Add(btnBanHang);
            pnlCenter.Dock = DockStyle.Fill;
            pnlCenter.Location = new Point(0, 150);
            pnlCenter.Margin = new Padding(4, 5, 4, 5);
            pnlCenter.Name = "pnlCenter";
            pnlCenter.Size = new Size(1229, 716);
            pnlCenter.TabIndex = 1;
            // 
            // btnDangXuat
            // 
            btnDangXuat.BackColor = Color.FromArgb(254, 242, 242);
            btnDangXuat.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
            btnDangXuat.FlatStyle = FlatStyle.Flat;
            btnDangXuat.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnDangXuat.ForeColor = Color.FromArgb(220, 38, 38);
            btnDangXuat.Location = new Point(71, 608);
            btnDangXuat.Margin = new Padding(4, 5, 4, 5);
            btnDangXuat.Name = "btnDangXuat";
            btnDangXuat.Size = new Size(1086, 70);
            btnDangXuat.TabIndex = 5;
            btnDangXuat.Text = "🚪 ĐĂNG XUẤT HỆ THỐNG";
            btnDangXuat.UseVisualStyleBackColor = false;
            btnDangXuat.Click += btnDangXuat_Click;
            // 
            // btnTraCuu
            // 
            btnTraCuu.BackColor = Color.White;
            btnTraCuu.FlatStyle = FlatStyle.Flat;
            btnTraCuu.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnTraCuu.ForeColor = Color.FromArgb(15, 23, 42);
            btnTraCuu.Location = new Point(71, 500);
            btnTraCuu.Margin = new Padding(4, 5, 4, 5);
            btnTraCuu.Name = "btnTraCuu";
            btnTraCuu.Size = new Size(1086, 87);
            btnTraCuu.TabIndex = 4;
            btnTraCuu.Text = "🔍 TRA CỨU HÓA ĐƠN BÁN & NHẬP KHO";
            btnTraCuu.UseVisualStyleBackColor = false;
            btnTraCuu.Click += btnTraCuu_Click;
            // 
            // btnBaoCao
            // 
            btnBaoCao.BackColor = Color.White;
            btnBaoCao.FlatStyle = FlatStyle.Flat;
            btnBaoCao.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnBaoCao.ForeColor = Color.FromArgb(15, 23, 42);
            btnBaoCao.Location = new Point(643, 292);
            btnBaoCao.Margin = new Padding(4, 5, 4, 5);
            btnBaoCao.Name = "btnBaoCao";
            btnBaoCao.Size = new Size(514, 175);
            btnBaoCao.TabIndex = 3;
            btnBaoCao.Text = "📊 BÁO CÁO THỐNG KÊ\r\nPhân tích doanh thu & Xuất file Excel";
            btnBaoCao.UseVisualStyleBackColor = false;
            btnBaoCao.Click += btnBaoCao_Click;
            // 
            // btnNhapHang
            // 
            btnNhapHang.BackColor = Color.White;
            btnNhapHang.FlatStyle = FlatStyle.Flat;
            btnNhapHang.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnNhapHang.ForeColor = Color.FromArgb(15, 23, 42);
            btnNhapHang.Location = new Point(71, 292);
            btnNhapHang.Margin = new Padding(4, 5, 4, 5);
            btnNhapHang.Name = "btnNhapHang";
            btnNhapHang.Size = new Size(514, 175);
            btnNhapHang.TabIndex = 2;
            btnNhapHang.Text = "📦 HÓA ĐƠN NHẬP KHO\r\nLập phiếu nhập kho từ nhà cung cấp";
            btnNhapHang.UseVisualStyleBackColor = false;
            btnNhapHang.Click += btnNhapHang_Click;
            // 
            // btnSanPham
            // 
            btnSanPham.BackColor = Color.White;
            btnSanPham.FlatStyle = FlatStyle.Flat;
            btnSanPham.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnSanPham.ForeColor = Color.FromArgb(15, 23, 42);
            btnSanPham.Location = new Point(643, 75);
            btnSanPham.Margin = new Padding(4, 5, 4, 5);
            btnSanPham.Name = "btnSanPham";
            btnSanPham.Size = new Size(514, 175);
            btnSanPham.TabIndex = 1;
            btnSanPham.Text = "📱 QUẢN LÝ SẢN PHẨM\r\nDanh mục thiết bị di động & cấu hình";
            btnSanPham.UseVisualStyleBackColor = false;
            btnSanPham.Click += btnSanPham_Click;
            // 
            // btnBanHang
            // 
            btnBanHang.BackColor = Color.FromArgb(15, 23, 42);
            btnBanHang.FlatStyle = FlatStyle.Flat;
            btnBanHang.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnBanHang.ForeColor = Color.White;
            btnBanHang.Location = new Point(71, 75);
            btnBanHang.Margin = new Padding(4, 5, 4, 5);
            btnBanHang.Name = "btnBanHang";
            btnBanHang.Size = new Size(514, 175);
            btnBanHang.TabIndex = 0;
            btnBanHang.Text = "\U0001f6d2 BÁN HÀNG TẠI QUẦY (POS)\r\nLập hóa đơn & Thanh toán VietQR";
            btnBanHang.UseVisualStyleBackColor = false;
            btnBanHang.Click += btnBanHang_Click;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1229, 866);
            Controls.Add(pnlCenter);
            Controls.Add(pnlTop);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            Name = "frmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Hệ Thống Quản Lý Bán Lẻ Điện Thoại Duy Mobile (Desktop POS)";
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            pnlCenter.ResumeLayout(false);
            ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblBrand;
        private System.Windows.Forms.Label lblSub;
        private System.Windows.Forms.Label lblUserDisplay;
        private System.Windows.Forms.Panel pnlCenter;
        private System.Windows.Forms.Button btnBanHang;
        private System.Windows.Forms.Button btnSanPham;
        private System.Windows.Forms.Button btnNhapHang;
        private System.Windows.Forms.Button btnBaoCao;
        private System.Windows.Forms.Button btnTraCuu;
        private System.Windows.Forms.Button btnDangXuat;
    }
}
