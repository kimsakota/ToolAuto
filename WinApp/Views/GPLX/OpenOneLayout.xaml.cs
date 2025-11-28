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
using Vst.Controls;

namespace WinApp.Views.GPLX
{
    /// <summary>
    /// Interaction logic for OpenOneLayout.xaml
    /// </summary>
    /// 
    public class PropertyGrid : TableView
    {
        protected override Vst.Controls.TableColumn[] GetRenderColumns()
        {
            RowIndexWidth = 0;
            return Columns.ToArray();
        }
        protected override void RaiseOpenOne(int index)
        {
            this.GetRow(index).GetCell(0).Background = Brushes.Black;
            base.RaiseOpenOne(index);
        }
        public PropertyGrid()
        {
            this.Columns.Add(new Vst.Controls.TableColumn { 
                Width = 12,
                Background = Brushes.WhiteSmoke,
            });
            this.Columns.Add(new Vst.Controls.TableColumn { 
                Caption = "Tên trường", 
                Name = "caption",
            });
            this.Columns.Add(new Vst.Controls.TableColumn { Caption = "Giá trị", Name = "value" });

            this.OpenItem += e => {

                var doc = (Document)e;

                var s = (string)doc.Value;
                System.Windows.Forms.SendKeys.SendWait(s);
            };
        }
    }
    public partial class OpenOneLayout : UserControl
    {
        public PropertyGrid PropertyGrid { get; private set; } = new PropertyGrid();
        public OpenOneLayout()
        {
            InitializeComponent();

            ((Grid)this.Content).Children.Add(PropertyGrid);

        }
    }
}
