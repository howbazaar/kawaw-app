using System.Diagnostics;
using System.Linq;
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
            ToolbarItems.Add(new ToolbarItem("Production", null, async () =>
            {
                MessagingCenter.Send<object, string>(this, "set-remote-site", "https://kawaw.com");
            }, ToolbarItemOrder.Secondary));
            ToolbarItems.Add(new ToolbarItem("Tim's Laptop", null, async () =>
            {
                MessagingCenter.Send<object, string>(this, "set-remote-site", "http://192.168.1.7:8080");
            }, ToolbarItemOrder.Secondary));
        }

        protected override bool OnBackButtonPressed()
        {
            // Say we have handled it.
            // This means that the app won't close on back button. Jason is going to think about it
            // and let me know what to do.
            return true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("appearing, so subscribe {0}", this.Id);
            MessagingCenter.Subscribe(this, "alert", async (LoginViewModel model, Alert alert) =>
            {
                await DisplayAlert(alert.Title, alert.Text, "OK");
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Debug.WriteLine("disappearing, so unsubscribe {0}", this.Id);
            MessagingCenter.Unsubscribe<LoginViewModel, Alert>(this, "alert");
        }
    
    }
}