namespace Kawaw
{
    class GlobalSettingsViewModel : BaseViewModel
    {
        public GlobalSettingsViewModel(IApp app)
            : base(app)
        {
            
        }
        private string _site;

        public string Site
        {
            get { return _site; }
            set { SetProperty(ref _site, value); }
        }

    }
}