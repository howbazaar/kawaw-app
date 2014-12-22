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
    }

    class BaseLogoutView : BaseView
    {
        public BaseLogoutView()
        {
            Icon = "kawaw.png";
            ToolbarItems.Add(new ToolbarItem("Logout", null, async () =>
            {
                var logout = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
                if (logout)
                {
                    MessagingCenter.Send<object>(this, "logout");
                }
            }, ToolbarItemOrder.Secondary));
        }

    }
}