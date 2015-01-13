using Xamarin.Forms;

namespace Kawaw
{
    class BaseLogoutView : BaseView
    {
        public BaseLogoutView()
        {
            Icon = "kawaw.png";
            ToolbarItems.Add(new ToolbarItem("Refresh", null, async () =>
            {
                MessagingCenter.Send<object>(this, "refresh");
            }, ToolbarItemOrder.Secondary));
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