using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.TextFormatting;
using WinApp;

namespace Models
{
    public class MouseRecord
    {
        public const uint LBUTTON_DOWN = 0x0002;   // Nhấn chuột trái
        public const uint LBUTTON_UP = 0x0004;     // Thả chuột trái
        public const uint RBUTTON_DOWN = 0x0008;  // Nhấn chuột phải
        public const uint RBUTTON_UP = 0x0010;    // Thả chuột phải

        public const uint MOVE = 0x0001;        // Di chuyển chuột
        public const uint WHEEL = 0x0800;       // Cờ cho sự kiện lăn chuột

        public double Before { get; set; }
        public long Flags { get; set; }
        public long Params { get; set; }
        public long Ext { get; set; }
        public long X { get; set; }
        public long Y { get; set; }
    }
    public class MouseScript : LinkedList<MouseRecord>
    {
        // HOOK
        static IKeyboardMouseEvents _hook;
        public void StartHook(Action<IKeyboardMouseEvents> callback)
        {
            if (_hook != null)
                return;

            // Đăng ký hook cho toàn bộ hệ thống
            _hook = Hook.GlobalEvents();

            callback(_hook);
        }
        public void StopHook()
        {
            if (_hook != null)
            {
                _hook.Dispose();
                _hook = null;
            }
        }

        static public MouseRecord CreateButtonEvent(MouseEventArgs e, bool up = false)
        {
            uint fla = (uint)(e.Button == MouseButtons.Left ? 2 : 8);
            if (up) fla <<= 1;

            return new MouseRecord
            {
                X = e.X,
                Y = e.Y,
                Flags = fla,
            };
        }
        public static MouseRecord CreateWheelEvent(MouseEventArgs e)
        {
            return new MouseRecord
            {
                X = e.X,
                Y = e.Y,
                Params = e.Delta,
                Flags = MouseRecord.WHEEL,
            };
        }
        public static MouseRecord CreateMovingEvent(MouseEventArgs e)
        {
            return new MouseRecord {
                X = e.X,
                Y = e.Y,
                Flags = 0,
            };
        }

        // RUN
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);
        static uint abs(uint f) => ABSOLUTE | f;
        private const uint ABSOLUTE = 0x8000; // Tọa độ tuyệt đối (0 đến 65535)

        public void SendEvent(MouseRecord r)
        {
            System.Threading.Thread.Sleep((int)r.Before);

            if (r.X != 0 && r.Y != 0)
            {
                int screenWidth = SystemInformation.VirtualScreen.Width;
                int screenHeight = SystemInformation.VirtualScreen.Height;

                uint absoluteX = (uint)(r.X * 65535 / screenWidth);
                uint absoluteY = (uint)(r.Y * 65535 / screenHeight);

                mouse_event(abs(MouseRecord.MOVE), absoluteX, absoluteY, 0, 0);
            }
            
            if (r.Flags != 0)
            {
                mouse_event(abs((uint)r.Flags), 0, 0, (uint)r.Params, (int)r.Ext);
            }
        }

        // RECORDING
        bool _recording;
        bool _playing;
        public bool IsRecording 
        {
            get => _recording;
            private set
            {
                if (_recording != value)
                {
                    _recording = value;
                    StateChanged?.Invoke();
                }
            }
        }
        public bool IsPlaying
        {
            get => _playing;
            private set
            {
                if (_playing != value)
                {
                    _playing = value;
                    StateChanged?.Invoke();
                }
            }
        }

        public event Action StateChanged;
        public void SwitchRecord()
        {
            if (IsRecording)
            {
                EndRecord();
            }
            else
            {
                BeginRecord();
            }
        }

        static DateTime? _last_record_time;
        public void BeginRecord()
        {
            var down = false;

            _last_record_time = DateTime.Now;
            StartHook(listener => {
                this.Clear();

                listener.MouseDown += (s, e) => {
                    Add(CreateButtonEvent(e, false));
                };
                listener.MouseUp += (s, e) => {
                    Add(CreateButtonEvent(e, true));
                    down = false;
                };
                listener.MouseMove += (s, e) => {
                    if (down) Add(CreateMovingEvent(e));
                };
                listener.MouseWheel += (s, e) => {
                    Add(CreateWheelEvent(e));
                };

                listener.KeyUp += (s, e) => { 
                    if (e.Control && e.KeyCode == System.Windows.Forms.Keys.S)
                    {
                        EndRecord();
                    }
                };
            });

            IsRecording = true;

        }
        public void EndRecord()
        {
            IsRecording = false;
            StopHook();
        }

        public void SwitchPlay()
        {
            if (IsPlaying)
            {
                EndPlay();
            }
            else
            {
                Play();
            }
        }
        public void Play()
        {
            if (this.Count == 0)
                return;

            StartHook(listener => {
                listener.KeyUp += (s, e) => {
                    if (e.Control && e.KeyCode == Keys.S)
                    {
                        EndPlay();
                    }
                };
            });

            IsPlaying = true;

            Task.Run(() => {
                var node = this.First;
                while (node != null && IsPlaying)
                {
                    SendEvent(node.Value);

                    var next = node.Next;
                    if (next == null)
                    {
                        EndPlay();
                        return;
                    }

                    node = next;
                }
            });
        }
        public void EndPlay()
        {
            StopHook();
            IsPlaying = false;
        }

        void Add(MouseRecord one)
        {
            one.Before = _last_record_time == null ? 0 
                : (DateTime.Now - _last_record_time.Value).TotalMilliseconds;
            _last_record_time = DateTime.Now;

            AddLast(one);
        }

        public void Open(string path)
        {
            try
            {
                this.Clear();

                var doc = Document.Parse(File.ReadAllText(path));
                foreach (Document e in doc.ValueList)
                {
                    this.AddLast(JObject.ToObject<MouseRecord>(e));
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void Save(string path)
        {
            var lst = new DocumentList();
            foreach (var item in this)
            {
                lst.Add(Document.FromObject(item));
            }
            var content = new Document { Value = lst };
            File.WriteAllText(path, content.ToString());
        }
    }
}
