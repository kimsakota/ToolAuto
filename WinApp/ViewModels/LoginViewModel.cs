using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.ViewModels
{
    class LoginViewModel : EditContext
    {
        public LoginViewModel() : base("login")
        {
            Value = new Document {
                UserName = "admin",
                Password = "1",
            };
        }
    }
}
