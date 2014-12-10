using System.Diagnostics;
using Xamarin.Forms;

namespace Kawaw
{
    class RootView : MasterDetailPage
    {
        public RootView()
        {
            this.SetBinding(NavigationProperty, new Binding("Navigation", converter: new NavigationConverter()));
            this.SetBinding(IsBusyProperty, "IsBusy");

            MessagingCenter.Subscribe(this, "show-page", (RootViewModel sender, BaseViewModel model) =>
            {
                Debug.WriteLine("root view show model {0}", model.ToString());
                Detail = ViewModelNavigation.GetPageForViewModel(model);
                IsPresented = false;
            });

        }
    }
}