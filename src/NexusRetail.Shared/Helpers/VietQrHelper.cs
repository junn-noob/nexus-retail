using System.Globalization;
using System.Text;

namespace NexusRetail.Shared.Helpers
{
    public static class VietQrHelper
    {
        private static string Tlv(string tag, string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return $"{tag}{value.Length:D2}{value}";
        }

        /// <summary>
        /// Tạo chuỗi VietQR EMVCo chuẩn NAPAS 24/7 hoàn toàn offline (chuẩn VietQR.io / NAPAS)
        /// </summary>
        public static string BuildVietQrEmvCoString(string bin, string accountNumber, long amount, string content)
        {
            // Tag 00: Version (01)
            string payload = "000201";

            // Tag 01: Phương thức khởi tạo (11: Động có số tiền, 12: Tĩnh không kèm số tiền)
            payload += amount > 0 ? "010211" : "010212";

            // Tag 38: Thông tin tài khoản thụ hưởng NAPAS
            string sub00 = "0010A000000727"; // GUID NAPAS
            string subSub00 = Tlv("00", bin); // Mã BIN ngân hàng (MBBank: 970422)
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

            // Tag 62: Thông tin bổ sung (Nội dung chuyển khoản không dấu)
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
    }
}
