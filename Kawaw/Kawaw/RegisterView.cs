using System.ServiceModel.Security;
using Kawaw.Framework;
using Xamarin.Forms;

namespace Kawaw
{
    class RegisterView : BaseView
    {
        private Entry _passwordEntry;
        private Entry _emailEntry;
        private Entry _passwordEntry2;

        public RegisterView()
        {
            Title = "Register";
            Icon = "kawaw.png";
            Padding = new Thickness(20);

            _emailEntry = new Entry
            {
                Placeholder = "E-mail address",
                Keyboard = Keyboard.Email
            };
            _emailEntry.SetBinding(Entry.TextProperty, "Email");
            _passwordEntry = new Entry
            {
                Placeholder = "Your password",
                IsPassword = true
            };
            _passwordEntry.SetBinding(Entry.TextProperty, "Password");
            _passwordEntry2 = new Entry
            {
                Placeholder = "Your password again",
                IsPassword = true
            };
            _passwordEntry2.SetBinding(Entry.TextProperty, "Password2");
            var registerButton = new Button
            {
                Text = "Register"
            };
            registerButton.SetBinding(Button.CommandProperty, "RegisterCommand");

            Content = new StackLayout
            {
                Spacing = 10,
                Children = {
                    _emailEntry,
                    _passwordEntry,
                    _passwordEntry2,
                    registerButton
                }
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // SubscribeAlert<RegisterViewModel>();
            MessagingCenter.Subscribe(this, "alert", async (RegisterViewModel model, Alert alert) =>
            {
                await DisplayAlert(alert.Title, alert.Text, "OK");
                if (alert.Callback != null)
                {
                    alert.Callback.Execute(this);
                }
            });
            if (_passwordEntry.Text != "")
            {
                _passwordEntry2.Focus();
            }
            else if (_emailEntry.Text != "")
            {
                _passwordEntry.Focus();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // UnsubscribeAlert<RegisterViewModel>();
            MessagingCenter.Unsubscribe<RegisterViewModel, Alert>(this, "alert");
        }

    }
}