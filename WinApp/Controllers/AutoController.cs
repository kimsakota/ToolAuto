using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Controllers
{
    class AutoController : BaseController
    {
        // Màn hình Chạy (OCR)
        public override object Index()
        {
            return View();
        }

        // Màn hình Cài đặt (Macro)
        public object Setting()
        {
            return View();
        }
    }
}