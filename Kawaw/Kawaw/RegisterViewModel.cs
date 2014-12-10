using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Kawaw
{
    class RegisterViewModel : BaseViewModel
    {
        private string email;
        private string password;
        private string password2;
        private string name;

        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }
        public string Password2
        {
            get { return password2; }
            set { SetProperty(ref password2, value); }
        }
        public ICommand RegisterCommand { get; private set; }
        public RegisterViewModel(IApp app)
            : base(app)
        {
            RegisterCommand = new Command(async () =>
            {
                Debug.WriteLine("Email: " + email);
                Debug.WriteLine("Password: {0}", password);
                IsBusy = true;
                await Task.Delay(2000);
                var request = HttpWebRequest.CreateHttp("https://kawaw.com/+login-form");

                // assume we have logged in and pop the page
                IsBusy = false;
                await Navigation.PopModalAsync();
            });
        }

    }
}