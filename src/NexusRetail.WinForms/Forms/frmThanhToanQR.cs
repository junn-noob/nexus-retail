using NexusRetail.WinForms.Services;
using System.Diagnostics;
using System.Media;

namespace NexusRetail.WinForms.Forms
{
    public partial class frmThanhToanQR : Form
    {
        private readonly string _maHdb;
        private readonly decimal _soTien;
        private readonly string _accountNumber;
        private readonly System.Windows.Forms.Timer _pollingTimer = new();
        public bool IsPaymentSuccessful { get; private set; }

        public frmThanhToanQR(string maHdb, decimal soTien, string accountNumber = "0988888888")
        {
            _maHdb = maHdb;
            _soTien = soTien;
            _accountNumber = accountNumber;

            InitializeComponent();

            KeyPreview = true;
            KeyDown += FrmThanhToanQR_KeyDown;

            lblMaDon.Text = $"Mã HĐ: {_maHdb}";
            lblSoTien.Text = $"{_soTien:N0} ₫";
            lblNoiDung.Text = $"DUYMOBILE {_maHdb}";
            lblStk.Text = _accountNumber;

            // Nạp mã VietQR động
            LoadVietQrImage();

            // Khởi động Timer lắng nghe Webhook (1.5 giây/lần)
            _pollingTimer.Interval = 1500;
            _pollingTimer.Tick += PollingTimer_Tick;
            _pollingTimer.Start();
        }

        private async void LoadVietQrImage()
        {
            try
            {
                string qrUrl = $"https://img.vietqr.io/image/MB-{_accountNumber}-compact2.png?amount={(long)_soTien}&addInfo=DUYMOBILE%20{_maHdb}&accountName=DUY%20MOBILE";
                using var http = new HttpClient();
                var bytes = await http.GetByteArrayAsync(qrUrl);
                using var ms = new MemoryStream(bytes);
                picQr.Image = Image.FromStream(ms);
            }
            catch
            {
                // Fallback nếu không có internet: vẽ placeholder QR trực tiếp
                var bmp = new Bitmap(260, 260);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                    using var font = new Font("Segoe UI", 11, FontStyle.Bold);
                    g.DrawString($"VIETQR MBBANK\n\nDUYMOBILE {_maHdb}\n{_soTien:N0} ₫", font, Brushes.DarkBlue, new PointF(30, 90));
                    g.DrawRectangle(Pens.DarkBlue, 10, 10, 240, 240);
                }
                picQr.Image = bmp;
            }
        }

        private async void PollingTimer_Tick(object? sender, EventArgs e)
        {
            var result = await WinFormsApiClient.Instance.CheckBankPaymentStatusAsync(_maHdb);
            if (result.IsPaid)
            {
                OnPaymentSuccess(result.TransactionCode ?? "MB_AUTO");
            }
        }

        private void OnPaymentSuccess(string txCode)
        {
            _pollingTimer.Stop();
            IsPaymentSuccessful = true;

            // Đổi trạng thái sang màu xanh lục
            pnlStatus.BackColor = Color.FromArgb(16, 185, 129); // Modern Emerald Green
            lblStatus.ForeColor = Color.White;
            lblStatus.Text = $"✔ ĐÃ NHẬN TIỀN THÀNH CÔNG! (MÃ GD: {txCode})";

            // Phát âm thanh Ting Ting
            try
            {
                SystemSounds.Exclamation.Play();
            }
            catch { }

            // Tự động đóng form sau 1.8 giây để hoàn tất hóa đơn
            var closeTimer = new System.Windows.Forms.Timer { Interval = 1800 };
            closeTimer.Tick += (s, ev) =>
            {
                closeTimer.Stop();
                DialogResult = DialogResult.OK;
                Close();
            };
            closeTimer.Start();
        }

        // Bấm phím F9 hoặc nút Simulate để giả lập Webhook tức thì (Dùng cho Demo bảo vệ)
        private async void FrmThanhToanQR_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F9)
            {
                await TriggerSimulateAsync();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                _pollingTimer.Stop();
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private async void btnSimulate_Click(object sender, EventArgs e)
        {
            await TriggerSimulateAsync();
        }

        private async Task TriggerSimulateAsync()
        {
            btnSimulate.Enabled = false;
            btnSimulate.Text = "Đang gửi Webhook giả lập...";
            var (success, _) = await WinFormsApiClient.Instance.SimulateBankPaymentAsync(_maHdb, _soTien);
            if (!success)
            {
                // Fallback cục bộ nếu API chưa chạy
                OnPaymentSuccess("SIM_OFFLINE");
            }
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            _pollingTimer.Stop();
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _pollingTimer.Stop();
            base.OnFormClosing(e);
        }
    }
}
