using Xamarin.Forms;

namespace Kawaw
{
    public class App : Application
    {
        public UserProfile Profile;

        // chevron is the rootview.master.icon
        // android is the page icon
        public App()
        {
            ViewModelNavigation.Register<LoginViewModel, LoginView>();
            ViewModelNavigation.Register<RegisterViewModel, RegisterView>();
            ViewModelNavigation.Register<EventsViewModel, EventsView>();
            ViewModelNavigation.Register<ConnectionsViewModel, ConnectionsView>();
            ViewModelNavigation.Register<NavigationViewModel, NavigationView>();

            var rootModel = new RootViewModel();
            var rootView = new RootView {BindingContext = rootModel};

            Profile = GetProfile();
            
            MainPage = rootView;
        }

        private UserProfile GetProfile()
        {
            if (!Properties.ContainsKey("UserProfile"))
            {
                Properties["UserProfile"] = new UserProfile();
            }

            return (UserProfile)Properties["UserProfile"];
        }

        private void SetProfile()
        {
            Properties["UserProfile"] = Profile;
        }

        protected override void OnResume()
        {
            Profile.Note("OnResume");
            base.OnResume();
        }

        protected override void OnSleep()
        {
            Profile.Note("OnSleep");
            base.OnSleep();
        }

        protected override void OnStart()
        {
            Profile.Note("OnStart");
            base.OnStart();
        }

    }

}
