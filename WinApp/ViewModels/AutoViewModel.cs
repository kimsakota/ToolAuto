using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WinApp.ViewModels
{
    // Class mô tả một bước click
    public class ClickStep
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int DelayMs { get; set; }

        public override string ToString()
        {
            return $"👉 Click ({X}, {Y}) -> Chờ {DelayMs} ms";
        }
    }

    // ViewModel dùng chung (Singleton) để Tab Setting và Tab Run thấy nhau
    public class AutoViewModel : INotifyPropertyChanged
    {
        private static AutoViewModel _instance;
        public static AutoViewModel Instance => _instance ?? (_instance = new AutoViewModel());

        // Danh sách các bước chạy (Macro)
        public ObservableCollection<ClickStep> ScriptSteps { get; set; } = new ObservableCollection<ClickStep>();

        // Cấu hình OCR
        public string X1 { get; set; } = "1261";
        public string Y1 { get; set; } = "507";
        public string X2 { get; set; } = "1595";
        public string Y2 { get; set; } = "526";
        public string Keyword { get; set; } = "Trường dữ liệu bạn nhập bị lỗi. Vui lòng kiểm tra lại";
        public bool IsExactMatch { get; set; } = true;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}