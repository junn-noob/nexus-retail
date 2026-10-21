using NexusRetail.WinForms.Services;

namespace NexusRetail.WinForms.Forms
{
    public partial class frmDangNhap : Form
    {
        public frmDangNhap()
        {
            InitializeComponent();
        }

        private async void btnDangNhap_Click(object sender, EventArgs e)
        {
            string user = txtUser.Text.Trim();
            string pass = txtPass.Text;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnDangNhap.Enabled = false;
            btnDangNhap.Text = "Đang xác thực...";

            var (success, role, message) = await WinFormsApiClient.Instance.LoginAsync(user, pass);

            btnDangNhap.Enabled = true;
            btnDangNhap.Text = "ĐĂNG NHẬP HỆ THỐNG";

            if (success)
            {
                Hide();
                var frmMain = new frmMain(user, role);
                frmMain.FormClosed += (s, args) => Close();
                frmMain.Show();
            }
            else
            {
                MessageBox.Show(message, "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnQuickAdmin_Click(object sender, EventArgs e)
        {
            txtUser.Text = "admin";
            txtPass.Text = "admin123";
            btnDangNhap_Click(sender, e);
        }

        private void btnQuickStaff_Click(object sender, EventArgs e)
        {
            txtUser.Text = "nhanvien";
            txtPass.Text = "123456";
            btnDangNhap_Click(sender, e);
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
