using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Controllers
{
    class ApiCaller : ApiRestfull
    {
        public ApiCaller(string action) 
        {
            //Protocol = "http";
            HostName = "test.vst.edu.vn";
            Action = $"{App.User?.Role ?? "guest"}/{action}";
        }
    }

    class BaseController : System.Mvc.Controller
    {
        protected object GoFirst()
        {
            return RedirectToAction("Index");
        }
        public async Task<object> Call(string action, Document data)
        {
            var api = new ApiCaller(action);
            var res = await api.Request(data);

            return res;
        }
        public object Send(string serverName, string action, Document data)
        {
            App.Mqtt.Publish($"{serverName}/{action}", data.ToString());
            return null;
        }

        public virtual object Index()
        {
            return View();
        }

        public virtual object Open(Document model) => View(model);
        public virtual object Edit(Document model) => View(model);
        public virtual object Add() => View();

        public object Update(EditContext context)
        {
            switch (context.Action)
            {
                case EditContext.Insert:
                    break;

                case EditContext.Delete:
                    break;
            }
            return GoFirst();
        }
    }
}
