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

            var dob = new Label();
            dob.SetBinding(Label.TextProperty, "DateOfBirth");

            Content = new StackLayout
            {
                Spacing = 10,
                Children =
                {
                    name,
                    address,
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            new Label{Text = "Date of Birth: "},
                            dob
                        }
                    },
                    changeDetails,
                }
            };

            ToolbarItems.Add(new ToolbarItem("Logout", null, () => MessagingCenter.Send<object>(this, "logout"), ToolbarItemOrder.Secondary));
        }
    }
}