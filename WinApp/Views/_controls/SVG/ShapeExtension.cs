using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Vst.Controls.SVG
{
    public static class ShapeExtension
    {
        static public void SetAttribute(this Panel layout, DependencyProperty property, object value)
        {
            foreach (UIElement e in layout.Children)
            {
                if (e is Panel p)
                {
                    SetAttribute(p, property, value);
                }
                else
                {
                    e.SetValue(property, value);
                }
            }
        }
    }
}
