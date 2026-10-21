using NexusRetail.WinForms.Services;
using QRCoder;
using System.Diagnostics;
using System.Media;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace NexusRetail.WinForms.Forms
{
    public partial class frmThanhToanQR : Form
    {
        private readonly string _maHdb;
        private readonly decimal _soTien;
        private readonly string _accountNumber;
        private readonly string _userBankName;
        private readonly System.Windows.Forms.Timer _pollingTimer = new();
        public bool IsPaymentSuccessful { get; private set; }

        public frmThanhToanQR(string maHdb, decimal soTien, string accountNumber = "09999999900", string userBankName = "NGUYEN VAN HUNG")
        {
            _maHdb = maHdb;
            _soTien = soTien;
            _accountNumber = accountNumber;
            _userBankName = userBankName;

            InitializeComponent();

            KeyPreview = true;
            KeyDown += FrmThanhToanQR_KeyDown;

            lblMaDon.Text = $"Mã HĐ: {_maHdb}";
            lblSoTien.Text = $"{_soTien:N0} ₫";
            lblNoiDung.Text = $"DUYMOBILE {_maHdb}";
            lblStk.Text = _accountNumber;
            lblChuTk.Text = _userBankName;
            lblNganHang.Text = "MB Bank (Ngân hàng TMCP Quân đội)";

            picQr.BringToFront();

            // 1. Sinh ngay lập tức mã QR bằng QRCoder (0ms, 100% offline, hiện tức thì)
            string quickEmv = $"00020101021138550010A000000727012500069704220111{_accountNumber}0208QRIBFTTA5303704540{((long)_soTien).ToString().Length:D2}{(long)_soTien}5802VN62{($"08{($"DUYMOBILE {_maHdb}".Length):D2}DUYMOBILE {_maHdb}").Length:D2}08{($"DUYMOBILE {_maHdb}".Length):D2}DUYMOBILE {_maHdb}6304";
            RenderQrCodeWithQRCoder(quickEmv);

            // 2. Khi form hiện lên, tải ảnh VietQR chính thức từ server để nâng cấp
            this.Shown += async (s, e) =>
            {
                await LoadVietQrImageAsync();
            };

            // Khởi động Timer lắng nghe Webhook (1.5 giây/lần)
            _pollingTimer.Interval = 1500;
            _pollingTimer.Tick += PollingTimer_Tick;
            _pollingTimer.Start();
        }

        private static Image LoadImageFromBytes(byte[] bytes)
        {
            using var ms = new MemoryStream(bytes);
            using var temp = Image.FromStream(ms);
            return new Bitmap(temp);
        }

        private async Task LoadVietQrImageAsync()
        {
            // 1. Ưu tiên 1: Lấy trực tiếp từ img.vietqr.io (SVG nhúng PNG base64 480x480 chính thức)
            try
            {
                string encodedContent = Uri.EscapeDataString($"DUYMOBILE {_maHdb}");
                string svgUrl = $"https://img.vietqr.io/image/MB-{_accountNumber}-qr_only.svg?amount={(long)_soTien}&addInfo={encodedContent}";

                using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
                string svgContent = await http.GetStringAsync(svgUrl);

                var match = Regex.Match(svgContent, @"href=""data:image/png;base64,([^""]+)""");
                if (match.Success)
                {
                    byte[] pngBytes = Convert.FromBase64String(match.Groups[1].Value);
                    var oldImg = picQr.Image;
                    picQr.Image = LoadImageFromBytes(pngBytes);
                    oldImg?.Dispose();
                    picQr.Refresh();
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[VietQR] Không thể tải SVG: " + ex.Message);
            }

            // 2. Ưu tiên 2: Gọi POST API vietqr.vn/api/vietqr/generate lấy chuỗi EMVCo qrCode
            try
            {
                using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
                var reqObj = new
                {
                    bankAccount = _accountNumber,
                    userBankName = _userBankName,
                    bankCode = "MB",
                    amount = ((long)_soTien).ToString(),
                    content = $"DUYMOBILE {_maHdb}"
                };

                var res = await http.PostAsJsonAsync("https://vietqr.vn/api/vietqr/generate", reqObj);
                if (res.IsSuccessStatusCode)
                {
                    var jsonDoc = await res.Content.ReadFromJsonAsync<JsonElement>();
                    if (jsonDoc.TryGetProperty("qrCode", out var qrEl) && !string.IsNullOrEmpty(qrEl.GetString()))
                    {
                        RenderQrCodeWithQRCoder(qrEl.GetString()!);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[VietQR] Không thể gọi API vietqr.vn: " + ex.Message);
            }

            // 3. Ưu tiên 3: QuickChart API
            try
            {
                string emvString = $"https://vietqr.me/MB/{_accountNumber}/{(long)_soTien}/DUYMOBILE%20{_maHdb}";
                using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(4) };
                byte[] qrBytes = await http.GetByteArrayAsync($"https://quickchart.io/qr?text={Uri.EscapeDataString(emvString)}&size=300");
                var oldImg = picQr.Image;
                picQr.Image = LoadImageFromBytes(qrBytes);
                oldImg?.Dispose();
                picQr.Refresh();
            }
            catch
            {
                // Fallback offline với thư viện QRCoder cục bộ
                RenderQrCodeWithQRCoder($"https://vietqr.me/MB/{_accountNumber}/{(long)_soTien}/DUYMOBILE%20{_maHdb}");
            }
        }

        private void RenderQrCodeWithQRCoder(string text)
        {
            try
            {
                using var qrGenerator = new QRCodeGenerator();
                using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.M);
                using var qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrBytes = qrCode.GetGraphic(20);

                var oldImg = picQr.Image;
                picQr.Image = LoadImageFromBytes(qrBytes);
                oldImg?.Dispose();
                picQr.Refresh();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[VietQR] RenderQrCodeWithQRCoder error: " + ex.Message);
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
