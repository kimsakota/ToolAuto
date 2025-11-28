using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Vst.Controls
{
    public class InlineBlockView<TItemView> : WrapPanel
        where TItemView: FrameworkElement, new()
    {
        public string Url { get; set; } = "/open";
        public event Action<object> ItemClick;
        public InlineBlockView()
        {
            DataContextChanged += (_, __) => { 
                if (DataContext is ListContext context)
                {
                    foreach (var one in context.ValueList)
                    {
                        var e = new TItemView();
                        e.DataContext = one;

                        e.RegisterClickEvent(() => {
                            ItemClick?.Invoke(one);
                            if (Url != null)
                            {
                                this.Request(Url, one);
                            }
                        });

                        Children.Add(e);
                    }
                }
            };
        }
    }
}
