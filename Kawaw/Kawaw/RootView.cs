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
 
            var navigationModel = new NavigationViewModel();
            var eventModel = new EventsViewModel();
            Master = ViewModelNavigation.GetPageForViewModel(navigationModel);
            Detail = ViewModelNavigation.GetPageForViewModel(eventModel);

            MessagingCenter.Subscribe(this, "show-page", (NavigationViewModel sender, string page) =>
            {
                Debug.WriteLine("show-page: {0}", page);
            });
        }
    }
}