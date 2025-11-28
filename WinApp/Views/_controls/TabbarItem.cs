using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Vst.Controls
{
    public class TabbarItem : MyPanel<StackPanel>
    {
        static public Brush ActiveColor { get; set; }
        static public Brush NormalColor { get; set; } = Brushes.Gray;
        static TabbarItem _current;

        SvgIcon _icon = new SvgIcon();
        TextBlock _text = new TextBlock { 
            TextAlignment = System.Windows.TextAlignment.Center,
            TextWrapping = System.Windows.TextWrapping.Wrap,
        };
        public string Source
        {
            get => _icon.Source;
            set => _icon.Source = value;
        }
        public string Text
        {
            get => _text.Text;
            set => _text.Text = value;
        }
        public string Url { get; set; }
        public bool IsActivated
        {
            get => _text.Foreground == ActiveColor;
            set
            {
                if (_current == this)
                    return;

                var color = NormalColor;
                if (value)
                {
                    var old = _current;      
                    _current = this;
                    if (old != null)
                    {
                        old.IsActivated = false;
                    }
                    color = ActiveColor;
                }
                else
                {
                }

                _icon.Stroke = color;
                _text.Foreground = color;
            }
        }

        public TabbarItem()
        {
            Children.Add(_icon);
            Children.Add(_text);

            IsActivated = false;

            this.MouseMove += (s, e) => {
                _icon.Stroke = ActiveColor;
                Background = Brushes.White;
            };
            this.MouseLeave += (s, e) => {
                if (!IsActivated)
                    _icon.Stroke = NormalColor;
                Background = Brushes.Transparent;
            };
            this.RegisterClickEvent(() => {
                IsActivated = true;
                System.Mvc.Engine.Execute(Url);
            });
        }
    }
}
