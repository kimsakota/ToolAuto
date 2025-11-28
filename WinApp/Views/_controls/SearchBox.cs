using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Vst.Controls
{
    public class SearchBox : GridLayout
    {
        public Action Cleared { get; set; }
        public Action<string> Enter { get; set; }
        public Action<string> Changed { get; set; }

        TextBox textBox = new TextBox();
        
        protected virtual void RaiseChanged()
        {
        }
        public string Comment
        {
            get => textBox.Placeholder;
            set => textBox.Placeholder = value;
        }
        public SearchBox()
        {
            textBox.Frame.BorderThickness = new System.Windows.Thickness(0);
            textBox.Placeholder = "search ...";

            textBox.ValueChanged += (_, __) => {
                string s = textBox.Text;
                if (s == string.Empty)
                {
                    Cleared?.Invoke();
                }
                else
                {
                    Changed?.Invoke(s);
                }
            };
            this.PreviewKeyDown += (s, e) => {
                if (e.Key == Key.Enter)
                {
                    e.Handled = true;

                    var v = textBox.Text;
                    if (v != string.Empty)
                    {
                        Enter?.Invoke(v);
                    }
                }
            };

            Children.Add(new SvgIcon { Source = "search", Stroke = Brushes.Gray });
            Children.Add(textBox);
            
            this.Split(1, 2);
            this.Columns[0].Width = new System.Windows.GridLength(40);
        }
    }

}
