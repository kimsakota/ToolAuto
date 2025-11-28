using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.ViewModels
{
    class AccountListContext : ListContext
    {
        public AccountListContext() : base("account") { }
    }

    class AccountEditContext : EditContext
    {
        public AccountEditContext() : base("account") { }
    }
}
