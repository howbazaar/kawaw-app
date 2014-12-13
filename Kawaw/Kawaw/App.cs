using System.Diagnostics;
using Xamarin.Forms;

namespace Kawaw
{
    interface IApp
    {
        IRemoteSite Remote { get; }
        RemoteUser User { get; set; }
    }

    public class App : Application, IApp
    {
        private RemoteUser _user;

        // chevron is the rootview.master.icon
        // android is the page icon
        public App()
        {
            ViewModelNavigation.Register<LoginViewModel, LoginView>();
            ViewModelNavigation.Register<RegisterViewModel, RegisterView>();
            ViewModelNavigation.Register<EventsViewModel, EventsView>();
            ViewModelNavigation.Register<ConnectionsViewModel, ConnectionsView>();
            ViewModelNavigation.Register<ProfileViewModel, ProfileView>();
            ViewModelNavigation.Register<ChangeDetailsViewModel, ChangeDetailsView>();
            ViewModelNavigation.Register<NavigationViewModel, NavigationView>();
            ViewModelNavigation.Register<DatePopupViewModel, DatePopupView>();

            // Look to see if we have a logged in person.
            string csrftoken = null;
            string sessionid = null;
            if (Properties.ContainsKey("User"))
            {
                User = Properties["User"] as RemoteUser;
                Debug.WriteLine(User.FullName);
                csrftoken = User.CSRFToken;
                sessionid = User.SessionId;
            }
            Remote = new RemoteSite(csrftoken, sessionid);

            var rootModel = new RootViewModel(this);
            var rootView = new RootView
            {
                BindingContext = rootModel,
                Master = ViewModelNavigation.GetPageForViewModel(rootModel.NavigationModel),
                // Profile for now, should remember the last page.
                Detail = new NavigationPage(ViewModelNavigation.GetPageForViewModel(rootModel.ProfileModel))
            };

            MainPage = rootView;


        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnSleep()
        {
            base.OnSleep();
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        public IRemoteSite Remote { get; private set; }

        public RemoteUser User
        {
            get { return _user; }
            set
            {
                _user = value;
                // should also work with nil...
                if (_user != null)
                {
                    Properties["User"] = _user;
                }
                else
                {
                    Properties.Remove("User");
                }
            }
        }
    }

}
