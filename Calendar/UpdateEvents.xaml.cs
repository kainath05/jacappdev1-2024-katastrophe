using Calendar.views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private readonly int _eventId;
        private readonly int _categoryId;
        private readonly double _duration;
        private readonly DateTime _date;
        private readonly string _details;
        public UpdateEvents(Presenter presenter, int eventId, DateTime date, int categoryId, double duration, string details)
        {
            InitializeComponent();
            _presenter = presenter;
            _eventId = eventId;
            _date = date;
            _categoryId = categoryId;
            _duration = duration;
            _details = details;

            DataContext = this;

            DisplayDatabaseFile();

            var categories = _presenter._calendar.categories.List();
            DisplayCategories(categories);
            ClearForm();
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

        private bool ValidateInput()
        {
            bool isValid = true;
            string errorMessage = "";

            if (string.IsNullOrWhiteSpace(EventDetailsTextBox.Text))
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
                ShowMessage(errorMessage.Trim()); //shows error message if no valid input
            }

            return isValid;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                ShowMessage("Please correct the input fields.");
                return;
            }

            string details = EventDetailsTextBox?.Text;
            if (string.IsNullOrEmpty(details))
            {
                ShowMessage("Please enter a description.");
                return;
            }

            if (CategoryComboBox.SelectedItem == null)
            {
                ShowMessage("Please select a valid category type."); //validates input
                return;
            }
            Category cat = CategoryComboBox.SelectedItem as Category;
            int categoryId = cat.Id;

            _presenter.UpdateEvent(_eventId, StartDatePicker.SelectedDate.Value, categoryId, double.Parse(DurationTextBox.Text), details);

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            Category cat = _presenter._calendar.categories.GetCategoryFromId(_categoryId);
            StartDatePicker.SelectedDate = _date;
            CategoryComboBox.SelectedItem = CategoryComboBox.Items.Cast<Category>().FirstOrDefault(c => c.Id == _categoryId);
            DurationTextBox.Text = _duration.ToString();
            EventDetailsTextBox.Text = _details;
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            _presenter.DeleteEvent(_eventId);
        }
       

        public void ShowTypes(List<Category.CategoryType> types)
        {
            //dont need this 
        }
    }
}
