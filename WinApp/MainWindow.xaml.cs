using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vst.Controls;

namespace WinApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public void ShowMainContent(bool show)
        {
            Dispatcher.InvokeAsync(() => {
                userForm.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
                loginForm.Visibility = show ? Visibility.Collapsed : Visibility.Visible;
            });
        }
        public MainWindow AddActionButton(FrameworkElement button)
        {
            this.ActionPanel.Children.Insert(0, button);
            return this;
        }
        public MainWindow ClearActions()
        {
            this.ActionPanel.Children.Clear();
            return this;
        }
        public MainWindow SetSummary(string text)
        {
            Summary.Text = text;
            return this;
        }
        public MainWindow Maximize(bool b)
        {
            this.WindowState = b ? WindowState.Maximized : WindowState.Normal;
            return this;
        }
        public MainWindow()
        {

            TabbarItem.ActiveColor = (SolidColorBrush)App.Current.Resources["PrimaryBrush"];
            App.Browser = this;

            InitializeComponent();

            //searchBox.Render(new ViewModels.SearchContext(null, s => MessageBox.Show(s) , null, null));

            loginForm.DataContext = new ViewModels.LoginViewModel();
            System.Mvc.Engine.Register(this, result => {
                ShowMainContent(true);

                var view = result.View;
                if (view != null)
                {
                    Dispatcher.Invoke(() => {
                        view.Render(result.Model);

                        if (view.Content is FrameworkElement e)
                        {
                            mainContent.Content = e;
                        }
                    });
                }
            });

            Closing += (s, e) => {
                ScriptTools.Dispose();
            };

            if (loginForm.Visibility != Visibility.Visible)
            {
                App.Request("home");
            }    
        }
    }
}
