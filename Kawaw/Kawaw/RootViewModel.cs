using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Kawaw.Exceptions;
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
                try
                {
                    RefreshUserInternal();
                }
                catch (SessionExpiredException)
                {
                    App.Remote.Logout();
                    App.User = null;
                    ShowLogin();
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch (Exception)
                {
                    // swallow any exceptions here
                }
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
        }

        public void RefreshUser()
        {
            try
            {
                RefreshUserInternal();
            }
            catch (NetworkDownException e)
            {
                MessagingCenter.Send(this, "alert", new Alert
                {
                    Title = "Refresh Failed",
                    Text = e.Message
                });
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

        private async void RefreshUserInternal()
        {
            try
            {
                IsBusy = true;
                if (_contentModel != null)
                    _contentModel.IsBusy = true;
                await App.User.Refresh(App.Remote);
            }
            finally
            {
                IsBusy = false;
                if (_contentModel != null)
                    _contentModel.IsBusy = false;
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
                case Profile:
                    return new ProfileViewModel(App);
                case Login:
                    return new LoginViewModel(App);
            }
            throw new Exception("Unknown model name " + name);
        }

    }
}