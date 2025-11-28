//using System;
//using System.Windows.Forms;

//namespace WinApp
//{
//    using System.Collections.Generic;
//    using System.Windows;
//    using System.Runtime.InteropServices;
//    using Gma.System.MouseKeyHook;
//    using Models;

//    public interface IMouseHookEvent
//    {
//        void Run();
//    }
//    public class MouseHook
//    {
//        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);
//        static uint abs(uint f) => ABSOLUTE | f;

//        // Định nghĩa các hằng số sự kiện chuột
//        private const uint LEFTDOWN = 0x0002;   // Nhấn chuột trái
//        private const uint LEFTUP = 0x0004;     // Thả chuột trái
//        private const uint RIGHTDOWN = 0x0008;  // Nhấn chuột phải
//        private const uint RIGHTUP = 0x0010;    // Thả chuột phải

//        private const uint MOVE = 0x0001;     // Di chuyển chuột
//        private const uint ABSOLUTE = 0x8000; // Tọa độ tuyệt đối (0 đến 65535)
//        private const uint WHEEL = 0x0800; // Cờ cho sự kiện lăn chuột

//        static IKeyboardMouseEvents _hook;

//        class MoveEvent : IMouseHookEvent
//        {
//            public int X { get; set; }
//            public int Y { get; set; }
//            public virtual void Run()
//            {
//                if (X != 0 && Y != 0)
//                    MoveTo(X, Y);
//            }
//        }
//        class WheelEvent : MoveEvent
//        {
//            public int Delta { get; set; }
//            public override void Run()
//            {
//                base.Run();
//                Wheel(Delta);
//            }
//        }
//        class ButtonEvent : MoveEvent
//        {
//            public uint Flags { get; set; }           
//            public override void Run()
//            {
//                base.Run();
//                if (Flags != 0)
//                    mouse_event(abs(Flags), 0, 0, 0, 0);
//            }
//        }

//        static public MouseRecord CreateButtonEvent(MouseEventArgs e, bool up = false)
//        {
//            int fla = e.Button == MouseButtons.Left ? 2 : 8;
//            if (up) fla <<= 1;

//            return new MouseRecord {
//                X = e.X,
//                Y = e.Y,
//                Flags = (uint)fla,
//            };
//        }
//        public static MouseRecord CreateWheelEvent(MouseEventArgs e)
//        {
//            return new MouseRecord {
//                X = e.X,
//                Y = e.Y,
//                Params = e.Delta
//            };
//        }
//        public static IMouseHookEvent CreateMovingEvent(MouseEventArgs e)
//        {
//            return new ButtonEvent {
//                X = e.X,
//                Y = e.Y,
//                Flags = 0,
//            };
//        }

//        public static void Start(Action<IKeyboardMouseEvents> callback)
//        {
//            if (_hook != null)
//                return;

//            // Đăng ký hook cho toàn bộ hệ thống
//            _hook = Hook.GlobalEvents();

//            callback(_hook);
//        }
//        // Quan trọng: Hủy đăng ký hook khi ứng dụng đóng để tránh rò rỉ bộ nhớ (memory leaks)
//        public static void Stop()
//        {
//            if (_hook != null)
//            {
//                _hook.Dispose();
//                _hook = null;
//            }
//        }


//        // Hàm lăn chuột
//        public static void Wheel(int delta, int times = 1)
//        {
//            mouse_event(WHEEL, 0, 0, (uint)(delta * times), 0);
//        }

//        // Hàm di chuyển chuột đến vị trí màn hình (X, Y)
//        public static void MoveTo(int x, int y)
//        {
//            // Chuyển đổi tọa độ pixel thành tọa độ tuyệt đối (0-65535)
//            int screenWidth = SystemInformation.VirtualScreen.Width;
//            int screenHeight = SystemInformation.VirtualScreen.Height;

//            uint absoluteX = (uint)(x * 65535 / screenWidth);
//            uint absoluteY = (uint)(y * 65535 / screenHeight);

