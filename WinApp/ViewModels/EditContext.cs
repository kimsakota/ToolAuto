using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vst.Controls;

namespace System
{
    public class EditContext : BaseViewModel
    {
        public const string Insert = "+";
        public const string Delete = "-";
        public DocumentList Editors 
        { 
            get => GetDocumentList("editors"); 
            set => Push("editors", value); 
        }
        public Action<Document> AcceptCallback { get; set; }

        public EditContext(string name, string action, Document model)
        {
            Load(name);
            Value = model;
            Action = action;
        }
        public EditContext(string name) { Load(name); }
        public EditContext() { }
        public EditContext Load(string name)
        {
            Clear();
            using (var sr = new IO.StreamReader(DB.Main.DataPath("forms.json")))
            {
                var content = sr.ReadToEnd();
                var doc = Document.Parse(content);
                Copy(doc.GetDocument(name));

                return this;
            }
        }
    }
}
