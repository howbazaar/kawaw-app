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

            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Spacing = 10,
                Children =
                {
                    new Label
                    {
                        Text = "Add E-Mail address",
                        Font = Font.SystemFontOfSize(NamedSize.Large),
                    },
                    emailEntry,
                    new StackLayout
                    {
                        // LayoutOptions = LayoutAlignment.End,
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            cancelButton,
                            addButton,
                        }
                    },
                }
            };


        }

    }
}