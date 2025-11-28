using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VSI = Vst.Controls.SvgIcon;

namespace WinApp.Views
{
    /// <summary>
    /// Interaction logic for ScriptToolbar.xaml
    /// </summary>
    public partial class ScriptToolbar : UserControl, IDisposable
    {
        public ScriptToolbar()
        {
            InitializeComponent();
            var map = new Dictionary<string, VSI>();
            var script = new Models.MouseScript();

            script.StateChanged += () => {
                Dispatcher.Invoke(() =>
                {
                    if (script.IsPlaying || script.IsRecording)
                    {
                        buttonsPanel.Visibility = Visibility.Hidden;
                        indicator.Visibility = Visibility.Visible;

                        var animation = new DoubleAnimation
                        {
                            From = 1,
                            To = 0,
                            AutoReverse = true,
                            Duration = TimeSpan.FromMilliseconds(500),
                            RepeatBehavior = RepeatBehavior.Forever,
                        };
                        indicator.BeginAnimation(OpacityProperty, animation);
                        indicator.Background = script.IsPlaying ? Brushes.Green : Brushes.Red;
                    }
                    else
                    {
                        buttonsPanel.Visibility = Visibility.Visible;
                        indicator.Visibility = Visibility.Collapsed;
                    }
                });
            };
            DataContext = script;

            foreach (var e in buttonsPanel.Children)
            {
                if (e is VSI icon)
                {
                    map.Add(icon.Source, icon);
                    icon.RegisterClickEvent(() => { 
                        switch (icon.Source)
                        {
                            case "open":
                                var openDialog = new Microsoft.Win32.OpenFileDialog {
                                    Filter = "Json Document|*.json|All Files|*.*",
                                };
                                if (openDialog.ShowDialog() == true)
                                {
                                    script.Open(openDialog.FileName);
                                }
                                return;

                            case "save":
                                var saveDialog = new Microsoft.Win32.SaveFileDialog {
                                    Filter = "Json Document|*.json|All Files|*.*",
                                };
                                if (saveDialog.ShowDialog() == true)
                                {
                                    script.Save(saveDialog.FileName);
                                }

                                break;

                            case "play": script.Play(); return;
                            case "stop": script.EndPlay(); return;
                            case "record": script.BeginRecord(); return;
                            case "end-record": script.EndRecord(); return;
                        }
                    });
                }
            }
        }

        public void Dispose()
        {
            var s = (Models.MouseScript)DataContext;
            s.EndPlay();
            s.EndRecord();
        }
    }
}
