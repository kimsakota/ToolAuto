using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;

namespace Vst.Controls
{
    public class MyForm : GridLayout
    {
        protected HeaderText _header = new HeaderText { 
            HeaderSize = 4
        };
        protected EditorPanel _editorPanel = new EditorPanel { };
        protected StackPanel _actionPanel = new StackPanel {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        protected List<EditorInfo> _error;

        protected Button _submitButton = new Button { 
            Text = "SUBMIT",
            Padding = new Thickness(10),
        };

        protected override Size MeasureOverride(Size constraint)
        {
            _submitButton.Width = constraint.Width;
            return base.MeasureOverride(constraint);
        }

        public string Caption
        {
            get => _header.Text;
            set => _header.Text = value;
        }
        public MyForm()
        {
            panel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            panel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            panel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            Add(_header, 0, 0);
            Add(_editorPanel, 1, 0);
            Add(_actionPanel, 2, 0);

            _actionPanel.Children.Add(_submitButton);

            _submitButton.Click += (_, __) => {
                _error = null;
                if (_editorPanel.CheckError(lst => _error = lst))
                {
                    MessageBox.Show("Các trường bắt buộc không được bỏ trống", "Lỗi cập nhật");
                }
            };
        }
        public MyForm Add(EditorInfo context)
        {
            _editorPanel.Add(context);
            return this;
        }

        public FormControl this[string name] => _editorPanel.Find(name);
    }

    public class MyTemplateForm : MyForm
    {
        public MyTemplateForm()
        {
            DataContextChanged += (_, __) => {
                if (DataContext is EditContext context)
                {
                    _header.Text = context.Caption;
                    foreach (var i in context.Editors)
                    {
                        var e = i.ChangeType<EditorInfo>();

                        Add(e);
                        e.Control.Value = e.Value;
                    }

                    var model = context.ValueContext;
                    if (model != null)
                    {
                        _editorPanel.ForEach((f, c) => {
                            var k = f.EditorInfo.Name;
                            if (model.TryGetValue(k, out object v))
                            {
                                c.Value = v;
                            }
                        });
                    }
                    else
                    {
                        context.Value = model = new Document();
                    }

                    _submitButton.Click += (___, ____) => {
                        if (_error != null) return;
                        _editorPanel.ForEach((f, c) => {
                            var k = f.EditorInfo.Name;
                            var v = c.Value;
                            model.Push(k, v);
                        });

                        context.AcceptCallback?.Invoke(model);
                        this.Request(context.Url ?? "/update", context);
                    };
                }
            };
        }
    }
}
