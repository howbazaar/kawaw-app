using System;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;

namespace Kawaw
{
    class AddEmailViewModel : BaseViewModel
    {
        private string _email;

        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }
        public ICommand AddCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public AddEmailViewModel(IApp app)
            :base(app)
        {
            AddCommand = new Command(async () =>
            {
                try
                {
                    // TODO: disable add and cancel while this is running.
                    var user = await app.Remote.AddEmail(Email);
                    app.User.UpdateUser(user);
                    await Navigation.PopAsync();
                }
                catch (Exception)
                {
                    Debug.WriteLine("oops");
                }
            });
            CancelCommand = new Command(async () =>
            {
                await Navigation.PopAsync();
            });
        }        
         
    }
}