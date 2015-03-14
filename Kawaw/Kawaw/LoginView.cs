using System.Diagnostics;
using System.Linq;
using Kawaw.Framework;
using Xamarin.Forms;

namespace Kawaw
{
    class LoginView : BaseView
    {
        public LoginView()
        {
            Title = "Login";
            Icon = "kawaw.png";
            Padding = new Thickness(20);

            var emailEntry = new Entry
            {
                Placeholder = "E-mail address",
                Keyboard = Keyboard.Email
            };
            emailEntry.SetBinding(Entry.TextProperty, "Email");
            var passwordEntry = new Entry
            {
                Placeholder = "Password",
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

            var remoteUrl = new Label();
            remoteUrl.SetBinding(Label.TextProperty, "RemoteUrl");

            Content = new StackLayout
            {
                Spacing = 10,
                Children = {
                    emailEntry,
                    passwordEntry,
                    loginButton,
                    registerButton,
#if DEBUG
                    remoteUrl,
#endif
                }
            };

#if DEBUG
            AddSiteToolbarOptions();
#endif
        }

        private void AddSiteToolbarOptions()
        {
            var itemOrder = Device.OnPlatform(ToolbarItemOrder.Primary, ToolbarItemOrder.Secondary, ToolbarItemOrder.Default);
            ToolbarItems.Add(new ToolbarItem("Production", null, () => MessagingCenter.Send<object, string>(this, "set-remote-site", "https://kawaw.com"), itemOrder));
            ToolbarItems.Add(new ToolbarItem("Tim's Laptop", null, () => MessagingCenter.Send<object, string>(this, "set-remote-site", "http://192.168.1.7:8080"), itemOrder));
        }

        protected override bool OnBackButtonPressed()
        {
            // Say we have handled it.
            // This means that the app won't close on back button.

            // Fix is in forms 1.3.5, correct thing to do is return flase there.
            // Bug still there in 1.4.0.
            return true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SubscribeAlert<LoginViewModel>();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            UnsubscribeAlert<LoginViewModel>();
        }
    
    }
}