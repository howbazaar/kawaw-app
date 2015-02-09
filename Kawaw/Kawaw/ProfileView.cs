using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace Kawaw
{
    class ProfileView : PrimaryView
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

            var emailList = new ListView
            {
                RowHeight = 60,
                VerticalOptions = LayoutOptions.Start
            };
            emailList.SetBinding(ListView.ItemsSourceProperty, "Emails");
            emailList.SetBinding(ListView.SelectedItemProperty, "SelectedItem", BindingMode.TwoWay);
            emailList.ItemTemplate = new DataTemplate(typeof(TextCell));
            emailList.ItemTemplate.SetBinding(TextCell.TextProperty, "Address");
            emailList.ItemTemplate.SetBinding(TextCell.DetailProperty, "Description");

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
                    emailList,
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
            SubscribeAlert<ProfileViewModel>();
            MessagingCenter.Subscribe(this, "emails-updated", (ProfileViewModel model) => ForceLayout());
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
            UnsubscribeAlert<ProfileViewModel>();
            MessagingCenter.Unsubscribe<ProfileViewModel>(this, "emails-updated");
            MessagingCenter.Unsubscribe<ProfileViewModel, EmailActionOptions>(this, "show-options");
        }
    }
}