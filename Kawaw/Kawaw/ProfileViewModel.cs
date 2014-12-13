using System;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;

namespace Kawaw
{
    class ProfileViewModel : BaseViewModel
    {
        private string _fullName;
        private string _address;
        private string _dateOfBirth;

        public string FullName
        {
            get { return _fullName; }
            private set { SetProperty(ref _fullName, value); }
        }

        public string Address
        {
            get { return _address; }
            private set { SetProperty(ref _address, value); }
        }

        public string DateOfBirth
        {
            get { return _dateOfBirth; }
            private set { SetProperty(ref _dateOfBirth, value); }
        }

        public ICommand ChangeDetailsCommand{ get; private set; }

        public ProfileViewModel(IApp app)
            : base(app)
        {
            UpdateFromUser(app.User);

            ChangeDetailsCommand = new Command(async () =>
            {
                await Navigation.PushAsync(new ChangeDetailsViewModel(app));
            });
            MessagingCenter.Subscribe<object>(this, "user-updated", delegate
            {
                UpdateFromUser(app.User);
            });
        }

        private void UpdateFromUser(RemoteUser user)
        {
            if (user == null) return;

            FullName = user.FullName;
            Address = user.Address == "" ? "no address set" : user.Address;
            DateOfBirth = RemoteUser.OptionalDateTime(user.DateOfBirth);
        }

    }
}