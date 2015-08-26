using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;
using Kawaw.Exceptions;
using Kawaw.Framework;
using Kawaw.JSON;
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
        public ICommand ClearDateOfBirthCommand { get; private set; }

        public ChangeDetailsViewModel(IApp app)
            : base(app)
        {
            FirstName = app.User.FirstName;
            LastName = app.User.LastName;
            Address = app.User.Address;
            DateOfBirth = app.User.DateOfBirth;

            SaveCommand = new Command(async () =>
            {
                try
                {
                    await app.User.UpdateUserDetails(FirstName, LastName, Address, DateOfBirth);
                    await Navigation.PopAsync();
                }
                catch (SessionExpiredException)
                {
                    MessagingCenter.Send(this, "alert", new Alert
                    {
                        Title = "Session Expired",
                        Text = "Your session has expired. Please log in again.",
                        Callback = new Command(async () =>
                        {
                            await Navigation.PopAsync();
                            MessagingCenter.Send((object) this, "session-expired");
                        }),
                    });
                }
                catch (Exception e)
                {
                    MessagingCenter.Send(this, "alert", new Alert
                    {
                        Title = "Changing Details Failed",
                        Text = e.Message
                    });
                }
            });
            ClearDateOfBirthCommand = new Command(() => { DateOfBirth = new DateTime(0); });
        }
    }
}