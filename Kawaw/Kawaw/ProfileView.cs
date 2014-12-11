using Xamarin.Forms;

namespace Kawaw
{
    class ProfileView : BaseView
    {
        public ProfileView()
        {
            Title = "Profile";
            Icon = "kawaw.png";

            var name = new Label();
            name.SetBinding(Label.TextProperty, "FullName");
            var address = new Label();
            address.SetBinding(Label.TextProperty, "Address");

            var changeDetails = new Button
            {
                Text = "Change Details"
            };
            changeDetails.SetBinding(Button.CommandProperty, "ChangeDetailsCommand");

            Content = new StackLayout
            {
                Spacing = 10,
                Children =
                {
                    name,
                    address,
                    changeDetails,
                }
            };

            ToolbarItems.Add(new ToolbarItem("Logout", null, () => MessagingCenter.Send<object>(this, "logout"), ToolbarItemOrder.Secondary));
        }
    }
}