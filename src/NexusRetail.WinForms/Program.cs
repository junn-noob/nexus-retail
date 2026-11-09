namespace NexusRetail.WinForms;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        if (args.Length > 0 && args[0] == "--capture")
        {
            ScreenshotRunner.CaptureAll();
            return;
        }
        Application.Run(new Forms.frmDangNhap());
    }    
}