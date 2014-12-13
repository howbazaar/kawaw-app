using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace Kawaw
{
    class ProfileView : BaseView
    {
        class EmailCell : ViewCell
        {
            public EmailCell()
            {
                var address = new Label();
                address.SetBinding(Label.TextProperty, "Address");

                var viewLayout = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    // Children = { image, nameLayout }
                };
                View = viewLayout;
            }
        }

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
                Text = "Change Details",
                HorizontalOptions = LayoutOptions.Center,
            };
            changeDetails.SetBinding(Button.CommandProperty, "ChangeDetailsCommand");

            var dob = new Label();
            dob.SetBinding(Label.TextProperty, "DateOfBirth");

            var list = new ListView();
            list.SetBinding(ListView.ItemsSourceProperty, "Emails");
            list.SetBinding(ListView.SelectedItemProperty, "SelectedItem", BindingMode.TwoWay);
            list.ItemTemplate = new DataTemplate(typeof(TextCell));
            list.ItemTemplate.SetBinding(TextCell.TextProperty, "Address");
            list.ItemTemplate.SetBinding(TextCell.DetailProperty, "Description");

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
                    new Label{Text = "Email Addresses: "},
                    list,
                }
            };

            ToolbarItems.Add(new ToolbarItem("Logout", null, () => MessagingCenter.Send<object>(this, "logout"), ToolbarItemOrder.Secondary));

            Debug.WriteLine("subscribe to alert message {0}", this.Id);
            MessagingCenter.Subscribe(this, "alert", async (ProfileViewModel model, Alert alert) =>
            {
                Debug.WriteLine("alert {0}", this.Id);
                await DisplayAlert(alert.Title, alert.Text, "OK");
            });

            Debug.WriteLine("subscribe to show-options message {0}", this.Id);
            MessagingCenter.Subscribe(this, "show-options", async (ProfileViewModel model, EmailActionOptions options) =>
            {
                Debug.WriteLine("show-options {0}", this.Id);
                var textOptions = from tuple in options.Options select tuple.Item2;
                var action = await DisplayActionSheet("E-mail Action", "Cancel", null, textOptions.ToArray());
                // action here is the long name, and we want the short one.
                if (action == "Cancel")
                    return;
                var result = from tuple in options.Options where tuple.Item2 == action select tuple.Item1;
                MessagingCenter.Send((object)this, "email-action", new EmailAction
                {
                    Email = options.Email,
                    Name = result.Single(),
                });
            });

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Debug.WriteLine("disappearing, so unsubscribe {0}", this.Id);
            MessagingCenter.Unsubscribe<ProfileViewModel, Alert>(this, "alert");
            MessagingCenter.Unsubscribe<ProfileViewModel, EmailActionOptions>(this, "show-options");
        }
    }
}