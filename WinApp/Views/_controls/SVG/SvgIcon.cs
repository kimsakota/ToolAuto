using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Vst.Controls.SVG;

using VS = Vst.Controls.SVG;

namespace Vst.Controls
{
    public class SvgCanvas : Viewbox, VS.IAddChild
    {
        Canvas _canvas;

        public SvgCanvas()
        {
            _canvas = new Canvas {
            };

            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;
            Stretch = Stretch.Uniform;

            Child = _canvas;
        }

        public void AppendChild(object child) => _canvas.Children.Add((UIElement)child);
        public void Clear() => _canvas.Children.Clear();
        protected void SetAttribute(DependencyProperty property, object value)
        {
            foreach (UIElement e in _canvas.Children)
            {
                if (e is Shape)
                {
                    e.SetValue(property, value);
                }
                else if (e is SvgCanvas) 
                {
                    ((SvgCanvas)e).SetAttribute(property, value);
                }
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            switch (e.Property.Name)
            {
                case nameof(Fill):
                    _canvas.SetAttribute(Shape.FillProperty, e.NewValue);
                    return;

                case nameof(Stroke):
                    _canvas.SetAttribute(Shape.StrokeProperty, e.NewValue);
                    return;
            }
            base.OnPropertyChanged(e);
        }

        public Brush Fill { get => (Brush)GetValue(Shape.FillProperty); set => SetValue(Shape.FillProperty, value); }
        public Brush Stroke { get => (Brush)GetValue(Shape.StrokeProperty); set => SetValue(Shape.StrokeProperty, value); }
    }
    public class SvgIcon : SvgCanvas
    {
        static public VS.ElementCollection Resource { get; private set; } = new VS.ElementCollection();
        static public void Register(string path)
        {
            foreach (var name in System.IO.Directory.GetFiles(path))
            {
                var names = System.IO.Path.GetFileName(name).ToLower().Split('.');
                var k = names[0];

                switch (names[1])
                {
                    case "json":
                        var context = Document.Parse(System.IO.File.ReadAllText(name));
                        Resource.Add(k, new Element(context));
                        break;
                    case "svg":
                        var doc = new System.Xml.XmlDocument();
                        doc.Load(name);

                        Resource.Add(k, new Element(doc.DocumentElement));
                        break;

                }
            }
        }

        static readonly ShapeCreator _shapes = new ShapeCreator {
            { "svg", typeof(SvgCanvas) },
            { "g", typeof(SvgCanvas) },
            { "polyline", typeof(Polyline) },
            { "polygon", typeof(Polygon) },
            { "line", typeof(Line) },
            { "path", typeof(Path) },
            { "rect", typeof(Rectangle) },
            { "circle", typeof(Ellipse) },
            { "ellipse", typeof(Ellipse) },
        };

        static readonly AttributeConverter _attributes = new AttributeConverter {
            { typeof(ColorAttribute), v => v.Equals("none") ? null : new BrushConverter().ConvertFromString((string)v) },
            { typeof(PointsAttribute), GetPoints },
            { typeof(DataPathAttribute), GetPathData },
            { typeof(ViewBoxAttribute), GetViewBox },
        };

        static object GetViewBox(object value)
        {
            var v = (double[])value;
            return new Rect(0, 0, v[2], v[3]);
        }
        static object GetPathData(object value)
        {
            return Geometry.Parse((string)value);
        }
        static object GetPoints(object value)
        {
            var pts = new PointCollection();
            var v = (double[])value;

            int i = 0;
            while (i < v.Length)
            {
                var x = v[i++];
                var y = v[i++];
                pts.Add(new Point(x, y));
            }
            return pts;
        }
        public SvgIcon()
        {
            Width = Height = 24;
        }

        static public readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached(
                nameof(Source), typeof(string), typeof(SvgIcon),
                new PropertyMetadata(string.Empty, OnSourceChanged));

        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var k = ((string)e.NewValue).ToLower();
            if (Resource.TryGetValue(k, out var element))
            {
                ((SvgIcon)d).LoadElement(element);
            }
        }
        protected virtual void LoadElement(VS.Element element)
        {
            try
            {
                Clear();
                element.Attributes.GetAttribute<ViewBoxAttribute>("viewBox", a => {
                    var c = (Canvas)Child;
                    c.Width = a.Width;
                    c.Height = a.Height;
                });
                Dispatcher.InvokeAsync(() => {
                    element.Render(_shapes, _attributes, this);
                    SetAttribute(Shape.FillProperty, Fill);
                    SetAttribute(Shape.StrokeProperty, Stroke);
                });
            }
            catch
            {
            }
        }
    }
}
