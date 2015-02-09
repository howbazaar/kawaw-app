using System;
using Kawaw.Exceptions;
using Kawaw.Framework;
using Xamarin.Forms;

namespace Kawaw
{
    class RootViewModel : BaseViewModel
    {
        public const string Events = "Events";
        public const string Connections = "Connections";
        public const string Notifications = "Notifications";
        public const string Profile = "Profile";
        public const string Login = "Login";

        private ViewModelNavigation _navigation;
        private BaseViewModel _contentModel;

        public override ViewModelNavigation Navigation
        {
            get { return _navigation; }
            set { _navigation = value; Init(); }
        }

        public NavigationViewModel NavigationModel { get; private set; }

        private void Init()
        {
            if (App.User == null)
            {
                ShowLogin();
            }
            else
            {
                RefreshUser(true);
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
            MessagingCenter.Subscribe(this, "refresh", (object sender) => RefreshUser());
            MessagingCenter.Subscribe(this, "action-started", (object sender) =>
            {
                IsBusy = true;
                if (_contentModel != null)
                    _contentModel.IsBusy = true;
            });
            MessagingCenter.Subscribe(this, "action-stopped", (object sender) =>
            {
                IsBusy = false;
                if (_contentModel != null)
                    _contentModel.IsBusy = false;
            });
            MessagingCenter.Subscribe(this, "session-expired", (object sender) =>
            {
                    App.Remote.Logout();
                    App.User = null;
                    ShowLogin();
            });
        }

        public async void RefreshUser(bool silent = false)
        {
            try
            {
                App.User.Remote = App.Remote;
                await App.User.Refresh();
            }
            catch (SessionExpiredException)
            {
                MessagingCenter.Send(this, "alert", new Alert
                {
                    Title = "Session Expired",
                    Text = "Your session has expired. Please log in again.",
                    Callback = new Command(() => MessagingCenter.Send((object)this, "session-expired")),
                });
            }
            catch (NetworkDownException ne)
            {
                if (!silent)
                {
                    MessagingCenter.Send(this, "alert", new Alert
                    {
                        Title = "Refresh Failed",
                        Text = ne.Message
                    });
                }
            }
            catch (Exception e)
            {
                MessagingCenter.Send(this, "alert", new Alert
                {
                    Title = "Refresh Failed",
#if DEBUG
                    Text = e.Message
#else
                    Text = "Something went wrong, please try again later"
#endif
                });
            }            
        }


        public void SetDetails(string page)
        {
            _contentModel = CreateViewModel(page);
            _contentModel.IsBusy = IsBusy;
            MessagingCenter.Send(this, "show-page", _contentModel);
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
                //case Notifications:
                //    return new NotificationsViewModel(App);
                case Profile:
                    return new ProfileViewModel(App);
                case Login:
                    return new LoginViewModel(App);
            }
            return new ProfileViewModel(App);
            // throw new Exception("Unknown model name " + name);
        }

    }
}