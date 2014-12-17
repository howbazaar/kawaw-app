using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Kawaw
{
    class LoginViewModel : BaseViewModel
    {
        private string email;
        private string password;

        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }

        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
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
            bool canLogin = true;
            loginCommand = new Command(async () =>
            {
                canLogin = false;
                loginCommand.ChangeCanExecute();
                IsBusy = true;

                var remote = app.Remote;
                var worked = await remote.Login(email, password);
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
        }

        public void Reset()
        {
            Email = "";
            Password = "";
        }
    }
}