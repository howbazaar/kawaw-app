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
                Debug.WriteLine("root view show model {0}", model);
                Detail = new NavigationPage(ViewModelNavigation.GetPageForViewModel(model));
                IsPresented = false;
            });
        }

        protected override void OnAppearing()
        {
            Debug.WriteLine("subscribe to root view model alert {0}", this);
            base.OnAppearing();
            MessagingCenter.Subscribe(this, "alert", async (RootViewModel model, Alert alert) =>
            {
                await DisplayAlert(alert.Title, alert.Text, "OK");
                if (alert.Callback != null)
                {
                    alert.Callback.Execute(this);
                }
            });
        }

        protected override void OnDisappearing()
        {
            Debug.WriteLine("unsubscribe to root view model alert {0}", this);
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<RootViewModel, Alert>(this, "alert");
        }
    }
}