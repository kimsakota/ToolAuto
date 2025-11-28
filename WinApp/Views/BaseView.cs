using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WinApp.Views
{
    interface IAppView : System.Mvc.IView
    {
    }

    class BaseView<TView, TViewModel> : IAppView
        where TView: FrameworkElement, new()
        where TViewModel: BaseViewModel, new()
    {
        public object Content => MainView;
        public void Render(object model)
        {
            ViewModel.Value = model;
            MainView = new TView();

            App.Browser.ClearActions();

            RenderCore();
        }
        protected TViewModel ViewModel { get; set; } = new TViewModel();
        protected TView MainView { get; set; }
        protected Document Document => (Document)ViewModel.Value;
        protected DocumentList DocumentList => ViewModel.ValueList;
        protected virtual void RenderCore()
        {
            MainView.DataContext = ViewModel;
        }
    }
    class BaseView<TView> : BaseView<TView, BaseViewModel>
        where TView : FrameworkElement, new()
    {
        protected override void RenderCore()
        {
            MainView.DataContext = ViewModel.Value;
        }
    }

class FormView<TViewModel> : BaseView<Vst.Controls.MyTemplateForm, TViewModel>
        where TViewModel: EditContext, new()
    {
        protected override void RenderCore()
        {
            var id = ViewModel.ValueContext.ObjectId;
            string pre = id == null ? "Thêm" : "Cập nhật";
            
            ViewModel.Caption = $"{pre} {ViewModel.Caption}";
            if (id == null)
            {
                ViewModel.Action = EditContext.Insert;
            }

            base.RenderCore();
        }
    }
}
