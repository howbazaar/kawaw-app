using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace Kawaw
{
    class RootViewModel : BaseViewModel
    {
        public const string Events = "Events";
        public const string Connections = "Connections";
        public const string Profile = "Profile";
        public const string Login = "Login";

        private ViewModelNavigation _navigation;

        public override ViewModelNavigation Navigation
        {
            get { return _navigation; }
            set { _navigation = value; Init(); }
        }

        public NavigationViewModel NavigationModel { get; private set; }

        private async void Init()
        {
            if (App.User == null)
            {
                ShowLogin();
            }
            else
            {
                IsBusy = true;
                await App.User.Refresh(App.Remote);
                IsBusy = false;
            }
        }

        public RootViewModel(IApp app) : base(app)
        {
            NavigationModel = new NavigationViewModel(app);

            // not logged in so push the login page
            MessagingCenter.Subscribe(this, "show-page", (NavigationViewModel sender, string page) => SetDetails(page));
            MessagingCenter.Subscribe(this, "logout", (object sender) =>
            {
                App.Remote.Logout();
                App.User = null;
                ShowLogin();
            });
        }

        public void SetDetails(string page)
        {
            var viewModel = CreateViewModel(page);
            MessagingCenter.Send(this, "show-page", viewModel);
        }

        private async void ShowLogin()
        {
            SetDetails(Profile);
            var loginModel = CreateViewModel(Login);
            await Navigation.PushModalAsync(loginModel);
        }

        private BaseViewModel CreateViewModel(string name)
        {
            switch (name)
            {
                case Events:
                    return new EventsViewModel(App);
                case Connections:
                    return new ConnectionsViewModel(App);
                case Profile:
                    return new ProfileViewModel(App);
                case Login:
                    return new LoginViewModel(App);
            }
            throw new Exception("Unknown model name " + name);
        }

    }
}