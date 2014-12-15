﻿using System.Diagnostics;
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

                canRegister = false;
                registerCommand.ChangeCanExecute();
                IsBusy = true;
                Debug.WriteLine("Name: " + name);
                Debug.WriteLine("Email: " + email);
                Debug.WriteLine("Password: {0}", password);
                Debug.WriteLine("Password: {0}", password2);
                
                var remote = app.Remote;
                var worked = await remote.Register(email, name, password, password2);
                Debug.WriteLine(worked);
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
                var user = new RemoteUser(details);
                user.CSRFToken = remote.CSRFToken;
                user.SessionId = remote.SessionId;
                App.User = user;
                MessagingCenter.Send<object>(this, "user-updated");

                // assume we have logged in and pop the page
                IsBusy = false;
                await Navigation.PopModalAsync();
            }, () => canRegister);
            RegisterCommand = registerCommand;

        }

    }
}