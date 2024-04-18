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
    /// Interaction logic for Categories.xaml
    /// </summary>
    public partial class Categories : Window, View
    {
        private readonly Presenter _presenter;
        public Categories(Presenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;

            ShowTypes(_presenter.DisplayTypes());

            DisplayDatabaseFile();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (_presenter.ConfirmApplicationClosure())
            {
                Application.Current.Shutdown();
            }
        }

        private void Add_Category(object sender, RoutedEventArgs e)
        {
            try
            {
                string descr = Description?.Text;
                if (string.IsNullOrEmpty(descr))
                {
                    ShowMessage("Please enter a description to add a new category.");
                    return;
                }

                if (Type.SelectedItem == null)
                {
                    ShowMessage("Please select a valid category type."); //validates input
                    return;
                }

                Category.CategoryType type = (Category.CategoryType)Type.SelectedItem;
                _presenter.AddCategory(descr, type);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding category: {ex.Message}");
            }
        }

        private void Go_To_Events(object sender, RoutedEventArgs e)
        {
            var window = new Events_Categories(_presenter);
            window.Show();
            Close();
        }


        public bool ConfirmCloseApplication()
        {
            MessageBoxResult result = MessageBox.Show("Do you want to save changes and exit?", "Confirm Exit", MessageBoxButton.YesNoCancel);
            return result == MessageBoxResult.Yes;
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Message");
        }

        public void DisplayDatabaseFile()
        {
            DisplayDatabase.Text = "Database: " + System.IO.Path.GetFileName(_presenter.fileName);
        }

        public void ShowTypes(List<Category.CategoryType> types)
        {
            foreach (Category.CategoryType item in types)
            {
                Type.Items.Add(item); //fill in drop list for types
            }
        }
    }
}
