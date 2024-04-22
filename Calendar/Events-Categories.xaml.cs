using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace Calendar
{
    /// <summary>
    /// Interaction logic for Events_Categories.xaml
    /// </summary>
    public partial class Events_Categories : Window, View, IAddEvent
    {
        private readonly Presenter _presenter;

        public Events_Categories(Presenter presenter)
        {

            InitializeComponent();

            _presenter = presenter;

            _presenter.SetAddEventView(this); //adds new view

            _presenter.InitializeForm();


            // Subscribe to the ThemeChanged event from the ThemeManager.
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;

            ToggleTheme(!ThemeManager.IsDarkTheme);

        }
        // Event handler for when the theme is changed.
        private void ThemeManager_ThemeChanged(object sender, EventArgs e)
        {
            // Clear existing resource dictionaries to refresh themes.
            Resources.MergedDictionaries.Clear();
            // Load the appropriate theme based on the current setting in ThemeManager.
            var themeDict = new ResourceDictionary
            {
                Source = new Uri(ThemeManager.IsDarkTheme ? "DarkMode.xaml" : "LightMode.xaml", UriKind.Relative)
            };
            // Add the loaded dictionary to the window's resources.
            Resources.MergedDictionaries.Add(themeDict);
        }

        private void SetTimeAfter30Mins()
        {

            DateTime time = DateTime.Now.AddMinutes(30);

            HourComboBox.SelectedItem = time.Hour;
            MinuteComboBox.SelectedItem = time.ToString("mm");
            SecondComboBox.SelectedItem = time.ToString("ss");
            AmPmComboBox.SelectedIndex = -1;

        }


        public bool ConfirmCloseApplication()
        {
            MessageBoxResult result = MessageBox.Show("Do you want to save changes and exit?", "Confirm Exit", MessageBoxButton.YesNoCancel);
            return result == MessageBoxResult.Yes;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (_presenter.ConfirmApplicationClosure())
            {
                Application.Current.Shutdown();
            }
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public void ShowMessage(string message, string title = "Information", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.Information)
        {
            MessageBox.Show(message, title, button, icon);
        }

        private void Categories_Button(object sender, RoutedEventArgs e)
        {
            var window = new Categories(_presenter);
            window.Show();
            Close();
        }

        #region Button
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                ShowMessage("Please correct the input fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Extracting date and time components
            var selectedDate = EventDatePicker.SelectedDate.Value;
            int hour = int.Parse(HourComboBox.SelectedItem.ToString());
            int minute = int.Parse(MinuteComboBox.SelectedItem.ToString());
            int second = int.Parse(SecondComboBox.SelectedItem.ToString());

            // Adjusting hour for 12-hour clock format
            if (AmPmComboBox.SelectedItem.ToString() == "PM" && hour < 12)
                hour += 12;
            else if (AmPmComboBox.SelectedItem.ToString() == "AM" && hour == 12)
                hour = 0;

            // Creating DateTime object
            DateTime selectedDateTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, hour, minute, second);

            // Data from form
            int categoryId = (int)CategoryComboBox.SelectedValue;
            double duration = double.Parse(DurationTextBox.Text);
            string details = EventDetailsTextBox.Text;

            // Request the presenter to add the event
            _presenter.AddEvent(selectedDateTime, categoryId, duration, details);
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }
        #endregion

        private void ClearForm()
        {
            // Reset DatePicker
            EventDatePicker.SelectedDate = null;

            // Reset Time ComboBoxes
            HourComboBox.SelectedItem = -1;
            MinuteComboBox.SelectedItem = -1;
            SecondComboBox.SelectedItem = -1;
            AmPmComboBox.SelectedIndex = -1;

            // Reset Category ComboBox
            CategoryComboBox.SelectedIndex = -1;
        }

        private bool ValidateInput()
        {
            bool isValid = true;
            string errorMessage = "";

            if (string.IsNullOrWhiteSpace(EventDetailsTextBox.Text) || EventDetailsTextBox.Text == "Enter event details here...")
            {
                errorMessage += "Please enter event details.\n";
                isValid = false;
            }

            if (!EventDatePicker.SelectedDate.HasValue)
            {
                errorMessage += "Please select a date.\n";
                isValid = false;
            }

            if (HourComboBox.SelectedItem == null || MinuteComboBox.SelectedItem == null || AmPmComboBox.SelectedItem == null)
            {
                errorMessage += "Please complete the time selection.\n";
                isValid = false;
            }

            if (!int.TryParse(DurationTextBox.Text, out int duration) || duration <= 0)
            {
                errorMessage += "Please enter a valid duration in minutes.\n";
                isValid = false;
            }

            if (CategoryComboBox.SelectedItem == null)
            {
                errorMessage += "Please select a category.\n";
                isValid = false;
            }

            if (!isValid)
            {
                ShowMessage(errorMessage.Trim(), "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning); //shows error message if no valid input
            }

            return isValid;
        }

        public void UpdateComboBoxes(List<Category> categories)
        {
            HourComboBox.ItemsSource = Enumerable.Range(1, 12).ToList();
            MinuteComboBox.ItemsSource = Enumerable.Range(0, 60).Select(i => i.ToString("00")).ToList();
            SecondComboBox.ItemsSource = Enumerable.Range(0, 60).Select(i => i.ToString("00")).ToList();
            AmPmComboBox.ItemsSource = new[] { "AM", "PM" };

            HourComboBox.SelectedIndex = 0;
            MinuteComboBox.SelectedIndex = 0;
            SecondComboBox.SelectedIndex = 0;
            AmPmComboBox.SelectedIndex = 0;

            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.DisplayMemberPath = "Description";  
            CategoryComboBox.SelectedValuePath = "Id";
            CategoryComboBox.SelectedIndex = categories.Any() ? 0 : -1;
        }

        // Method to toggle the theme based on the current theme setting.
        private void ToggleTheme(bool useDarkTheme)
        {
            Application.Current.Resources.MergedDictionaries.Clear();
            string themeUri;
            if (useDarkTheme)
            {
                themeUri = "DarkMode.xaml";
                var darkGrayColor = new SolidColorBrush(Color.FromRgb(30, 30, 30));
                grid.Background = darkGrayColor;
            }
            else
            {
                themeUri = "LightMode.xaml";
                grid.Background = Brushes.LightGray;
            }
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(themeUri, UriKind.Relative) });
        }
        private void ToggleThemeButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle the theme and update the UI accordingly.
            bool newThemeIsDark = !ThemeManager.IsDarkTheme;
            ThemeManager.IsDarkTheme = newThemeIsDark;  

            ToggleTheme(newThemeIsDark);
        }

        public void DisplayDatabaseFile()
        {
            DisplayDatabase.Text = "Database: " + System.IO.Path.GetFileName(_presenter.fileName);
        }

        public void ShowTypes(List<Category.CategoryType> types)
        {
            //does not need this method
        }
    }


    }
}
