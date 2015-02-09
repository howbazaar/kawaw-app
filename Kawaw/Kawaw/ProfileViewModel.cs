using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Kawaw.Exceptions;
using Xamarin.Forms;

namespace Kawaw
{
    struct Alert
    {
        public string Title;
        public string Text;
        public ICommand Callback;
    }
    struct EmailAction
    {
        public Email Email;
        public string Name;
    }

    struct EmailActionOptions
    {
        public Email Email;
        public List<Tuple<string, string>> Options;
    }

    class ProfileViewModel : BaseViewModel
    {
        private string _fullName;
        private string _address;
        private string _dateOfBirth;
        private IList<Email> _emails;
        private Email _selectedItem;

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

        public IList<Email> Emails
        {
            get { return _emails; }
            private set { SetProperty(ref _emails, value); }
        }

        public Email SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                var changed = SetProperty(ref _selectedItem, value);
                if (value == null || !changed) return;

                SelectedItem = null;

                // Look at the email and send a message...
                if (value.Primary && value.Verified)
                {
                    MessagingCenter.Send(this, "alert", new Alert
                    {
                        Title = "E-mail Action",
                        Text = "You cannot delete your primary email address.",
                    });
                }
                else
                {
                    var options = new EmailActionOptions
                    {
                        Email = value,
                        Options = new List<Tuple<string, string>>(),
                    };
                    if (value.Verified)
                    {
                        options.Options.Add(new Tuple<string, string>("primary", "Set as primary e-mail"));
                    }
                    else
                    {
                        options.Options.Add(new Tuple<string, string>("send", "Resend verification e-mail"));
                    }
                    if (!value.Primary)
                    {
                        options.Options.Add(new Tuple<string, string>("remove", "Remove this e-mail address"));
                    }

                    MessagingCenter.Send(this, "show-options", options);
                }
            }
        }

        public ICommand ChangeDetailsCommand { get; private set; }
        public ICommand AddEmailCommand { get; private set; }

        public ProfileViewModel(IApp app)
            : base(app, RootViewModel.Profile)
        {
            UpdateFromUser(app.User);

            ChangeDetailsCommand = new Command(async () =>
            {
                await Navigation.PushAsync(new ChangeDetailsViewModel(app));
            });
            AddEmailCommand = new Command(async () =>
            {
                await Navigation.PushAsync(new AddEmailViewModel(app));
            });
            MessagingCenter.Subscribe<object>(this, "user-updated", delegate
            {
                UpdateFromUser(app.User);
            });
            MessagingCenter.Subscribe(this, "email-action", async (object sender, EmailAction action) =>
            {
                try
                {
                    var user = await app.Remote.EmailAction(action.Name, action.Email.Address);
                    app.User.UpdateUser(user);
                }
                catch (SessionExpiredException)
                {
                    MessagingCenter.Send(this, "alert", new Alert
                    {
                        Title = "Session Expired",
                        Text = "Your session has expired. Please log in again.",
                        Callback = new Command(() => MessagingCenter.Send((object)this, "session-expired")),
                    });
                }
                catch (Exception e)
                {
                    MessagingCenter.Send(this, "alert", new Alert
                    {
                        Title = "Add Email Failed",
                        Text = e.Message
                    });
                }
            });

        }

        private void UpdateFromUser(RemoteUser user)
        {
            if (user == null) return;

            FullName = user.FullName;
            Address = user.Address == "" ? "no address set" : user.Address;
            DateOfBirth = RemoteUser.OptionalDateTime(user.DateOfBirth, "not set");
            Emails = new ObservableCollection<Email>(
                from email in user.Emails
                orderby email.Primary descending, email.Verified descending, email.Address
                select email);
            MessagingCenter.Send(this, "emails-updated");
        }

    }
}