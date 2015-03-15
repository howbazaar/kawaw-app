using Kawaw.Framework;
using Xamarin.Forms;

namespace Kawaw
{
    class AddEmailView : BaseView
    {
        public AddEmailView()
        {
            Title = "Profile";
            Icon = "kawaw.png";
            Padding = new Thickness(20);

            var size = Device.GetNamedSize(NamedSize.Medium, typeof (Label));
            var emailEntry = new Entry
            {
                Placeholder = "E-mail address",
                Keyboard = Keyboard.Email
            };
            emailEntry.SetBinding(Entry.TextProperty, "Email");

            var addButton = new Button{ Text = "Add" };
            addButton.SetBinding(Button.CommandProperty, "AddCommand");
            var cancelButton = new Button { Text = "Cancel" };
            cancelButton.SetBinding(Button.CommandProperty, "CancelCommand");

            Content = new ContentView
            {
                // Push the frame up slightly to be more visually appealing.
                Padding = new Thickness(0, 0, 0, 20),
                Content = new Frame
                {
                    VerticalOptions = LayoutOptions.Center,
                    OutlineColor = App.AccentColor,
                    BackgroundColor = App.AccentColor,
                    Padding = 2,

                    Content = new Grid
                    {
                        Padding = 10,
                        BackgroundColor = App.BackgroundColor,
                        RowSpacing = 10,
                        RowDefinitions =
                        {   // Make each of the three rows high enough for the content, not all equal for the biggest.
                            new RowDefinition{Height = GridLength.Auto},
                            new RowDefinition{Height = GridLength.Auto},
                            new RowDefinition{Height = GridLength.Auto}
                        },
                        Children =
                        {
                            {
                                new Label
                                {
                                    Text = "Add E-Mail address:",
                                    FontSize = size,
                                },
                                0, 2, 0, 1
                            },
                            {emailEntry, 0, 2, 1, 2},
                            {cancelButton, 0, 2},
                            {addButton, 1, 2},
                        }
                    }
                }
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // SubscribeAlert<AddEmailViewModel>();
            MessagingCenter.Subscribe(this, "alert", async (AddEmailViewModel model, Alert alert) =>
            {
                await DisplayAlert(alert.Title, alert.Text, "OK");
                if (alert.Callback != null)
                {
                    alert.Callback.Execute(this);
                }
            });

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // UnsubscribeAlert<AddEmailViewModel>();
            MessagingCenter.Unsubscribe<AddEmailViewModel, Alert>(this, "alert");
        }

    }
}