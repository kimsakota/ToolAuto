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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WinApp.Views.GPLX
{
    /// <summary>
    /// Interaction logic for GplxItemView.xaml
    /// </summary>
    public partial class GplxItemView : UserControl
    {
        public GplxItemView()
        {
            InitializeComponent();

            DataContextChanged += (_, __) => { 
                if (DataContext is Document e)
                {
                    var v = e.Time;
                    if (v != null)
                    {
                        time.Visibility = Visibility.Visible;
                        time.Content = v.Value.ToString("dd/MM/yyyy HH:mm");
                    }
                }
            };
        }
    }
}
