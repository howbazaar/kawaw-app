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
            _navigation.PushModalAsync(new LoginViewModel());
        }

        public RootViewModel()
        {
            // not logged in so push the login page
            
        }
    }
}