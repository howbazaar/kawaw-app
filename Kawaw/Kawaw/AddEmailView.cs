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
                    OutlineColor = Device.OnPlatform(Color.Black, Color.White, Color.Blue),
                    Content = new Grid
                    {
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
                                0, 3, 0, 1
                            },
                            {emailEntry, 0, 3, 1, 2},
                            {cancelButton, 1, 2},
                            {addButton, 2, 2},
                        }
                    }
                }
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SubscribeAlert<AddEmailViewModel>();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            UnsubscribeAlert<AddEmailViewModel>();
        }

    }
}