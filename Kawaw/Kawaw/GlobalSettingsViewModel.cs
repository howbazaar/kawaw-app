using Kawaw.Framework;
using Kawaw.Models;

namespace Kawaw
{
    class GlobalSettingsViewModel : BaseViewModel
    {
        public GlobalSettingsViewModel(User user)
            : base(user)
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