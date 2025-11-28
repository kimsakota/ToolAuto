using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Vst.Controls;
using WinApp.ViewModels;
using VstButton = Vst.Controls.Button;

namespace WinApp.Views.GPLX
{
    class ListView : MyScrollViewer
    {
        public ListView()
        {
            Background = Brushes.White;
            DataContextChanged += (_, __) => { 
                if (DataContext is DocumentList lst)
                {
                    foreach (var e in lst)
                    {
                        var item = new GplxItemView { 
                            DataContext = e
                        };
                        item.RegisterClickEvent(() => {
                            App.Request("/open", e);
                        });
                        this.Children.Add(item);
                    }
                }
            };
        }
    }
    class Index : BaseView<ListView>
    {
        protected override void RenderCore()
        {
            base.RenderCore();
            App.Browser
                .SetSummary($"Số lượng: {ViewModel.Documents.Count}")
                .Maximize(false);
        }
    }

    class Saved : Index { }

    class Add : BaseView<TemplateTableView, ViewModels.GPLXListContext>
    {
        protected override void RenderCore()
        {
            var text = Clipboard.GetText();
            var lst = ViewModel.Import(text);

            App.Browser.Maximize(true);
            if (lst.Count == 0)
            {
            }
            else
            {
                var accept = new VstButton { Text = "Xác nhận" };
                accept.Click += (_, __) => {
                    App.Request("/import", lst);
                };

                App.Browser
                    .ClearActions()
                    .AddActionButton(new VstButton { Text = "Hủy", Background = Brushes.Black, Url = "/index" })
                    .AddActionButton(accept);
            }

            App.Browser.SetSummary($"Số lượng: {lst.Count}");
            base.RenderCore();
        }
    }

    class Open : BaseView<OpenOneLayout>
    {
        protected override void RenderCore()
        {
            var lst = new DocumentList();
            var context = new GPLXListContext();

            App.Browser.SetCanFocus(false);
            var data = (Document)ViewModel.Value;

            foreach (var col in context.Columns)
            {
                var v = data.GetString(col.Name);
                if (string.IsNullOrWhiteSpace(v))
                    v = col.GetString("default");

                if (v == null)
                {
                    MessageBox.Show("Bản ghi bị lỗi");
                    return;
                }
                lst.Add(new Document {
                    Caption = col.Caption,
                    Value = v
                });
            }
            
            MainView.PropertyGrid.ItemsSource = lst;
        }
    }
}
