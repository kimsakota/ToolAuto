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

namespace WinApp.Views
{
    /// <summary>
    /// Interaction logic for FlyoutPanel.xaml
    /// </summary>
    public partial class FlyoutPanel : UserControl
    {
        public FlyoutPanel()
        {
            InitializeComponent();

            double flyoutWidth = flyoutPanel.Width;
            flyoutButton.RegisterClickEvent(() => {
                var from = flyoutPanel.Width == 0 ? 0 : flyoutWidth;
                var to = flyoutPanel.Width == 0 ? flyoutWidth : 0;
                var animation = new DoubleAnimation
                {
                    From = from,
                    To = to,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase()
                };

                flyoutPanel.BeginAnimation(WidthProperty, animation);
            });

        }
    }
}
