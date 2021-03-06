using Kawaw.Models;

namespace Kawaw.Framework
{
    class BaseViewModel : BaseProperties
    {
        // The name of the view model is usually only set for the main master details pages
        // and is used for the app to remember which was the last page.
        public string Name { get; private set; }
        public virtual ViewModelNavigation Navigation {get; set;}

        protected User User { get; private set; }

        private bool _isBusy;

        // Autoproperty find because the binding is one way to source, and no one is listening to change events.
        public virtual bool IsPageVisible { get; set; }

        public BaseViewModel(User user, string name = "")
        {
            User = user;
            Name = name;
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }
    }
}