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

            Command registerCommand = null;
            bool canRegister = true;
            registerCommand = new Command(async () =>
            {

                // Check that the passwords match...
                if (password != password2)
                {
                    MessagingCenter.Send(this, "alert", new Alert
                    {
                        Title = "Password Action",
                        Text = "Your passwords must match."
                    });
                    return;
                }

                canRegister = false;
                registerCommand.ChangeCanExecute();
                IsBusy = true;
                
                var remote = app.Remote;
                var worked = await remote.Register(name, email, password, password2);
                if (!worked)
                {
                    IsBusy = false;
                    // TODO: show error message about bad credentials.
                    canRegister = true;
                    registerCommand.ChangeCanExecute();
                    return;
                }

                worked = await remote.Login(email, password);
                var details = await remote.GetUserDetails();
                App.User = new RemoteUser(details, remote);
                MessagingCenter.Send<object>(this, "user-updated");

                // assume we have logged in and pop the page
                IsBusy = false;
                await Navigation.PopModalAsync();
            }, () => canRegister);
            RegisterCommand = registerCommand;

        }

    }
}