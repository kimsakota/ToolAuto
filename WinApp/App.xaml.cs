using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Vst.Controls
{
    public static class VstControlExtention
    {
        static public void Request(this FrameworkElement element, string url, params object[] args)
        {
            WinApp.App.Request(url, args);
        }
    }
}

namespace WinApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static public void Request(string url, params object[] args)
        {
            if (url[0] == '/')
            {
                url = System.Mvc.Engine.RequestContext.ControllerName + url;
            }
            System.Mvc.Engine.Execute(url, args);
        }
        static public void ShowError(string content)
        {
            MessageBox.Show("     " + content + "     ", "Error");
        }
        static Vst.MQTT.Client _mqtt;
        static public Vst.MQTT.Client Mqtt
        {
            get
            {
                if (_mqtt == null)
                {
                    string host = "system.aks.vn";
                    _mqtt = new Vst.MQTT.Client(host);

                    _mqtt.Connected += () => {
                        _mqtt.Subscribe($"response/{_mqtt.ID}");
                    };
                    _mqtt.ConnectionLost += () => {

                    };
                    _mqtt.ConnectionError += () => {

                    };
                    _mqtt.DataReceived += (t, p) => {
                        try
                        {
                            var context = Document.Parse(p.UTF8());
                            var url = context.Url;
                            if (context.Code != 0)
                            {
                                ShowError(url);
                                return;
                            }

                            Request(url, context.ValueContext);
                        }
                        catch
                        {

                        }
                    };
                }
                return _mqtt;
            }
        }
        static public Actors.User User { get; set; }
        static public MainWindow Browser { get; set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            //Mqtt.Connect();

            base.OnStartup(e);

            DB.Start(Environment.CurrentDirectory + "/App_Data");
            Vst.Controls.SvgIcon.Register(DB.Main.DataPath("Svg"));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            DB.End();
            base.OnExit(e);
        }
    }
}
