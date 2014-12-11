using Xamarin.Forms;

namespace Kawaw
{
    class ChangeDetailsView : BaseView
    {
        public ChangeDetailsView()
        {
            Title = "Change Details";
            Icon = "kawaw.png";
            Padding = new Thickness(20);

            var firstNameEntry = new Entry
            {
                Placeholder = "First name"
            };
            firstNameEntry.SetBinding(Entry.TextProperty, "FirstName");
            var lastNameEntry = new Entry
            {
                Placeholder = "Last name"
            };
            lastNameEntry.SetBinding(Entry.TextProperty, "LastName");

            var addressEdit = new Editor
            {
                HeightRequest = 120
            };
            addressEdit.SetBinding(Editor.TextProperty, "Address");

            var dobEntry = new DatePicker
            {
            };
            dobEntry.SetBinding(DatePicker.DateProperty, "DateOfBirth");

            var saveButton = new Button
            {
                Text = "Save"
            };
            saveButton.SetBinding(Button.CommandProperty, "SaveCommand");

            Content = new StackLayout
            {
                Spacing = 10,
                Children =
                {
                    firstNameEntry,
                    lastNameEntry,
                    new Label {Text = "Address"},
                    addressEdit,
                    dobEntry,
                    saveButton
                }
            };

        }
    }
}