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


            PopulateCategories();

            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;

            ToggleTheme(ThemeManager.IsDarkTheme);
           
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

        private void ThemeManager_ThemeChanged(object sender, EventArgs e)
        {
            // Clear any existing resource dictionaries to prepare for the theme switch.
            Resources.MergedDictionaries.Clear();
            var themeDict = new ResourceDictionary
            {
                Source = new Uri(ThemeManager.IsDarkTheme ? "LightMode.xaml" : "DarkMode.xaml", UriKind.Relative)
            };
            // Add the new theme dictionary to the window's resources, applying the new theme to the window.
            Resources.MergedDictionaries.Add(themeDict);
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
