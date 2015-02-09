using System;
using Xamarin.Forms;

namespace Kawaw.Framework
{
    class BaseView : ContentPage
    {
        static readonly BindablePropertyKey IsPageVisiblePropertyKey = BindableProperty.CreateReadOnly(
            "IsPageVisible", typeof (bool), typeof (BaseView), false);
        public static readonly BindableProperty IsPageVisibleProperty = IsPageVisiblePropertyKey.BindableProperty;

        public bool IsPageVisible
        {
            get { return (bool) GetValue(IsPageVisibleProperty); }
            private set { SetValue(IsPageVisiblePropertyKey, value); }
        }

        public BaseView()
        {
            // Navigation is the name of the property in the view-model instance
            this.SetBinding(NavigationProperty, new Binding("Navigation", converter: new NavigationConverter()));
            this.SetBinding(IsBusyProperty, "IsBusy");
            this.SetBinding(IsPageVisibleProperty, "IsPageVisible");
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            IsPageVisible = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            IsPageVisible = false;
        }
    }
}