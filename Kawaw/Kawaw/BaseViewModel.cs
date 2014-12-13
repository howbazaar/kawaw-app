using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Kawaw
{
    class BaseViewModel : INotifyPropertyChanged
    {
        // The name of the view model is usually only set for the main master details pages
        // and is used for the app to remember which was the last page.
        public string Name { get; private set; }
        public virtual ViewModelNavigation Navigation {get; set;}
        private bool isBusy;
        protected IApp App;

        public BaseViewModel(IApp app, string name = "")
        {
            App = app;
            Name = name;
        }

        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(storage, value))
                return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}