using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace Kawaw
{
    class ProfileView : BaseLogoutView
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
            var size = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
            var name = new Label
            {
                FontSize = size
            };
            name.SetBinding(Label.TextProperty, "FullName");
            var address = new Label
            {
                FontSize = size
            };
            address.SetBinding(Label.TextProperty, "Address");

            var changeDetails = new Button
            {
                Text = "Change Details",
                HorizontalOptions = LayoutOptions.Center,
            };
            changeDetails.SetBinding(Button.CommandProperty, "ChangeDetailsCommand");

            var dob = new Label()
            {
                FontSize = size
            };
            dob.SetBinding(Label.TextProperty, "DateOfBirth");

            var list = new ListView
            {
                // NOTE: this is a bug, fixed in 1.3 final, will need to change number once we update.
                RowHeight = 120,
                VerticalOptions = LayoutOptions.Start
            };
            list.SetBinding(ListView.ItemsSourceProperty, "Emails");
            list.SetBinding(ListView.SelectedItemProperty, "SelectedItem", BindingMode.TwoWay);
            list.ItemTemplate = new DataTemplate(typeof(TextCell));
            list.ItemTemplate.SetBinding(TextCell.TextProperty, "Address");
            list.ItemTemplate.SetBinding(TextCell.DetailProperty, "Description");

            var addEmail = new Button
            {
                Text = "Add E-mail address",
                HorizontalOptions = LayoutOptions.Center,
            };
            addEmail.SetBinding(Button.CommandProperty, "AddEmailCommand");

            var view = new StackLayout
            {
                Padding = 10,
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
                            new Label{Text = "Date of Birth: ", FontSize = size},
                            dob
                        }
                    },
                    changeDetails,
                    new Label{Text = "Email Addresses: ", FontSize = size},
                    list,
                    addEmail,
                }
            };

            Content = new ScrollView
            {
                Content = view
            };

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("appearing, so subscribe {0}", this.Id);
            MessagingCenter.Subscribe(this, "alert", async (ProfileViewModel model, Alert alert) =>
            {
                await DisplayAlert(alert.Title, alert.Text, "OK");
            });

            MessagingCenter.Subscribe(this, "show-options", async (ProfileViewModel model, EmailActionOptions options) =>
            {
                var textOptions = from tuple in options.Options select tuple.Item2;
                var action = await DisplayActionSheet("E-mail Action", "Cancel", null, textOptions.ToArray());
                // action here is the long name, and we want the short one.
                if (action == null || action == "Cancel")
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