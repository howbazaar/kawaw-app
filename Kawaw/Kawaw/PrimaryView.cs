using Xamarin.Forms;

namespace Kawaw
{
    class PrimaryView : BaseView
    {
        public PrimaryView()
        {
            Icon = "kawaw.png";
            ToolbarItems.Add(new ToolbarItem(
                "Refresh", null,
                () => MessagingCenter.Send<object>(this, "refresh"),
                ToolbarItemOrder.Secondary));
            ToolbarItems.Add(new ToolbarItem(
                "Logout", null, async () =>
                {
                    var logout = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
                    if (logout)
                    {
                        MessagingCenter.Send<object>(this, "logout");
                    }
                },
                ToolbarItemOrder.Secondary));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Subscribe(this, "alert", async (RootViewModel model, Alert alert) =>
            {
                await DisplayAlert(alert.Title, alert.Text, "OK");
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<RootViewModel, Alert>(this, "alert");
        }

    }
}