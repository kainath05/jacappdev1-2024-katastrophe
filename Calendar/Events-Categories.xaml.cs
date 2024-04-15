using System;
using System.Collections.Generic;
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
        public Events_Categories()
        {
            InitializeComponent();
        }

        public bool ConfirmCloseApplication()
        {
            throw new NotImplementedException();
        }

        public string ShowFilePicker(string initialDirectory)
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string message)
        {
            throw new NotImplementedException();
        }

        #region Button
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) // <- TODO
            {
                MessageBox.Show("Please correct the input fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Proceed with adding the event
            MessageBox.Show("Event successfully added!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            ClearForm();
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

            // Validate Time - assuming these ComboBoxes have their items set correctly
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


    }
}
