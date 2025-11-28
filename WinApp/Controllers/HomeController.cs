using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Controllers
{
    internal class HomeController : BaseController
    {
        public override object Index()
        {
            return Redirect("gplx");
        }
    }
}
