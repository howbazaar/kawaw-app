using System;
using System.Diagnostics;
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
            if (App.User.Authenticated)
            {
                RefreshUser(true);
            }
            else
            {
                ShowLogin();
            }
        }

        public RootViewModel(IApp app) : base(app)
        {
            NavigationModel = new NavigationViewModel(app);

            // not logged in so push the login page
            MessagingCenter.Subscribe(this, "show-page", (NavigationViewModel sender, string page) => SetDetails(page));
            MessagingCenter.Subscribe(this, "logout", async (object sender) =>
            {
                Debug.WriteLine("logout");
                await App.User.Logout();
                Debug.WriteLine("show login");
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
                {
                    _contentModel.IsBusy = false;
                    Debug.WriteLine("busy is false");                    
                }
            });
            MessagingCenter.Subscribe(this, "session-expired", async (object sender) =>
            {
                    await App.User.Logout();
                    ShowLogin();
            });
            // TODO: determine if I really need the following two.
            MessagingCenter.Subscribe(this, "unregister-device", (object sender) =>
            {
                Debug.WriteLine("unregister-device");
                if (App.User.Authenticated)
                {
                    App.User.UnregisterDevice();
                }
            });
            MessagingCenter.Subscribe(this, "register-device", async (object sender) =>
            {
                if (App.User.Authenticated)
                {
                    await App.User.RegisterDevice();
                }
            });
            MessagingCenter.Subscribe(this, "show event", (object sender, int id) =>
            {
                Debug.WriteLine("show event {0}", id);
                SetDetails(Events);
            });
            MessagingCenter.Subscribe(this, "show notification", (object sender, int id) =>
            {
                Debug.WriteLine("show notification {0}", id);
                SetDetails(Notifications);
            });
            MessagingCenter.Subscribe(this, "show connections", (object sender) =>
            {
                Debug.WriteLine("show connections");
                SetDetails(Connections);
            });
        }

        public async void RefreshUser(bool silent = false)
        {
            try
            {
                await App.User.Refresh();
            }
            catch (SessionExpiredException)
            {
                // TODO: try to login again using saved credentials.
                // If that then fails, then we get the user to try.
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
            var loginModel = new LoginViewModel(App);
            await Navigation.PushLoginAsync(loginModel);
        }

        private BaseViewModel CreateViewModel(string name)
        {
            switch (name)
            {
                case Events:
                    return new EventsViewModel(App);
                case Connections:
                    return new ConnectionsViewModel(App);
                case Notifications:
                    return new NotificationsViewModel(App);
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