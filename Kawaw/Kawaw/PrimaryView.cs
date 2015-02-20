using Kawaw.Framework;
using Xamarin.Forms;

namespace Kawaw
{
    class PrimaryView : BaseView
    {
        public PrimaryView()
        {
            Icon = "kawaw.png";
            var itemOrder = Device.OnPlatform(ToolbarItemOrder.Primary, ToolbarItemOrder.Secondary, ToolbarItemOrder.Default);
            ToolbarItems.Add(new ToolbarItem(
                "Refresh", "refresh.png",
                () => MessagingCenter.Send<object>(this, "refresh"),
                itemOrder));
            ToolbarItems.Add(new ToolbarItem(
                "Logout", "logout.png", async () =>
                {
                    var logout = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
                    if (logout)
                    {
                        MessagingCenter.Send<object>(this, "logout");
                    }
                },
                itemOrder));
        }

    }
}