using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using Kawaw.Exceptions;
using Kawaw.Framework;
using Xamarin;
using Xamarin.Forms;

namespace Kawaw
{
    class LoginViewModel : BaseViewModel
    {
        private string _email;
        private string _password;
        private string _remoteUrl;
        private readonly Command _loginCommand;
        private readonly Command _registerCommand;
        private bool _buttonsActive;

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

        public string RemoteUrl
        {
            get { return _remoteUrl; }
            set { SetProperty(ref _remoteUrl, value); }
        }

        public ICommand LoginCommand
        {
            get { return _loginCommand; }
        }

        public ICommand RegisterCommand
        {
            get { return _registerCommand; }
        }

        public LoginViewModel(IApp app)
            : base(app)
        {
            RemoteUrl = App.Remote.BaseUrl;

            _buttonsActive = true;
            _registerCommand = new Command(async () =>
            {
                await Navigation.PushAsync(new RegisterViewModel(App));
            }, () => _buttonsActive);

            _loginCommand = new Command(async () =>
            {
                if (!Validate()) return;

                UpdateButtonsActive(false);

                var remote = app.Remote;
                try
                {
                    App.User = await remote.Login(_email, _password);
                    await App.User.Refresh();
                    // Let's tell Xamarin about this user.
                    var traits = new Dictionary<string, string>
                    {
                        {Insights.Traits.Email, App.User.PrimaryEmail},
                        {Insights.Traits.Name, App.User.FullName}
                    };
                    Insights.Identify(App.User.PrimaryEmail, traits);
                    await Navigation.PopModalAsync();
                }
                catch (FormErrorsException e)
                {
                    MessagingCenter.Send(this, "alert", new Alert
                    {
                        Title = "Login Failed",
                        Text = e.Message
                    });
                }
                catch (NetworkDownException e)
                {
                    MessagingCenter.Send(this, "alert", new Alert
                    {
                        Title = "Login Failed",
                        Text = e.Message
                    });
                }
                catch (Exception exception)
                {
                    Insights.Report(exception, new Dictionary<string, string>
                    {
                        {"Location", "LoginViewModel Login"}
                    }, ReportSeverity.Error);
                    MessagingCenter.Send(this, "alert", new Alert
                    {
                        Title = "Login Failed",
#if DEBUG
                        Text = exception.Message,
#endif
                        Text = "Something went wrong, please try again later",
#endif
                    });
                }

                UpdateButtonsActive(true);
            }, () => _buttonsActive);

            MessagingCenter.Subscribe(this, "set-remote-site", (object sender, string url) =>
            {
                Debug.WriteLine("Setting remote site to {0}", url);
                App.Remote.BaseUrl = url;
                RemoteUrl = url;
            });

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
                    Text = "You must enter your password."
                });
                return false;
            }

            return true;
        }

        private void UpdateButtonsActive(bool active)
        {
            _buttonsActive = active;
            IsBusy = !active;
            _loginCommand.ChangeCanExecute();
            _registerCommand.ChangeCanExecute();
        }

        public void Reset()
        {
            Email = "";
            Password = "";
        }
    }
}