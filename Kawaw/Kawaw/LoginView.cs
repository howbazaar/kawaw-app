using System;
using System.Diagnostics;
using System.Linq;
using Kawaw.Framework;
using Xamarin.Forms;

namespace Kawaw
{
    class LoginView : BaseView
    {
        private Entry _emailEntry;

        public LoginView()
        {
            Title = "Login";
            Icon = "kawaw.png";
            Padding = new Thickness(20);

            _emailEntry = new Entry
            {
                Placeholder = "E-mail address",
                Keyboard = Keyboard.Email
            };
            _emailEntry.SetBinding(Entry.TextProperty, "Email");
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
                    _emailEntry,
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
            ToolbarItems.Add(new ToolbarItem("Prod", null, () => MessagingCenter.Send<object, string>(this, "set-remote-site", "https://kawaw.com"), itemOrder));
            ToolbarItems.Add(new ToolbarItem("Laptop", null, () => MessagingCenter.Send<object, string>(this, "set-remote-site", "http://10.0.0.21:8080"), itemOrder));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // SubscribeAlert<LoginViewModel>();
            MessagingCenter.Subscribe(this, "alert", async (LoginViewModel model, Alert alert) =>
            {
                await DisplayAlert(alert.Title, alert.Text, "OK");
                if (alert.Callback != null)
                {
                    alert.Callback.Execute(this);
                }
            });
            _emailEntry.Focus();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // UnsubscribeAlert<LoginViewModel>();
            MessagingCenter.Unsubscribe<LoginViewModel, Alert>(this, "alert");
        }
    
    }

    class LoginNavigationPage : NavigationPage
    {
        public LoginNavigationPage(Page page)
            :base(page)
        {
        }

        protected override bool OnBackButtonPressed()
        {
            // Say we have handled it.
            // This means that the app won't close on back button.

            // Fix is in forms 1.3.5, correct thing to do is return false there.
            // Bug still there in 1.4.0.
            return false;
        }
    }
}