using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    partial class Document
    {
        public string Caption { get => GetString("caption"); set => Push("caption", value); }
        public string Phone { get => GetString("phone"); set => Push("phone", value); }
        public DateTime? Time { get => GetDateTime("time"); set => Push("time", value); }
    }
}
