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
                    Content = new StackLayout
                    {
                        VerticalOptions = LayoutOptions.Center,
                        Spacing = 10,
                        Children =
                        {
                            new Label
                            {
                                Text = "Add E-Mail address:",
                                FontSize = size,
                            },
                            emailEntry,
                            new Grid
                            {
                                HorizontalOptions = LayoutOptions.End,
                                Children =
                                {
                                    {cancelButton,0,0},
                                    {addButton,1,0},
                                }
                            },
                        }
                    }
                }
            };

        }

    }
}