using System.Collections;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace Kawaw
{
    class CellHeaderLabel : Label { }

    class ProfileView : PrimaryView
    {
        class EmailCell : ViewCell
        {
            public EmailCell()
            {
                var address = new CellHeaderLabel();
                address.SetBinding(Label.TextProperty, "Address");
                var description = new Label();
                description.SetBinding(Label.TextProperty, "Description");

                var viewLayout = new StackLayout()
                {
                    Padding = new Thickness(15,0,0,0),
                    Children = { address, description}
                };
                View = viewLayout;
            }
        }

        public ProfileView()
        {
            Title = "Profile";
            var name = new Label();
            name.SetBinding(Label.TextProperty, "FullName");
            var address = new Label();
            address.SetBinding(Label.TextProperty, "Address");

            var changeDetails = new Button
            {
                Text = "Change Details",
                HorizontalOptions = LayoutOptions.Fill,
            };
            changeDetails.SetBinding(Button.CommandProperty, "ChangeDetailsCommand");

            var dob = new Label();
            dob.SetBinding(Label.TextProperty, "DateOfBirth");

            var emailList = new ListView
            {
                RowHeight = 60,
                VerticalOptions = LayoutOptions.Start
            };
            emailList.SetBinding(ListView.ItemsSourceProperty, "Emails");
            emailList.SetBinding(ListView.SelectedItemProperty, "SelectedItem", BindingMode.TwoWay);
            emailList.ItemTemplate = new DataTemplate(typeof(EmailCell));
            emailList.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == ListView.ItemsSourceProperty.PropertyName)
                {
                    emailList.HeightRequest = emailList.RowHeight*((ICollection)emailList.ItemsSource).Count;
                }
            };

            var addEmail = new Button
            {
                Text = "Add E-mail address",
                HorizontalOptions = LayoutOptions.Fill,
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
                            new Label{Text = "Date of Birth: "},
                            dob
                        }
                    },
                    changeDetails,
                    new Label{Text = "Email Addresses: "},
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
            Debug.WriteLine("ProfileView.OnAppearing");
            base.OnAppearing();
            // SubscribeAlert<ProfileViewModel>();
            MessagingCenter.Subscribe(this, "alert", async (ProfileViewModel model, Alert alert) =>
            {
                Debug.WriteLine("Show an alert: {0}", alert.Text);
                await DisplayAlert(alert.Title, alert.Text, "OK");
                if (alert.Callback != null)
                {
                    alert.Callback.Execute(this);
                }
            });

            // Calling the ForceLayout directly has it attempting to relayout the items list before it
            // has the source property set, so by calling invoke on main thread, this call gets put at
            // the end of the current call queue.
            // MessagingCenter.Subscribe(this, "emails-updated", (ProfileViewModel model) => Device.BeginInvokeOnMainThread(ForceLayout));
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
            Debug.WriteLine("ProfileView.OnDisappearing");
            base.OnDisappearing();
            //UnsubscribeAlert<ProfileViewModel>();
            MessagingCenter.Unsubscribe<ProfileViewModel, Alert>(this, "alert");

            MessagingCenter.Unsubscribe<ProfileViewModel>(this, "emails-updated");
            MessagingCenter.Unsubscribe<ProfileViewModel, EmailActionOptions>(this, "show-options");
        }
    }
}