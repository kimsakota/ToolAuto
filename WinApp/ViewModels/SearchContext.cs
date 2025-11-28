using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.ViewModels
{
    public class SearchContext : Document
    {
        //public string Comment { get => GetString(nameof(Comment)); set => Push(nameof(Comment), value); }
        //public object Cleared { get => base[nameof(Cleared)]; set => Push(nameof(Cleared), value); }
        //public object Searching { get => base[nameof(Searching)]; set => Push(nameof(Searching), value); }

        public SearchContext(string comment, Action<string> enter, Action<string> changed, Action cleared)
        {
            Add("Comment", comment ?? "tìm kiếm ... ");
            Push("Cleared", cleared);
            Push("Enter", enter);
            Push("Changed", changed);
        }
    }
}
