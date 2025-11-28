using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Views.Account
{
    class Edit : FormView<ViewModels.AccountEditContext>
    {
    }

    class Add : Edit 
    {
        protected override void RenderCore()
        {
            base.RenderCore();

            var phone = MainView["phone"].EditorInfo.Control;
            var usern = MainView["_id"].EditorInfo.Control;

            phone.ValueChanged += (_, __) => {
                usern.Value = phone.Value;
            };
        }
    }
}
