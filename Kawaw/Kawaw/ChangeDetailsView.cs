using System;
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

            var largeFont = Font.SystemFontOfSize(NamedSize.Large);

            var datepicker = new DatePicker();
            datepicker.SetBinding(DatePicker.DateProperty, "DateOfBirth");

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

            var clearDOB = new Button
            {
                Text = "Clear"
            };
            clearDOB.SetBinding(Button.CommandProperty, "ClearDateOfBirthCommand ");

            var dob = new Label
            {
                HorizontalOptions= LayoutOptions.StartAndExpand,
                // FontSize = NamedSize.Large  <--- Why not?
            };
            dob.SetBinding(Label.TextProperty, "DateOfBirth", BindingMode.Default,
                new OptionalDateConverter());
            var absolute = new AbsoluteLayout();
            var view = new StackLayout
            {
                Spacing = 10,
                Children =
                {
                    firstNameEntry,
                    lastNameEntry,
                    new Label {Text = "Address", FontSize = largeFont.FontSize},
                    addressEdit,
                    new Label{Text = "Date of Birth: "},
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            dob,
                            clearDOB,
                        }
                    },

                    saveButton
                }
            };
            absolute.Children.Add(view, new Rectangle(0,0,1,1), AbsoluteLayoutFlags.All);
            absolute.Children.Add(datepicker, new Rectangle(-300,-300,100,30));
            Content = absolute;

            var tap = new TapGestureRecognizer();
            tap.Tapped += (sender, args) =>
            {
                if (datepicker.Date == datepicker.MinimumDate)
                {
                    datepicker.Date = DateTime.Today;
                }
                datepicker.Focus();
            };
            dob.GestureRecognizers.Add(tap);
        }
    }
}