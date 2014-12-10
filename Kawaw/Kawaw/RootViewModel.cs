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

        private void Init()
        {
            // check to see if logged in, and if not pushe the login page
            //_navigation.PushModalAsync(new LoginViewModel(_app));
        }

        public RootViewModel(IApp app) : base(app)
        {
            if (app.User == null)
            {

            }
            // not logged in so push the login page
            MessagingCenter.Subscribe(this, "show-page", (NavigationViewModel sender, string page) =>
            {
                Debug.WriteLine("show-page: {0}", page);
            });

        }
    }
}