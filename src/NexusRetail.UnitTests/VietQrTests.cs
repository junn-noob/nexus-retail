using NexusRetail.Shared.Helpers;
using Xunit;

namespace NexusRetail.UnitTests
{
    public class VietQrTests
    {
        [Fact]
        public void BuildVietQrEmvCoString_ValidParameters_ProducesStandardEmvCoFormat()
        {
            // Arrange
            string bin = "970422"; // MBBank
            string stk = "09999999900";
            long amount = 25000000;
            string content = "DUYMOBILE HDB01";

            // Act
            string payload = VietQrHelper.BuildVietQrEmvCoString(bin, stk, amount, content);

            // Assert
            Assert.NotNull(payload);
            Assert.StartsWith("000201", payload); // Tag 00: Version
            Assert.Contains("010211", payload);   // Tag 01: Dynamic QR with amount
            Assert.Contains("0010A000000727", payload); // NAPAS GUID
            Assert.Contains(bin, payload);        // MBBank BIN
            Assert.Contains(stk, payload);        // Account Number
            Assert.Contains("5303704", payload);  // Currency VND (704)
            Assert.Contains("540825000000", payload); // Amount: 25000000 (length 08)
            Assert.Contains("5802VN", payload);   // Country: VN
            Assert.Contains("6304", payload);     // CRC tag
            Assert.Equal(4, payload.Substring(payload.IndexOf("6304") + 4).Length); // 4-char hex CRC
        }

        [Fact]
        public void RemoveDiacritics_VietnameseInput_ReturnsUnaccentedString()
        {
            // Arrange
            string input = "Nguyễn Văn Hùng thanh toán đơn hàng ĐTDĐ";

            // Act
            string result = VietQrHelper.RemoveDiacritics(input);

            // Assert
            Assert.Equal("Nguyen Van Hung thanh toan don hang DTDD", result);
        }

        [Theory]
        [InlineData("123456789", "29B1")] // Standard CRC16-CCITT test vector (0xFFFF init, 0x1021 poly)
        public void ComputeCrc16_KnownData_CalculatesCorrectChecksum(string input, string expectedCrc)
        {
            // Act
            string calculated = VietQrHelper.ComputeCrc16(input);

            // Assert
            Assert.Equal(expectedCrc, calculated);
        }
    }
}
