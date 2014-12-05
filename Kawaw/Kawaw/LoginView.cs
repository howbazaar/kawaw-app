using Xamarin.Forms;

namespace Kawaw
{
    class LoginView : BaseView
    {
        public LoginView()
        {
            Title = "Login";

            Padding = new Thickness(20);
            //BackgroundColor = Color.Aqua;
            var emailEntry = new Entry
            {
                Placeholder = "name@example.com"
            };
            emailEntry.SetBinding(Entry.TextProperty, "Email");
            var passwordEntry = new Entry
            {
                IsPassword = true
            };
            passwordEntry.SetBinding(Entry.TextProperty, "Password");
            var loginButton = new Button
            {
                Text = "Login"
            };
            loginButton.SetBinding(Button.CommandProperty, "LoginCommand");
            var registerButton = new Button
            {
                Text = "Register"
            };
            registerButton.SetBinding(Button.CommandProperty, "RegisterCommand");

            Content = new StackLayout
            {
                Spacing = 10,
                Children = {
                    new Label{ Text = "E-mail" },
                    emailEntry,
                    new Label{ Text = "Password" },
                    passwordEntry,
                    loginButton,
                    registerButton
                }
            };
        }
    }
}