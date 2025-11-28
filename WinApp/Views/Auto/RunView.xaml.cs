using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Tesseract;
using WinApp.ViewModels;

namespace WinApp.Views.Auto
{
    public partial class RunView : UserControl
    {
        private DispatcherTimer _ocrTimer;
        private DispatcherTimer _mouseTimer;
        private bool _isRunning = false;
        private TesseractEngine _ocrEngine;

        // --- WIN32 API ---
        [DllImport("user32.dll")] static extern bool SetCursorPos(int x, int y);
        [DllImport("user32.dll")] public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        [DllImport("user32.dll")] public static extern bool GetCursorPos(out POINT lpPoint);
        public struct POINT { public int X; public int Y; }
        const int MOUSEEVENTF_LEFTDOWN = 0x02;
        const int MOUSEEVENTF_LEFTUP = 0x04;

        public RunView()
        {
            InitializeComponent();

            var vm = AutoViewModel.Instance;
            txtX1.Text = vm.X1; txtY1.Text = vm.Y1;
            txtX2.Text = vm.X2; txtY2.Text = vm.Y2;
            txtKeyword.Text = vm.Keyword;
            chkExact.IsChecked = vm.IsExactMatch;

            this.Unloaded += (s, e) => {
                vm.X1 = txtX1.Text; vm.Y1 = txtY1.Text;
                vm.X2 = txtX2.Text; vm.Y2 = txtY2.Text;
                vm.Keyword = txtKeyword.Text;
                vm.IsExactMatch = chkExact.IsChecked == true;
            };

            _ocrTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _ocrTimer.Tick += async (s, e) => await ProcessOcr();

            _mouseTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _mouseTimer.Tick += (s, e) => {
                if (GetCursorPos(out POINT p)) lblMouse.Text = $"Mouse: {p.X}, {p.Y}";
            };
            _mouseTimer.Start();

            btnStart.Click += ToggleRun;

            // Gọi khởi tạo sau khi giao diện đã hiện để in Log
            this.Loaded += (s, e) => InitializeOcr();
        }

        private void InitializeOcr()
        {
            try
            {
                // 1. Lấy đường dẫn thư mục gốc
                string exePath = AppDomain.CurrentDomain.BaseDirectory;
                string tessDataPath = Path.Combine(exePath, "tessdata");

                Log("=== KIỂM TRA MÔI TRƯỜNG ===");
                Log($"Đường dẫn gốc: {exePath}");
                Log($"Đang tìm thư mục data tại: {tessDataPath}");

                // 2. Kiểm tra thư mục
                if (!Directory.Exists(tessDataPath))
                {
                    Log("[LỖI] KHÔNG CÓ thư mục 'tessdata'.");
                    Log("Hãy tạo thư mục này ngay cạnh file WinApp.exe");
                    btnStart.IsEnabled = false;
                    return;
                }

                // 3. Liệt kê file bên trong để debug
                string[] files = Directory.GetFiles(tessDataPath);
                Log($"Tìm thấy {files.Length} file trong tessdata:");
                foreach (var f in files) Log(" - " + Path.GetFileName(f));

                // 4. Quyết định ngôn ngữ dựa trên file có sẵn
                string lang = "vie"; // Mặc định
                if (File.Exists(Path.Combine(tessDataPath, "eng.traineddata")) && !File.Exists(Path.Combine(tessDataPath, "vie.traineddata")))
                {
                    lang = "eng";
                }

                Log($"=> Đang thử khởi tạo với ngôn ngữ: '{lang}'...");

                _ocrEngine = new TesseractEngine(tessDataPath, lang, EngineMode.Default);
                Log("=> OCR KHỞI TẠO THÀNH CÔNG! (Màu xanh)");
                btnStart.IsEnabled = true;

            }
            catch (Exception ex)
            {
                Log("=> [LỖI CRITICAL]: " + ex.Message);
                if (ex.InnerException != null) Log("Chi tiết: " + ex.InnerException.Message);

                Log("GỢI Ý SỬA LỖI:");
                Log("1. File trong thư mục tessdata có dung lượng > 1MB không? (Nếu vài KB là sai)");
                Log("2. Bạn đã cài 'Visual C++ Redistributable 2015-2022' chưa?");
                btnStart.IsEnabled = false;
            }
        }

        private void ToggleRun(object sender, RoutedEventArgs e)
        {
            if (!_isRunning)
            {
                _isRunning = true;
                btnStart.Content = "DỪNG LẠI";
                btnStart.Background = System.Windows.Media.Brushes.Red;
                _ocrTimer.Start();
                Log("Đã bắt đầu quét...");
            }
            else
            {
                Stop();
            }
        }

        private void Stop()
        {
            _isRunning = false;
            _ocrTimer.Stop();
            btnStart.Content = "BẮT ĐẦU CHẠY";
            btnStart.Background = System.Windows.Media.Brushes.Green;
            Log("Đã dừng.");
        }

        private async Task ProcessOcr()
        {
            if (_ocrEngine == null) return;

            try
            {
                int.TryParse(txtX1.Text, out int x1); int.TryParse(txtY1.Text, out int y1);
                int.TryParse(txtX2.Text, out int x2); int.TryParse(txtY2.Text, out int y2);
                int w = Math.Abs(x2 - x1); int h = Math.Abs(y2 - y1);
                if (w < 5 || h < 5) return;

                using (Bitmap bmp = new Bitmap(w, h))
                {
                    using (Graphics g = Graphics.FromImage(bmp)) g.CopyFromScreen(Math.Min(x1, x2), Math.Min(y1, y2), 0, 0, bmp.Size);

                    using (var page = _ocrEngine.Process(bmp))
                    {
                        string text = page.GetText().Trim();
                        if (!string.IsNullOrEmpty(text))
                        {
                            Log($"Đọc được: {text}");
                            string key = txtKeyword.Text.Trim();
                            bool match = chkExact.IsChecked == true ? text.Equals(key, StringComparison.OrdinalIgnoreCase) : text.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0;

                            if (match)
                            {
                                Log("=> TÌM THẤY! Bắt đầu chuỗi thao tác...");
                                PerformClick(Math.Min(x1, x2) + w / 2, Math.Min(y1, y2) + h / 2);
                                await RunScript();
                                Stop();
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Log("Lỗi Runtime: " + ex.Message); }
        }

        private async Task RunScript()
        {
            var steps = AutoViewModel.Instance.ScriptSteps;
            if (steps.Count > 0)
            {
                Log($"=> Chạy tiếp {steps.Count} bước cấu hình...");
                await Task.Delay(500);
                foreach (var step in steps)
                {
                    Log($"Executing: {step}");
                    PerformClick(step.X, step.Y);
                    if (step.DelayMs > 0) await Task.Delay(step.DelayMs);
                }
                Log("=> Đã chạy xong Script.");
            }
        }

        private void PerformClick(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        private void Log(string msg) => txtLog.AppendText($"[{DateTime.Now:mm:ss}] {msg}\n");
    }
}