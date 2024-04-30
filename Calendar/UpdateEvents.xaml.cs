using Calendar.views;
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
    /// Interaction logic for UpdateEvents.xaml
    /// </summary>
    public partial class UpdateEvents : Window, View
    {
        private readonly Presenter _presenter;
        public UpdateEvents(Presenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            DataContext = this;

            ShowTypes(_presenter.DisplayTypes());

            DisplayDatabaseFile();

            var categories = _presenter._calendar.categories.List();
            DisplayCategories(categories);

        }

        public bool ConfirmCloseApplication()
        {
            MessageBoxResult result = MessageBox.Show("Do you want to save changes and exit?", "Confirm Exit", MessageBoxButton.YesNoCancel);
            return result == MessageBoxResult.Yes;
        }

        public void DisplayCategories(List<Category> categories)
        {
            foreach (Category category in categories)
            {
                CategoryComboBox.Items.Add(category);
            }

        }

        public void DisplayDatabaseFile()
        {
            DisplayDatabase.Text = "Database: " + System.IO.Path.GetFileName(_presenter.fileName);
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public void ShowMessage(string message, string title = "Information", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.Information)
        {
            MessageBox.Show(message, title, button, icon);
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

            if (!StartDatePicker.SelectedDate.HasValue)
            {
                errorMessage += "Please select a date.\n";
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

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                ShowMessage("Please correct the input fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string details = EventDetailsTextBox?.Text;
            if (string.IsNullOrEmpty(details))
            {
                ShowMessage("Please enter a description to add a new category.");
                return;
            }

            if (CategoryComboBox.SelectedItem == null)
            {
                ShowMessage("Please select a valid category type."); //validates input
                return;
            }
            DateTime selectedDate = StartDatePicker.SelectedDate.Value;
            int categoryId = (int)CategoryComboBox.SelectedValue;
            double duration = double.Parse(DurationTextBox.Text);

            //_presenter.UpdateEvent(eventId, selectedDate, categoryId, duration, details); //TODO: How to add eventId

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            
        }

        private void ClearForm()
        {
            StartDatePicker.SelectedDate = null;
            CategoryComboBox.SelectedIndex = -1;
            EventDetailsTextBox.Text = null;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
                //_presenter.DeleteEvent(selected.Id); TODO
                ShowMessage("Deleted event");
        }
       

        public void ShowTypes(List<Category.CategoryType> types)
        {
            foreach (Category.CategoryType item in types)
            {
                CategoryComboBox.Items.Add(item); //fill in drop list for types
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "Enter event details here..." || textBox.Text == "Duration in minutes")
            {
                textBox.Text = string.Empty;
                textBox.Foreground = Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Foreground = Brushes.Gray;
                textBox.Text = textBox.Name == "EventDetailsTextBox" ? "Enter event details here..." : "Duration in minutes";
            }
        }
    }
}
