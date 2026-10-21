using NexusRetail.WinForms.Services;
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

            KeyPreview = true;
            KeyDown += FrmThanhToanQR_KeyDown;

            lblMaDon.Text = $"Mã HĐ: {_maHdb}";
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

        private static string Tlv(string tag, string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return $"{tag}{value.Length:D2}{value}";
        }

        public static string BuildVietQrEmvCoString(string bin, string accountNumber, long amount, string content)
        {
            // Tag 00: Version (01)
            string payload = "000201";

            // Tag 01: Phương thức khởi tạo (11: Động có số tiền, 12: Tĩnh)
            payload += amount > 0 ? "010211" : "010212";

            // Tag 38: Thông tin tài khoản thụ hưởng NAPAS
            string sub00 = "0010A000000727"; // GUID NAPAS
            string subSub00 = Tlv("00", bin); // Mã định danh ngân hàng (MB: 970422)
            string subSub01 = Tlv("01", accountNumber); // Số tài khoản thụ hưởng
            string sub01 = Tlv("01", subSub00 + subSub01);
            string sub02 = "0208QRIBFTTA"; // Mã dịch vụ chuyển nhanh NAPAS 24/7 đến tài khoản
            payload += Tlv("38", sub00 + sub01 + sub02);

            // Tag 53: Mã tiền tệ (704 = VND)
            payload += "5303704";

            // Tag 54: Số tiền giao dịch
            if (amount > 0)
            {
                payload += Tlv("54", amount.ToString());
            }

            // Tag 58: Mã quốc gia (VN)
            payload += "5802VN";

            // Tag 62: Thông tin bổ sung (Nội dung chuyển khoản)
            if (!string.IsNullOrWhiteSpace(content))
            {
                string cleanContent = RemoveDiacritics(content.Trim());
                string sub62_08 = Tlv("08", cleanContent);
                payload += Tlv("62", sub62_08);
            }

            // Tag 63: Khai báo trường CRC (Độ dài 04)
            payload += "6304";

            // Tính mã kiểm tra CRC-16/CCITT-FALSE (Đa thức 0x1021, Khởi tạo 0xFFFF)
            string crc = ComputeCrc16(payload);
            return payload + crc;
        }

        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC).Replace('đ', 'd').Replace('Đ', 'D');
        }

        public static string ComputeCrc16(string data)
        {
            ushort crc = 0xFFFF;
            byte[] bytes = Encoding.UTF8.GetBytes(data);

            foreach (byte b in bytes)
            {
                crc ^= (ushort)(b << 8);
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x8000) != 0)
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    else
                        crc = (ushort)(crc << 1);
                }
            }

            return crc.ToString("X4");
        }

        #endregion

        private async void PollingTimer_Tick(object? sender, EventArgs e)
        {
            var result = await WinFormsApiClient.Instance.CheckBankPaymentStatusAsync(_maHdb, _soTien);

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
            _pollingTimer.Stop();
            IsPaymentSuccessful = true;

            decimal amount = paidAmount ?? _soTien;

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

        // Bấm phím F9 hoặc nút Simulate để giả lập Webhook tức thì (Dùng cho Demo bảo vệ)
        private async void FrmThanhToanQR_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F9)
            {
                await TriggerSimulateAsync();
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

        private async void btnSimulate_Click(object sender, EventArgs e)
        {
            await TriggerSimulateAsync();
        }

        private async Task TriggerSimulateAsync()
        {
            btnSimulate.Enabled = false;
            btnSimulate.Text = "Đang gửi Webhook giả lập...";
            try
            {
                var (success, _) = await WinFormsApiClient.Instance.SimulateBankPaymentAsync(_maHdb, _soTien);
                if (success)
                {
                    OnPaymentSuccess("SIM_MBBANK", _soTien);
                }
                else
                {
                    OnPaymentSuccess("SIM_OFFLINE", _soTien);
                }
            }
            catch
            {
                OnPaymentSuccess("SIM_OFFLINE", _soTien);
            }
            finally
            {
                btnSimulate.Enabled = true;
                btnSimulate.Text = "⚡ Giả lập Webhook MBBank (F9)";
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
