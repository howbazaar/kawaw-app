using System;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;

namespace Kawaw
{
    class ChangeDetailsViewModel : BaseViewModel
    {
        private string _firstName;
        private string _lastName;
        private string _address;
        private DateTime _dateOfBirth;

        public string FirstName
        {
            get { return _firstName; }
            set { SetProperty(ref _firstName, value); }
        }

        public string LastName
        {
            get { return _lastName; }
            set { SetProperty(ref _lastName, value); }
        }

        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }

        public DateTime DateOfBirth
        {
            get { return _dateOfBirth; }
            set { SetProperty(ref _dateOfBirth, value); }
        }

        public ICommand SaveCommand { get; private set; }

        public ChangeDetailsViewModel(IApp app)
            :base(app)
        {
            FirstName = app.User.FirstName;
            LastName = app.User.LastName;
            Address = app.User.Address;
            DateOfBirth = app.User.DateOfBirth;

            SaveCommand = new Command(async () =>
            {
                var jsonUser = await app.Remote.UpdateUserDetails(FirstName, LastName, Address, DateOfBirth);
                app.User.UpdateUser(jsonUser);
                MessagingCenter.Send(this, "user-updated");
                await Navigation.PopAsync();
            });

        }        
    }
}