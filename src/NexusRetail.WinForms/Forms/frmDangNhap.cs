using NexusRetail.WinForms.Services;
using NexusRetail.WinForms.UI;

namespace NexusRetail.WinForms.Forms
{
    public partial class frmDangNhap : Form
    {
        public frmDangNhap()
        {
            InitializeComponent();

            AcceptButton = btnDangNhap;
            CancelButton = btnThoat;

            UiTheme.BeautifyForm(this);
            ApplyCustomStyles();
        }

        private void ApplyCustomStyles()
        {
            UiTheme.ApplyFormHeader(pnlHeader, lblTitle, lblSub, "KIOTVIET • DUY MOBILE POS", "Cổng đăng nhập hệ thống Quản lý Bán lẻ & Thu ngân");

            UiTheme.ApplyPrimaryButton(btnDangNhap, "🔐 ĐĂNG NHẬP HỆ THỐNG");
            UiTheme.ApplySecondaryButton(btnThoat, "❌ Thoát ứng dụng (Esc)");
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
                frmMain.FormClosed += (s, args) =>
                {
                    if (frmMain.IsLogoutRequested)
                    {
                        txtPass.Clear();
                        txtUser.Focus();
                        Show();
                    }
                    else
                    {
                        Close();
                    }
                };
                frmMain.Show();
            }
            else
            {
                MessageBox.Show(message, "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
