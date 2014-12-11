using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;

namespace Kawaw
{
    class ProfileViewModel : BaseViewModel
    {
        public string FullName { get { return App.User.FullName; } }
        public string Address { get { return App.User.Address == "" ? "<no address set>" : App.User.Address; } }
        public ICommand ChangeDetailsCommand{ get; private set; }

        public ProfileViewModel(IApp app)
            : base(app)
        {
            ChangeDetailsCommand = new Command(async () =>
            {
                await Navigation.PushAsync(new ChangeDetailsViewModel(app));
            });
            MessagingCenter.Subscribe<ChangeDetailsViewModel>(this, "user-updated", delegate
            {
                OnPropertyChanged("FullName");
                OnPropertyChanged("Address");
            });
        }

    }
}