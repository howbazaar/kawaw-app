using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Kawaw
{
    class LoginViewModel : BaseViewModel
    {
        private string _email;
        private string _password;
        private string _remoteUrl;

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

        // autoproperty magic, c# rubbish
        public ICommand LoginCommand { get; private set; }
        public ICommand RegisterCommand { get; private set; }
        public LoginViewModel(IApp app)
            : base(app)
        {
            RegisterCommand = new Command(async () =>
            {
                await Navigation.PushAsync(new RegisterViewModel(App));
            });
            Command loginCommand = null;
            RemoteUrl = App.Remote.BaseUrl;
            bool canLogin = true;
            loginCommand = new Command(async () =>
            {
                canLogin = false;
                loginCommand.ChangeCanExecute();
                IsBusy = true;

                var remote = app.Remote;
                var worked = await remote.Login(_email, _password);
                if (!worked)
                {
                    IsBusy = false;
                    // TODO: show error message about bad credentials.
                    canLogin = true;
                    loginCommand.ChangeCanExecute();
                    return;
                }
                var details = await remote.GetUserDetails();
                var user = new RemoteUser(details, app.Remote);
                App.User = user;
                MessagingCenter.Send<object>(this, "user-updated");

                // assume we have logged in and pop the page
                IsBusy = false;
                await Navigation.PopModalAsync();

                canLogin = true;
                loginCommand.ChangeCanExecute();

                // set canLogin back to true if something fucked up.
            }, () => canLogin);
            LoginCommand = loginCommand;

            MessagingCenter.Subscribe<object, string>(this, "set-remote-site", (object sender, string url) =>
            {
                Debug.WriteLine("Setting remote site to {0}", url);
                App.Remote.BaseUrl = url;
                RemoteUrl = url;
            });

        }

        public void Reset()
        {
            Email = "";
            Password = "";
        }
    }
}