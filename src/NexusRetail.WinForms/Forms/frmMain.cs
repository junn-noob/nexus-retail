using NexusRetail.WinForms.Services;
using NexusRetail.WinForms.UI;

namespace NexusRetail.WinForms.Forms
{
    public partial class frmMain : Form
    {
        private readonly string _username;
        private readonly string _role;
        private readonly System.Windows.Forms.Timer _clockTimer;
        public bool IsLogoutRequested { get; private set; }

        public frmMain(string username, string role)
        {
            _username = username;
            _role = role;

            InitializeComponent();

            // Áp dụng bộ theme hiện đại
            UiTheme.BeautifyForm(this);
            ApplyCustomStyles();

            lblUserDisplay.Text = $"👤 Người dùng: {_username}  |  🛡️ Vai trò: {_role}  |  🟢 POS Online";
            
            // Phân quyền: Nếu là Nhân viên (Staff) thì khóa tính năng Báo cáo doanh thu tài chính
            if (_role != "Admin")
            {
                btnBaoCao.Enabled = false;
                btnBaoCao.Text = "🔒 BÁO CÁO THỐNG KÊ (Chỉ dành cho Quản trị viên)";
                btnBaoCao.BackColor = Color.FromArgb(241, 245, 249);
                btnBaoCao.ForeColor = Color.FromArgb(148, 163, 184);
            }

            // Đồng hồ thời gian thực
            _clockTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _clockTimer.Tick += (s, e) =>
            {
                lblSub.Text = $"Phần mềm Bán Lẻ & Quản Lý Kho Thiết Bị Di Động  •  🕒 {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            };
            _clockTimer.Start();
        }

        private void ApplyCustomStyles()
        {
            pnlTop.BackColor = UiTheme.DarkSlate;
            lblBrand.ForeColor = Color.White;
            lblUserDisplay.ForeColor = UiTheme.PrimaryLight;

            // KiotViet Ribbon Navigation Styles
            UiTheme.ApplyPrimaryButton(btnBanHang, "⚡ BÁN HÀNG POS (F9)");
            UiTheme.ApplySecondaryButton(btnSanPham, "📱 Quản Lý Sản Phẩm");
            UiTheme.ApplySecondaryButton(btnNhapHang, "📦 Hóa Đơn Nhập Kho");
            UiTheme.ApplySecondaryButton(btnTraCuu, "🔍 Tra Cứu Hóa Đơn & Sổ");
            UiTheme.ApplySecondaryButton(btnBaoCao, "📊 Báo Cáo Doanh Thu");
            UiTheme.ApplyDangerButton(btnDangXuat, "🚪 Đăng xuất");

            // Beautify Card borders
            pnlCard1.BackColor = Color.White;
            pnlCard2.BackColor = Color.White;
            pnlCard3.BackColor = Color.White;
            pnlCard4.BackColor = Color.White;
            pnlBanner.BackColor = Color.White;

            LoadDashboardStatsAsync();
        }

        private async void LoadDashboardStatsAsync()
        {
            try
            {
                var products = await WinFormsApiClient.Instance.GetProductsAsync();
                lblCard3Val.Text = $"{products.Count} Sản Phẩm";
                int lowStock = products.Count(p => p.SoLuong <= 5);
                lblCard4Val.Text = lowStock > 0 ? $"⚠️ {lowStock} SP sắp hết" : "Tồn kho: Ổn định";
                lblCard4Val.ForeColor = lowStock > 0 ? Color.FromArgb(220, 38, 38) : Color.FromArgb(16, 185, 129);

                var invoices = await WinFormsApiClient.Instance.SearchSaleInvoicesAsync();
                lblCard2Val.Text = $"{invoices.Count} Hóa Đơn";
                decimal totalRev = invoices.Sum(i => i.TongTien);
                lblCard1Val.Text = $"{totalRev:N0} ₫";
            }
            catch { }
        }

        private void btnBanHang_Click(object sender, EventArgs e)
        {
            var frm = new frmHoaDonBan(_username);
            frm.ShowDialog(this);
            LoadDashboardStatsAsync();
        }

        private void btnSanPham_Click(object sender, EventArgs e)
        {
            var frm = new frmSanPham(_role);
            frm.ShowDialog(this);
            LoadDashboardStatsAsync();
        }

        private void btnNhapHang_Click(object sender, EventArgs e)
        {
            var frm = new frmHoaDonNhap(_username);
            frm.ShowDialog(this);
            LoadDashboardStatsAsync();
        }

        private void btnBaoCao_Click(object sender, EventArgs e)
        {
            if (_role != "Admin")
            {
                MessageBox.Show("Chức năng xem Báo cáo doanh thu chỉ dành cho Quản trị viên (Admin)!", "Phân quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var frm = new frmBaoCaoThongKe();
            frm.ShowDialog(this);
        }

        private void btnTraCuu_Click(object sender, EventArgs e)
        {
            var frm = new frmTraCuuHoaDon();
            frm.ShowDialog(this);
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất khỏi hệ thống?", "Xác nhận đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                IsLogoutRequested = true;
                NexusRetail.WinForms.Services.WinFormsApiClient.Instance.Logout();
                Close();
            }
        }
    }
}
