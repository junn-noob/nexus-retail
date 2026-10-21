namespace NexusRetail.WinForms.Forms
{
    public partial class frmMain : Form
    {
        private readonly string _username;
        private readonly string _role;

        public frmMain(string username, string role)
        {
            _username = username;
            _role = role;

            InitializeComponent();

            lblUserDisplay.Text = $"Xin chào: {_username} | Vai trò: {_role}";
            
            // Phân quyền: Nếu là Nhân viên (Staff) thì khóa tính năng Báo cáo doanh thu tài chính
            if (_role != "Admin")
            {
                btnBaoCao.Enabled = false;
                btnBaoCao.Text += " 🔒(Chỉ Admin)";
            }
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
            Close();
        }
    }
}
