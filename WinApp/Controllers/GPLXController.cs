using System;
using System.Collections.Generic;
using System.Linq;
using System.Mvc;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WinApp.Controllers
{
    internal class GPLXController : BaseController
    {
        static Models.GPLX _model = new Models.GPLX();
        public override object Open(Document model)
        {
            return base.Open(_model.Current = model);
        }

        public object Import(DocumentList data)
        {
            _model.Import(data);
            return GoFirst();
        }

        public object Xong()
        {
            _model.Done();
            return GoFirst();
        }

        public override object Index()
        {
            return View(_model.LoadData());
        }
        public object Saved()
        {
            return View(_model.Saved.LoadData());
        }
    }
}
