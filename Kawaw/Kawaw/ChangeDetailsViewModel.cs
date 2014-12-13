using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;
using Kawaw.JSON;
using Xamarin.Forms;

namespace Kawaw
{
    class DatePopupViewModel : BaseViewModel
    {
        private DateTime _value;

        public DateTime Value
        {
            get { return _value; }
            set
            {
                // we don't want DateTime(0), so use today
                if (value == new DateTime(0)) value = DateTime.Today;
                SetProperty(ref _value, value);
            }
        }

        public DatePopupViewModel(IApp app, DateTime initial) : base(app)
        {
            Value = initial;
        }
    }

    class ChangeDetailsViewModel : BaseViewModel
    {
        private string _firstName;
        private string _lastName;
        private string _address;
        private DateTime _dateOfBirth;

        private DatePopupViewModel _datePicker;

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
        public ICommand ClearDateOfBirthCommand { get; private set; }
        public ICommand ChangeDateOfBirthCommand { get; private set; }

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
                MessagingCenter.Send<object>(this, "user-updated");
                await Navigation.PopAsync();
            });
            ClearDateOfBirthCommand = new Command(() => { DateOfBirth = new DateTime(0);});

            _datePicker = new DatePopupViewModel(app, app.User.DateOfBirth);
            // ChangeDateOfBirthCommand = new Command(() => Navigation.PushAsync(_datePicker));
            MessagingCenter.Subscribe(this, "done", (DatePopupViewModel model, DateTime date) =>
            {
                Debug.WriteLine("got {0} back", date.ToString());
            });
        }        
    }
}