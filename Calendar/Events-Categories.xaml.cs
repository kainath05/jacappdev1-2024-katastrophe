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

namespace Calendar
{
    /// <summary>
    /// Interaction logic for Events_Categories.xaml
    /// </summary>
    public partial class Events_Categories : Window, View
    {
        private readonly Presenter _presenter;

        public Events_Categories(Presenter presenter)
        {
            InitializeComponent();

            _presenter = presenter;

            InitializeForm();
        }

        public bool ConfirmCloseApplication()
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string message)
        {
            throw new NotImplementedException();
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
                MessageBox.Show("Please correct the input fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Extracting date components
                int year = EventDatePicker.SelectedDate.Value.Year;
                int month = EventDatePicker.SelectedDate.Value.Month;
                int day = EventDatePicker.SelectedDate.Value.Day;

                // Extracting time components
                int hour = int.Parse(HourComboBox.SelectedItem.ToString());
                int minute = int.Parse(MinuteComboBox.SelectedItem.ToString());
                int second = int.Parse(SecondComboBox.SelectedItem.ToString());

                // Handling 12-hour clock and PM designation
                if (AmPmComboBox.SelectedItem.ToString() == "PM" && hour < 12)
                {
                    hour += 12;
                }
                else if (AmPmComboBox.SelectedItem.ToString() == "AM" && hour == 12)
                {
                    hour = 0; // Midnight case
                }

                // Constructing the DateTime
                DateTime selectedDateTime = new DateTime(year, month, day, hour, minute, second);

                // Proceed with using the constructed DateTime
                int categoryId = (int)CategoryComboBox.SelectedValue;
                double duration = double.Parse(DurationTextBox.Text);
                string details = EventDetailsTextBox.Text;

                // Use HomeCalendar to add the event to the database
                _presenter._calendar.events.Add(selectedDateTime, categoryId, duration, details);

                MessageBox.Show("Event successfully added!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to create event: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }
        #endregion

        private void ClearForm()
        {
            // Reset Event Details TextBox
            EventDetailsTextBox.Text = "Enter event details here...";
            EventDetailsTextBox.Foreground = System.Windows.Media.Brushes.Gray;

            // Reset DatePicker
            EventDatePicker.SelectedDate = null;

            // Reset Time ComboBoxes
            HourComboBox.SelectedIndex = -1;
            MinuteComboBox.SelectedIndex = -1;
            SecondComboBox.SelectedIndex = -1;
            AmPmComboBox.SelectedIndex = -1;

            // Reset Duration TextBox
            DurationTextBox.Text = "Duration in minutes";
            DurationTextBox.Foreground = System.Windows.Media.Brushes.Gray;

            // Reset Category ComboBox
            CategoryComboBox.SelectedIndex = -1;
        }

        private bool ValidateInput()
        {
            // Validate Event Details
            if (string.IsNullOrWhiteSpace(EventDetailsTextBox.Text) || EventDetailsTextBox.Text == "Enter event details here...")
            {
                MessageBox.Show("Please enter event details.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validate Date
            if (!EventDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a date.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validate Time
            if (HourComboBox.SelectedItem == null || MinuteComboBox.SelectedItem == null || AmPmComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please complete the time selection.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validate Duration
            if (!int.TryParse(DurationTextBox.Text, out int duration) || duration <= 0)
            {
                MessageBox.Show("Please enter a valid duration in minutes.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validate Category
            if (CategoryComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a category.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void InitializeForm()
        {
            try
            {
                var categories = _presenter._calendar.categories.List();
                Dispatcher.Invoke(() => UpdateComboBoxes(categories));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to initialize form data due to database connection issues: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateComboBoxes(List<Category> categories)
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
    
    }

}
