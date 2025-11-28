using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace System.Windows.Controls
{
    public static class ControlExtention
    {
        static public void Render(this FrameworkElement element, Document context)
        {
            var type = element.GetType();
            foreach (var p in context)
            {
                var prop = type.GetProperty(p.Key);
                if (prop != null)
                {
                    try
                    {
                        prop.SetValue(element, p.Value);
                    }
                    catch
                    {

                    }
                }
            }

        }
        static public void RegisterClickEvent(this FrameworkElement element, Action clicked)
        {
            bool down = false;
            element.Cursor = Input.Cursors.Hand;
            element.MouseLeftButtonDown += (s, e) => down = true;
            element.MouseLeftButtonUp += (s, e) => {
                if (down)
                {
                    down = false;

                    if (element.IsEnabled)
                        clicked();
                }
            };
            element.MouseLeave += (s, e) => down = false;
        }
    }
}
