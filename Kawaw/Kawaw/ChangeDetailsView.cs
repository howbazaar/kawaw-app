using Xamarin.Forms;

namespace Kawaw
{
    class DatePopupView: BaseView
    {
        public DatePopupView()
        {
            Label header = new Label
            {
                Text = "DatePicker",
                Font = Font.SystemFontOfSize(50, FontAttributes.Bold),
                HorizontalOptions = LayoutOptions.Center
            };

            DatePicker datePicker = new DatePicker
            {
                Format = "D",
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            // Accomodate iPhone status bar.
            this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

            // Build the page.
            this.Content = new StackLayout
            {
                Children = 
                {
                    header,
                    datePicker
                }
            };
        }
    }
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

            var changeDOB = new Button
            {
                Text = "Change"
            };
            changeDOB.SetBinding(Button.CommandProperty, "ChangeDateOfBirthCommand");
            var clearDOB = new Button
            {
                Text = "Clear"
            };
            clearDOB.SetBinding(Button.CommandProperty, "ClearDateOfBirthCommand ");

            var dob = new Label
            {
                VerticalOptions = LayoutOptions.Center,
            };
            dob.SetBinding(Label.TextProperty, "DateOfBirth", BindingMode.Default,
                new OptionalDateConverter());

            Content = new StackLayout
            {
                Spacing = 10,
                Children =
                {
                    firstNameEntry,
                    lastNameEntry,
                    new Label {Text = "Address"},
                    addressEdit,
                    new Label{Text = "Date of Birth: "},
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            dob,
                            changeDOB,
                            clearDOB,
                        }
                    },

                    saveButton
                }
            };

        }
    }
}