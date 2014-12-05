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
        public LoginViewModel()
        {
            RegisterCommand = new Command(async () =>
            {
                await Navigation.PushAsync(new RegisterViewModel());
            });
            Command loginCommand = null;
            bool canLogin = true;
            loginCommand = new Command(async () =>
            {
                canLogin = false;
                loginCommand.ChangeCanExecute();
                Debug.WriteLine("Email: " + email);
                Debug.WriteLine("Password: {0}", password);
                IsBusy = true;
                await Task.Delay(2000);
                var request = HttpWebRequest.CreateHttp("https://kawaw.com/+login-form");

                // assume we have logged in and pop the page
                IsBusy = false;
                await Navigation.PopModalAsync();

                // set canLogin back to true if something fucked up.
            }, () => canLogin);
            LoginCommand = loginCommand;
        }
    }
}