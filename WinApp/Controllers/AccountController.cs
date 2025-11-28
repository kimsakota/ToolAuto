using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Controllers
{
    class AccountController : BaseController
    {
        public override object Add()
        {
            return base.Add();
        }
        public override object Index()
        {
            return View(new Models.Account());
        }
        public object Login(Document context)
        {
            var role = context.GetString("role");
            App.User = (Actors.User)Activator.CreateInstance(Type.GetType($"Actors.{role}"));
            return Redirect("device");
        }
    }
}
