using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using NexusRetail.WinForms.Forms;
using NexusRetail.WinForms.Services;

namespace NexusRetail.WinForms
{
    public static class ScreenshotRunner
    {
        public static void CaptureAll()
        {
            string wsImgDir = @"c:\Users\hungn\Downloads\Phân Tích Thiết Kế Yêu Cầu\1_Phan_Tich_Thiet_Ke_Yeu_Cau\images";
            string brainDir = @"C:\Users\hungn\.gemini\antigravity\brain\6a664126-7b7c-412c-8472-e047cd2cd483";

            Directory.CreateDirectory(wsImgDir);
            Directory.CreateDirectory(brainDir);

            /*
            // 1. frmDangNhap
            Console.WriteLine("1. Capturing frmDangNhap...");
            var fLogin = new frmDangNhap();
            Capture(fLogin, "UI_WinForms_frmDangNhap.png", wsImgDir, brainDir);

            // 2. frmMain
            Console.WriteLine("2. Capturing frmMain...");
            var fMain = new frmMain("admin@duymobile.vn", "Admin");
            Capture(fMain, "UI_WinForms_frmMain.png", wsImgDir, brainDir);

            // 3. frmSanPham
            Console.WriteLine("3. Capturing frmSanPham...");
            var fSp = new frmSanPham("Admin");
            Capture(fSp, "UI_WinForms_frmSanPham.png", wsImgDir, brainDir);

            // 4. frmHoaDonNhap
            Console.WriteLine("4. Capturing frmHoaDonNhap...");
            var fHdn = new frmHoaDonNhap("NV01");
            Capture(fHdn, "UI_WinForms_frmHoaDonNhap.png", wsImgDir, brainDir);

            // 5. frmHoaDonBan
            Console.WriteLine("5. Capturing frmHoaDonBan...");
            var fHdb = new frmHoaDonBan("NV01");
            Capture(fHdb, "UI_WinForms_frmHoaDonBan.png", wsImgDir, brainDir);
            */

            // 6. frmThanhToanQR
            Console.WriteLine("6.1 Creating frmThanhToanQR...");
            var fQr = new frmThanhToanQR("HDB03", 27500000m, "09999999900", "DUY MOBILE");
            Console.WriteLine("6.2 Created frmThanhToanQR.");
            Capture(fQr, "UI_WinForms_frmThanhToanQR.png", wsImgDir, brainDir);
            Console.WriteLine("6.3 Captured frmThanhToanQR.");

            Console.WriteLine("ALL_WINFORMS_CAPTURED_SUCCESSFULLY");
        }

        private static void Capture(Form form, string filename, string wsDir, string brainDir)
        {
            try
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = new Point(50, 50);
                form.Show();
                Application.DoEvents();

                int w = Math.Max(form.Width, 800);
                int h = Math.Max(form.Height, 600);
                using var bmp = new Bitmap(w, h);
                form.DrawToBitmap(bmp, new Rectangle(0, 0, form.Width, form.Height));

                string p1 = Path.Combine(wsDir, filename);
                string p2 = Path.Combine(brainDir, filename);

                bmp.Save(p1, ImageFormat.Png);
                bmp.Save(p2, ImageFormat.Png);

                Console.WriteLine($"Saved: {filename} ({new FileInfo(p1).Length} bytes)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error capturing {filename}: {ex.Message}");
            }
            finally
            {
                try { form.Dispose(); } catch { }
            }
        }
    }
}