//            mouse_event(abs(MOVE), absoluteX, absoluteY, 0, 0);
//        }

//        // Hàm xử lý chuột trái
//        public static void LeftDown() => mouse_event(abs(LEFTDOWN), 0, 0, 0, 0);
//        public static void LeftUp() => mouse_event(abs(LEFTUP), 0, 0, 0, 0);
//        public static void LeftClick()
//        {
//            LeftUp();
//            LeftDown();
//        }

//        // Hàm xử lý chuột phải
//        public static void RightDown() => mouse_event(abs(RIGHTDOWN), 0, 0, 0, 0);
//        public static void RightUp() => mouse_event(abs(RIGHTUP), 0, 0, 0, 0);
//        public static void RightClick()
//        {
//            RightUp();
//            RightDown();
//        }
//    }
//}

//namespace Vst.Controls
//{
//    using System.Runtime.InteropServices;
//    using System.Diagnostics;
//    public class MouseHook
//    {
//        // Định nghĩa các hằng số và cấu trúc cần thiết
//        private const int WH_MOUSE_LL = 14; // Hook loại Low-Level Mouse
//        private const int WM_LBUTTONDOWN = 0x0201; // Mã thông điệp nút chuột trái nhấn xuống
//        private const int WM_RBUTTONDOWN = 0x0204; // Mã thông điệp nút chuột phải nhấn xuống

//        // Cấu trúc chứa thông tin về sự kiện chuột
//        [StructLayout(LayoutKind.Sequential)]
//        private struct MSLLHOOKSTRUCT
//        {
//            public System.Drawing.Point pt;
//            public uint mouseData;
//            public uint flags;
//            public uint time;
//            public IntPtr dwExtraInfo;
//        }

//        // Delegate cho Hook Procedure
//        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

//        // Imports các hàm API từ user32.dll và kernel32.dll
//        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

//        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

//        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

//        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//        private static extern IntPtr GetModuleHandle(string lpModuleName);

//        private IntPtr _hookHandle = IntPtr.Zero;
//        private HookProc _hookProc = null;

//        public void StartHook()
//        {
//            _hookProc = new HookProc(HookCallback);
//            using (Process curProcess = Process.GetCurrentProcess())
//            using (ProcessModule curModule = curProcess.MainModule)
//            {
//                // Cài đặt hook toàn hệ thống (dwThreadId = 0)
//                // Lấy handle của module hiện tại
//                _hookHandle = SetWindowsHookEx(WH_MOUSE_LL, _hookProc, GetModuleHandle(curModule.ModuleName), 0);

//                if (_hookHandle == IntPtr.Zero)
//                {
//                    // Xử lý lỗi nếu cài đặt hook thất bại
//                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
//                }
//            }
//        }

//        public void StopHook()
//        {
//            if (_hookHandle != IntPtr.Zero)
//            {
//                UnhookWindowsHookEx(_hookHandle);
//                _hookHandle = IntPtr.Zero;
//            }
//        }

//        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
//        {
//            // nCode >= 0 nghĩa là thông điệp hợp lệ
//            if (nCode >= 0)
//            {
//                // Kiểm tra loại sự kiện chuột
//                if (wParam == (IntPtr)WM_LBUTTONDOWN)
//                {
//                    // marshal lParam thành cấu trúc MSLLHOOKSTRUCT để lấy tọa độ
//                    MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
//                    Console.WriteLine($"Left Mouse Down at: X={hookStruct.pt.X}, Y={hookStruct.pt.Y}");
//                }
//                else if (wParam == (IntPtr)WM_RBUTTONDOWN)
//                {
//                    // Xử lý nút chuột phải nhấn xuống
//                }
//            }

//            // Luôn gọi hàm CallNextHookEx để chuyển sự kiện cho các hook khác trong chuỗi
//            return CallNextHookEx(_hookHandle, nCode, wParam, lParam);
//        }
//    }
//}