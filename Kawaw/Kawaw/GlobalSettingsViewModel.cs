namespace Kawaw
{
    class GlobalSettingsViewModel : BaseViewModel
    {
        private string _site;

        public string Site
        {
            get { return _site; }
            set { SetProperty(ref _site, value); }
        }

    }
}