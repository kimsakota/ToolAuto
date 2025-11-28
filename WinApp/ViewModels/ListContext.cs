using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class ListContext : BaseViewModel
    {
        public DocumentList Columns 
        { 
            get => GetDocumentList("columns"); 
            set => Push("columns", value); 
        }
        public ListContext() { }
        public ListContext(string name)
        {
            Load(name);
        }
        public ListContext Load(string name)
        {
            Clear();

            this.Name = name;
            using (var sr = new IO.StreamReader(DB.Main.DataPath("list.json")))
            {
                var content = sr.ReadToEnd();
                var doc = Document.Parse(content);
                Copy(doc.GetDocument(name));

                return this;
            }
        }

        public DocumentList Import(string text)
        {
            var data = new DocumentList();

            var lines = text.Split('\n');
            var headers = lines[0].Split('\t');

            // Xác định vị trí các cột
            var fields = new string[headers.Length];
            var map = new Document();
            foreach (var p in this.Columns)
            {
                string s = p.GetString("import");
                if (s == null) s = p.Caption.Trim().ToLower();

                map.Add(s, p.Name);
            }
            for (int i = 0; i < headers.Length; i++)
            {
                var header = headers[i].Trim().ToLower();
                if (header == string.Empty)
                    continue;

                var field = map.GetString(header);
                if (field != null)
                {
                    fields[i] = field.ToString();
                }    
            }

            // Bắt đầu Import
            for (int i = 1; i < lines.Length; i++)
            {
                var s = lines[i].Trim();
                if (s == string.Empty) continue;

                var line = s.Split('\t');
                var doc = new Document();
                for (int k = 0; k < line.Length; k++)
                {
                    var field = fields[k];
                    if (field != null)
                    {
                        doc.Add(field, line[k].Trim());
                    }
                }    
                if (doc.Count != 0)
                    data.Add(doc);
            }

            this.Value = data;

            return data;
        }

    }
}
