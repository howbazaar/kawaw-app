using System.Diagnostics;
using Xamarin.Forms;

namespace Kawaw
{
    class RootViewModel : BaseViewModel
    {
        private ViewModelNavigation _navigation;

        public override ViewModelNavigation Navigation
        {
            get { return _navigation; }
            set { _navigation = value; Init(); }
        }

        public NavigationViewModel NavigationModel { get; private set; }
        public EventsViewModel EventsModel { get; private set; }
        public ConnectionsViewModel ConnectionsModel { get; private set; }
        public LoginViewModel LoginModel { get; private set; }

        private async void Init()
        {
            if (App.User == null)
            {
                await Navigation.PushModalAsync(LoginModel);
            }
            // TODO: listen to login-needed events from other senders.
            //MessagingCenter.Subscribe(this, "login-needed", async (RootViewModel sender, LoginViewModel model) =>
           // {
             //   await Navigation.PushModalAsync(ViewModelNavigation.GetPageForViewModel(model));
            //});
        }

        public RootViewModel(IApp app) : base(app)
        {
            NavigationModel = new NavigationViewModel(app);
            EventsModel = new EventsViewModel(app);
            ConnectionsModel = new ConnectionsViewModel(app);
            LoginModel = new LoginViewModel(app);

            // not logged in so push the login page
            MessagingCenter.Subscribe(this, "show-page", (NavigationViewModel sender, string page) =>
            {
                Debug.WriteLine("show-page: {0}", page);
                switch (page)
                {
                    case "Events":
                        MessagingCenter.Send<RootViewModel, BaseViewModel>(this, "show-page", EventsModel);
                        break;
                    case "Connections":
                        MessagingCenter.Send<RootViewModel, BaseViewModel>(this, "show-page", ConnectionsModel);
                        break;
                    default:
                        Debug.WriteLine("Unknown page {0}", page);
                        break;
                }
            });
            MessagingCenter.Subscribe(this, "logout", async (object sender) =>
            {
                App.Remote.Logout();
                App.User = null;
                await Navigation.PushModalAsync(LoginModel);
            });
        }
    }
}