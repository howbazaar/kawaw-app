using Kawaw.Framework;
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

            var emailEntry = new Entry
            {
                Placeholder = "E-mail address",
                Keyboard = Keyboard.Email
            };
            emailEntry.SetBinding(Entry.TextProperty, "Email");
            var passwordEntry = new Entry
            {
                Placeholder = "Your password",
                IsPassword = true
            };
            passwordEntry.SetBinding(Entry.TextProperty, "Password");
            var passwordEntry2 = new Entry
            {
                Placeholder = "Your password again",
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
                    emailEntry,
                    passwordEntry,
                    passwordEntry2,
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
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // UnsubscribeAlert<RegisterViewModel>();
            MessagingCenter.Unsubscribe<RegisterViewModel, Alert>(this, "alert");
        }

    }
}