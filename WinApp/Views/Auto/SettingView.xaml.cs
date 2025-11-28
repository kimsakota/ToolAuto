using System.Windows;
using System.Windows.Controls;
using WinApp.ViewModels;

namespace WinApp.Views.Auto
{
    public partial class SettingView : UserControl
    {
        public SettingView()
        {
            InitializeComponent();

            // Liên kết với ViewModel chung
            var vm = AutoViewModel.Instance;
            lstSteps.ItemsSource = vm.ScriptSteps;

            // Xử lý nút Thêm
            btnAdd.Click += (s, e) => {
                if (int.TryParse(txtX.Text, out int x) &&
                    int.TryParse(txtY.Text, out int y) &&
                    int.TryParse(txtDelay.Text, out int delay))
                {
                    vm.ScriptSteps.Add(new ClickStep { X = x, Y = y, DelayMs = delay });
                }
                else
                {
                    MessageBox.Show("Vui lòng nhập số hợp lệ!");
                }
            };

            // Xử lý nút Xóa
            btnClear.Click += (s, e) => {
                vm.ScriptSteps.Clear();
            };
        }
    }
}