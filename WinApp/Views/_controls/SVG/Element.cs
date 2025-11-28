using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Vst.Controls.SVG
{
    public interface IAddChild
    {
        void AppendChild(object child);
    }
    public class ElementCollection : Dictionary<string, Element>
    {
    }
    public class ShapeCreator : Dictionary<string, Type>
    {
    }
    public class AttributeConverter : Dictionary<Type, Func<object, object>>
    {
    }
    public class Element
    {
        static AttributeCreator _attributeCreator = new AttributeCreator();

        public string Name { get; set; } = string.Empty;
        public AttributeCollection Attributes { get; private set; } = new AttributeCollection();

        public void ScaleTransform(double scale)
        {
            foreach (var attribute in Attributes.Values)
            {
                attribute.ScaleTransform(scale);
            }
            if (HasChildren)
            {
                foreach (var e in Children)
                { 
                    e.ScaleTransform(scale); 
                }
            }
        }

        List<Element> _children;
        public List<Element> Children
        {
            get
            {
                if (_children == null)
                    _children = new List<Element>();
                return _children;
            }
        }
        public bool HasChildren => _children?.Count > 0;

        public void Render(ShapeCreator shapeCreator,
            AttributeConverter attributeConverter, object target)
        {
            var type = target.GetType();
            foreach (var a in Attributes)
            {
                var name = a.Value.GetShapeName();
                var p = type.GetProperty(name);
                if (p == null) continue;

                object v = a.Value.GetShapeAttributeValue();
                if (v.GetType() == p.PropertyType)
                {
                    p.SetValue(target, v, null);
                    continue;
                }

                var atype = a.Value.GetType();
                if (attributeConverter.TryGetValue(atype, out var attrConvert))
                {
                    v = attrConvert(v);
                }
                try
                {
                    p.SetValue(target, v, null);
                    continue;
                }
                catch
                {
                }
                try
                {
                    v = System.Convert.ChangeType(v, p.PropertyType);
                    p.SetValue(target, v, null);
                }
                catch
                {
                }
            }

            if (target is IAddChild && HasChildren)
            {
                foreach (var child in Children)
                {
                    shapeCreator.TryGetValue(child.Name, out type);
                    if (type == null) continue;

                    var s = Activator.CreateInstance(type);
                    if (s != null)
                    {
                        child.Render(shapeCreator, attributeConverter, s);
                        ((IAddChild)target).AppendChild(s);
                    }
                }
            }
        }

        public Element() { }
        public Element(XmlNode xml)
        {
            Name = xml.Name;
            foreach (XmlAttribute attr in xml.Attributes)
            {
                _attributeCreator.Find(attr.Name, a => {
                    a.Value = attr.Value;
                    Attributes.Add(attr.Name, a);
                });
            }

            if (xml.HasChildNodes)
            {
                foreach (XmlNode child in xml)
                {
                    if (child.Name[0] != '#')
                    {
                        Children.Add(new Element(child));
                    }
                }
            }
        }
        public Element(Document context)
        {
            var name = context.GetString("t");
            Name = name;
            context.SelectContext("a", attrs => {
                foreach (var attr in attrs)
                {
                    _attributeCreator.Find(attr.Key, a => {
                        a.Value = attr.Value?.ToString() ?? string.Empty;
                        Attributes.Add(attr.Key, a);
                    });
                }
            });
                    
            var childs = context.GetDocumentList("c");
            if (childs.Count > 0)
            {
                foreach (var child in childs)
                {
                    Children.Add(new Element(child));
                }
            }
        }
    }
}
