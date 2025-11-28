using System;
using WinApp.Views; // Để sử dụng BaseView từ namespace cha

namespace WinApp.Views.Auto
{
    // 1. Ánh xạ cho màn hình Chạy (OCR)
    // Bỏ "public" để khớp với BaseView (internal)
    class Index : BaseView<RunView>
    {
    }

    // 2. Ánh xạ cho màn hình Cài đặt (Macro)
    // Bỏ "public" để khớp với BaseView (internal)
    class Setting : BaseView<SettingView>
    {
    }
}