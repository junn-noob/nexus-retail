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

            // Bán hàng POS (Primary Action - Nổi bật nhất)
            btnBanHang.FlatStyle = FlatStyle.Flat;
            btnBanHang.FlatAppearance.BorderSize = 0;
            btnBanHang.BackColor = UiTheme.Primary;
            btnBanHang.ForeColor = Color.White;
            btnBanHang.Font = new Font("Segoe UI", 13f, FontStyle.Bold);
            btnBanHang.Cursor = Cursors.Hand;
            btnBanHang.FlatAppearance.MouseOverBackColor = UiTheme.PrimaryDark;

            // Nhập hàng
            btnNhapHang.FlatStyle = FlatStyle.Flat;
            btnNhapHang.FlatAppearance.BorderSize = 1;
            btnNhapHang.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnNhapHang.BackColor = Color.White;
            btnNhapHang.ForeColor = UiTheme.TextMain;
            btnNhapHang.Font = new Font("Segoe UI", 11.5f, FontStyle.Bold);
            btnNhapHang.Cursor = Cursors.Hand;
            btnNhapHang.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 245, 249);

            // Sản phẩm
            btnSanPham.FlatStyle = FlatStyle.Flat;
            btnSanPham.FlatAppearance.BorderSize = 1;
            btnSanPham.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnSanPham.BackColor = Color.White;
            btnSanPham.ForeColor = UiTheme.TextMain;
            btnSanPham.Font = new Font("Segoe UI", 11.5f, FontStyle.Bold);
            btnSanPham.Cursor = Cursors.Hand;
            btnSanPham.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 245, 249);

            // Báo cáo
            btnBaoCao.FlatStyle = FlatStyle.Flat;
            btnBaoCao.FlatAppearance.BorderSize = 1;
            btnBaoCao.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnBaoCao.BackColor = Color.White;
            btnBaoCao.ForeColor = UiTheme.TextMain;
            btnBaoCao.Font = new Font("Segoe UI", 11.5f, FontStyle.Bold);
            btnBaoCao.Cursor = Cursors.Hand;
            btnBaoCao.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 245, 249);

            // Tra cứu
            btnTraCuu.FlatStyle = FlatStyle.Flat;
            btnTraCuu.FlatAppearance.BorderSize = 1;
            btnTraCuu.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnTraCuu.BackColor = Color.White;
            btnTraCuu.ForeColor = UiTheme.TextMain;
            btnTraCuu.Font = new Font("Segoe UI", 11.5f, FontStyle.Bold);
            btnTraCuu.Cursor = Cursors.Hand;
            btnTraCuu.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 245, 249);

            // Đăng xuất
            btnDangXuat.FlatStyle = FlatStyle.Flat;
            btnDangXuat.FlatAppearance.BorderSize = 1;
            btnDangXuat.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
            btnDangXuat.BackColor = Color.FromArgb(254, 242, 242);
            btnDangXuat.ForeColor = UiTheme.Danger;
            btnDangXuat.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
            btnDangXuat.Cursor = Cursors.Hand;
            btnDangXuat.FlatAppearance.MouseOverBackColor = Color.FromArgb(254, 226, 226);
        }

        private void btnBanHang_Click(object sender, EventArgs e)
        {
            var frm = new frmHoaDonBan(_username);
            frm.ShowDialog(this);
        }

        private void btnSanPham_Click(object sender, EventArgs e)
        {
            var frm = new frmSanPham(_role);
            frm.ShowDialog(this);
        }

        private void btnNhapHang_Click(object sender, EventArgs e)
        {
            var frm = new frmHoaDonNhap(_username);
            frm.ShowDialog(this);
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
