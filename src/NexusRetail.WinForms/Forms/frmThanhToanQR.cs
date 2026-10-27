using NexusRetail.Shared.Helpers;
using NexusRetail.WinForms.Services;
using NexusRetail.WinForms.UI;
using QRCoder;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Media;
using System.Text;

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

            UiTheme.BeautifyForm(this);
            ApplyCustomStyles();

            KeyPreview = true;
            KeyDown += FrmThanhToanQR_KeyDown;

            lblMaDon.Text = $"Mã đơn hàng: {_maHdb}  •  Tự động khớp lệnh Webhook";
            lblSoTien.Text = $"{_soTien:N0} ₫";
            lblNoiDung.Text = $"DUYMOBILE {_maHdb}";
            lblStk.Text = _accountNumber;
            lblChuTk.Text = _userBankName;
            lblNganHang.Text = "MB Bank (Ngân hàng TMCP Quân đội)";

            picQr.BringToFront();

            // 1. Sinh ngay lập tức mã VietQR EMVCo chuẩn NAPAS 100% OFFLINE (0ms, không phụ thuộc mạng hay API ngoài)
            GenerateAndDisplayQrOffline();

            this.Shown += (s, e) =>
            {
                if (picQr.Image == null)
                {
                    GenerateAndDisplayQrOffline();
                }
            };

            // Khởi động Timer lắng nghe Webhook (1.5 giây/lần)
            _pollingTimer.Interval = 1500;
            _pollingTimer.Tick += PollingTimer_Tick;
            _pollingTimer.Start();
        }

        private void ApplyCustomStyles()
        {
            UiTheme.ApplyFormHeader(pnlHeader, lblTitle, lblMaDon, "THANH TOÁN VIETQR (NAPAS 24/7 • MBBANK)", $"Mã đơn hàng: {_maHdb}");
            lblSoTien.Font = UiTheme.FontTitle;
            lblSoTien.ForeColor = UiTheme.Danger;

            UiTheme.ApplyPrimaryButton(btnKiemTra, "🔄 Kiểm Tra Thanh Toán (F5)");
            UiTheme.ApplySecondaryButton(btnDong, "❌ Đóng Giao Dịch (Esc)");
        }


        /// <summary>
        /// Tạo và hiển thị mã VietQR EMVCo chuẩn NAPAS hoàn toàn offline bằng code C#
        /// </summary>
        private void GenerateAndDisplayQrOffline()
        {
            try
            {
                // Xây dựng chuỗi EMVCo chuẩn VietQR MBBank
                string emvPayload = BuildVietQrEmvCoString(
                    bin: "970422", // MBBank NAPAS BIN
                    accountNumber: _accountNumber,
                    amount: (long)_soTien,
                    content: $"DUYMOBILE {_maHdb}"
                );

                using var qrGenerator = new QRCodeGenerator();
                using var qrCodeData = qrGenerator.CreateQrCode(emvPayload, QRCodeGenerator.ECCLevel.M);

                // Ưu tiên 1: Tạo trực tiếp Bitmap chuẩn GDI+ từ QRCoder
                using var qrCode = new QRCode(qrCodeData);
                using var bmp = qrCode.GetGraphic(12, Color.Black, Color.White, true);

                var oldImg = picQr.Image;
                picQr.Image = (Image)bmp.Clone();
                oldImg?.Dispose();
                picQr.Refresh();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[VietQR] Lỗi render bằng QRCode: " + ex.Message);

                // Ưu tiên 2: Fallback với PngByteQRCode (tạo Bitmap độc lập an toàn bộ nhớ)
                try
                {
                    string emvPayload = BuildVietQrEmvCoString("970422", _accountNumber, (long)_soTien, $"DUYMOBILE {_maHdb}");
                    using var qrGen = new QRCodeGenerator();
                    using var qrData = qrGen.CreateQrCode(emvPayload, QRCodeGenerator.ECCLevel.M);
                    using var pngQr = new PngByteQRCode(qrData);
                    byte[] bytes = pngQr.GetGraphic(12);

                    using var ms = new MemoryStream(bytes);
                    using var temp = Image.FromStream(ms);
                    var detachedBmp = new Bitmap(temp.Width, temp.Height);
                    using (var g = Graphics.FromImage(detachedBmp))
                    {
                        g.DrawImage(temp, 0, 0, temp.Width, temp.Height);
                    }

                    var oldImg = picQr.Image;
                    picQr.Image = detachedBmp;
                    oldImg?.Dispose();
                    picQr.Refresh();
                }
                catch (Exception ex2)
                {
                    Debug.WriteLine("[VietQR] Lỗi fallback render: " + ex2.Message);
                }
            }
        }

        #region Thuật toán tạo chuỗi VietQR EMVCo (NAPAS 247) 100% Offline
        public static string BuildVietQrEmvCoString(string bin, string accountNumber, long amount, string content)
            => VietQrHelper.BuildVietQrEmvCoString(bin, accountNumber, amount, content);

        public static string RemoveDiacritics(string text)
            => VietQrHelper.RemoveDiacritics(text);

        public static string ComputeCrc16(string data)
            => VietQrHelper.ComputeCrc16(data);
        #endregion

        private async void PollingTimer_Tick(object? sender, EventArgs e)
        {
            if (IsDisposed || !Visible || IsPaymentSuccessful) return;

            var result = await WinFormsApiClient.Instance.CheckBankPaymentStatusAsync(_maHdb, _soTien);

            if (IsDisposed || !Visible || IsPaymentSuccessful) return;

            if (result.PaidAmount > 0)
            {
                // SO SÁNH SỐ TIỀN:
                if (result.PaidAmount < _soTien || !result.IsPaid)
                {
                    // Khách chuyển thiếu tiền -> Cảnh báo màu đỏ nổi bật, thông báo rõ số tiền thiếu
                    pnlStatus.BackColor = Color.FromArgb(239, 68, 68); // Red warning
                    lblStatus.ForeColor = Color.White;
                    decimal missing = _soTien - result.PaidAmount;
                    lblStatus.Text = $"⚠ KHÁCH CHUYỂN THIẾU TIỀN: Đã nhận {result.PaidAmount:N0}đ / Cần {_soTien:N0}đ (Thiếu {missing:N0}đ)!";
                }
                else
                {
                    // Khách chuyển đủ hoặc thừa tiền -> Hoàn tất thanh toán
                    OnPaymentSuccess(result.TransactionCode ?? "MB_AUTO", result.PaidAmount);
                }
            }
        }

        private async void OnPaymentSuccess(string txCode, decimal? paidAmount = null)
        {
            if (IsPaymentSuccessful) return;
            IsPaymentSuccessful = true;
            _pollingTimer.Stop();

            decimal amount = paidAmount ?? _soTien;

            if (IsDisposed) return;

            // Đổi trạng thái sang màu xanh lục
            pnlStatus.BackColor = Color.FromArgb(16, 185, 129); // Modern Emerald Green
            lblStatus.ForeColor = Color.White;
            lblStatus.Text = $"✔ ĐÃ NHẬN ĐỦ {amount:N0}đ! (MÃ GD: {txCode})";

            // Đổi nút đóng thành nút hoàn tất
            btnDong.Text = "✔ Hoàn Tất Hóa Đơn (Enter)";
            btnDong.BackColor = Color.FromArgb(16, 185, 129);
            btnDong.ForeColor = Color.White;
            btnDong.Font = new Font(btnDong.Font, FontStyle.Bold);

            // Phát âm thanh Ting Ting
            try
            {
                SystemSounds.Exclamation.Play();
            }
            catch { }

            // Tự động đóng form chuyển tiếp sau 1.5 giây
            await Task.Delay(1500);

            if (!IsDisposed && Visible)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        // Bấm phím F5 hoặc nút Kiểm tra để đối soát ngân hàng ngay lập tức
        private async void FrmThanhToanQR_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                await KiemTraThanhToanAsync();
            }
            else if (e.KeyCode == Keys.Enter && IsPaymentSuccessful)
            {
                _pollingTimer.Stop();
                DialogResult = DialogResult.OK;
                Close();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                _pollingTimer.Stop();
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private async void btnKiemTra_Click(object sender, EventArgs e)
        {
            await KiemTraThanhToanAsync();
        }

        private async Task KiemTraThanhToanAsync()
        {
            if (IsDisposed || !Visible || IsPaymentSuccessful) return;

            btnKiemTra.Enabled = false;
            btnKiemTra.Text = "Đang kiểm tra...";
            lblStatus.Text = "⏳ Đang kết nối ngân hàng đối soát giao dịch...";

            try
            {
                var result = await WinFormsApiClient.Instance.CheckBankPaymentStatusAsync(_maHdb, _soTien);

                if (IsDisposed || !Visible || IsPaymentSuccessful) return;

                if (result.PaidAmount > 0)
                {
                    if (result.PaidAmount < _soTien || !result.IsPaid)
                    {
                        pnlStatus.BackColor = Color.FromArgb(239, 68, 68);
                        lblStatus.ForeColor = Color.White;
                        decimal missing = _soTien - result.PaidAmount;
                        lblStatus.Text = $"⚠ KHÁCH CHUYỂN THIẾU TIỀN: Đã nhận {result.PaidAmount:N0}đ / Cần {_soTien:N0}đ (Thiếu {missing:N0}đ)!";
                    }
                    else
                    {
                        OnPaymentSuccess(result.TransactionCode ?? "MB_AUTO", result.PaidAmount);
                    }
                }
                else
                {
                    lblStatus.Text = "⏳ Chưa phát hiện giao dịch khớp. Vui lòng kiểm tra lại app ngân hàng!";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Lỗi kiểm tra giao dịch: {ex.Message}";
            }
            finally
            {
                if (!IsDisposed && !IsPaymentSuccessful)
                {
                    btnKiemTra.Enabled = true;
                    btnKiemTra.Text = "🔄 Kiểm Tra Thanh Toán (F5)";
                }
            }
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            _pollingTimer.Stop();
            DialogResult = IsPaymentSuccessful ? DialogResult.OK : DialogResult.Cancel;
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _pollingTimer.Stop();
            _pollingTimer.Dispose();
            picQr.Image?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
