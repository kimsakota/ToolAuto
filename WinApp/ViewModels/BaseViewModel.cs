using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class BaseViewModel : Document
    {
        public string SearchComment { get; set; } = "tìm kiếm";
        public Action<string> SearchAction { get; set; }

        public DocumentList Documents => (DocumentList)Value;
    }
}
