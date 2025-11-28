using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Controllers
{
    class GuestController : BaseController
    {
        public object Login(EditContext context)
        {
            var res = Call("login", context.ValueContext);
            return Redirect("gflx");

            //App.User = new Actors.Admin();
            //return Redirect(App.User.GetType().Name);
        }
    }
}
