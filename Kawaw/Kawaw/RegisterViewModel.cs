using System;
using System.Diagnostics;
using System.Windows.Input;
using Kawaw.Exceptions;
using Kawaw.Framework;
using PushNotification.Plugin;
using Xamarin.Forms;

namespace Kawaw
{
    class RegisterViewModel : BaseViewModel
    {
        private bool _buttonsActive;
        private string _email;
        private string _password;
        private string _password2;
        private readonly Command _registerCommand;

        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        public string Password2
        {
            get { return _password2; }
            set { SetProperty(ref _password2, value); }
        }

        public ICommand RegisterCommand
        {
            get { return _registerCommand; }
        }

        public RegisterViewModel(IApp app, string email, string password)
            : base(app)
        {
            Email = email;
            Password = password;
            _buttonsActive = true;
            _registerCommand = new Command(async () =>
            {
                if (!Validate()) return;

                UpdateButtonsActive(false);

                try
                {
                    App.User = await app.Remote.Register(_email, _password);
                    await App.User.RegisterDevice();
                    await App.User.Refresh();
                    await Navigation.PopModalAsync();
                }
                catch (FormErrorsException e)
                {
                    MessagingCenter.Send(this, "alert", new Alert
                    {
                        Title = "Registration Failed",
                        Text = e.Message
                    });
                }
                catch (NetworkDownException e)
                {
                    MessagingCenter.Send(this, "alert", new Alert
                    {
                        Title = "Registration Failed",
                        Text = e.Message
                    });
                }
                catch (Exception)
                {
                    MessagingCenter.Send(this, "alert", new Alert
                    {
                        Title = "Registration Failed",
                        Text = "Something went wrong, please try again later"
                    });
                }

                UpdateButtonsActive(true);

            }, () => _buttonsActive);
        }

        private void UpdateButtonsActive(bool active)
        {
            _buttonsActive = active;
            IsBusy = !active;
            _registerCommand.ChangeCanExecute();
        }

        private bool Validate()
        {
            if (string.IsNullOrEmpty(Email))
            {
                MessagingCenter.Send(this, "alert", new Alert
                {
                    Title = "Missing e-mail address",
                    Text = "You must enter your email address."
                });
                return false;
            }

            if (string.IsNullOrEmpty(Password))
            {
                MessagingCenter.Send(this, "alert", new Alert
                {
                    Title = "Missing password",
                    Text = "You must enter a password."
                });
                return false;
            }

            if (Password != Password2)
            {
                MessagingCenter.Send(this, "alert", new Alert
                {
                    Title = "Mismatching passwords",
                    Text = "Your passwords must match."
                });
                return false;
            }
            
            return true;
        }

    }
}