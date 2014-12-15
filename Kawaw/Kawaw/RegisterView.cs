using Xamarin.Forms;

namespace Kawaw
{
    class RegisterView : BaseView
    {
        public RegisterView()
        {
            Title = "Register";
            Icon = "kawaw.png";
            Padding = new Thickness(20);

            var nameEntry = new Entry
            {
                Placeholder = "Name"
            };
            nameEntry.SetBinding(Entry.TextProperty, "Name");
            var emailEntry = new Entry
            {
                Placeholder = "E-mail address"
            };
            emailEntry.SetBinding(Entry.TextProperty, "Email");
            var passwordEntry = new Entry
            {
                Placeholder = "Password",
                IsPassword = true
            };
            passwordEntry.SetBinding(Entry.TextProperty, "Password");
            var passwordEntry2 = new Entry
            {
                Placeholder = "Password",
                IsPassword = true
            };
            passwordEntry2.SetBinding(Entry.TextProperty, "Password2");
            var registerButton = new Button
            {
                Text = "Register"
            };
            registerButton.SetBinding(Button.CommandProperty, "RegisterCommand");

            Content = new StackLayout
            {
                Spacing = 10,
                Children = {
                    nameEntry,
                    emailEntry,
                    passwordEntry,
                    passwordEntry2,
                    registerButton
                }
            };

        }
    }
  
}