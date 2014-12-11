using Xamarin.Forms;

namespace Kawaw
{
    class ProfileView : BaseView
    {
        public ProfileView()
        {
            Title = "Profile";
            Icon = "kawaw.png";
            Content = new Label { Text = "profile view" };

            ToolbarItems.Add(new ToolbarItem("Logout", null, () => MessagingCenter.Send<object>(this, "logout"), ToolbarItemOrder.Secondary));
        }
    }
}