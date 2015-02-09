using System.Diagnostics;
using Xamarin.Forms;

namespace Kawaw
{
    class BaseView : ContentPage
    {
        public BaseView()
        {
            // Navigation is the name of the property in the view-model instance
            this.SetBinding(NavigationProperty, new Binding("Navigation", converter: new NavigationConverter()));
            this.SetBinding(IsBusyProperty, "IsBusy");
        }

        protected void SubscribeAlert<TViewModel>()
            where TViewModel : class
        {
            MessagingCenter.Subscribe(this, "alert", async (TViewModel model, Alert alert) =>
            {
                await DisplayAlert(alert.Title, alert.Text, "OK");
                if (alert.Callback != null)
                {
                    alert.Callback.Execute(this);
                }
            });
        }

        protected void UnsubscribeAlert<TViewModel>()
            where TViewModel : class
        {
            MessagingCenter.Unsubscribe<TViewModel, Alert>(this, "alert");
        }

    }
}