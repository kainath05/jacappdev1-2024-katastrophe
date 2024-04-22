using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;

namespace Calendar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, View
    {
        private readonly Presenter _presenter;
        private string _lastUsedDirectory;

        public MainWindow()
        {
            InitializeComponent();
            _presenter = new Presenter(this);

            LoadDefaultFolderLocations();
            _lastUsedDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            ApplyTheme();
        }
       
        private void ThemeManager_ThemeChanged(object sender, EventArgs e)
        {
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            Resources.MergedDictionaries.Clear();
            string themeUri = ThemeManager.IsDarkTheme ? "DarkMode.xaml" : "LightMode.xaml";
            Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(themeUri, UriKind.Relative) });
            grid.Background = ThemeManager.IsDarkTheme ? new SolidColorBrush(Color.FromRgb(30, 30, 30)) : Brushes.LightGray;
        }

        private void LoadDefaultFolderLocations()
        {
            FolderComboBox.Items.Add("Documents/Calendars");
            FolderComboBox.Items.Add("Desktop");
            FolderComboBox.Items.Add("Downloads");

            FolderComboBox.SelectedIndex = 0; 
        }

        private void OpenFileExplorer_Click(object sender, RoutedEventArgs e)
        {
            string selectedFile = ShowFilePicker(_lastUsedDirectory); 
            if (!string.IsNullOrEmpty(selectedFile))
            {
                ShowMessage($"Selected Calendar File: {selectedFile}"); //selects file and folder from file explorer
                _lastUsedDirectory = System.IO.Path.GetDirectoryName(selectedFile);

                FolderComboBox.Items.Add(_lastUsedDirectory); 
                FileNameTextBox.Text = System.IO.Path.GetFileNameWithoutExtension(selectedFile);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (_presenter.ConfirmApplicationClosure())
            {
                Application.Current.Shutdown(); //shutdown the program
            }
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

        public string ShowFilePicker(string initialDirectory)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog(); //opens file explorer
            openFileDialog.InitialDirectory = initialDirectory;
            openFileDialog.Filter = "All Files (*.*)|*.*"; //all files
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName; //returns the name we pick
            }

            return null;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string fileName = FileNameTextBox.Text.Trim();
            string selectedFolder = FolderComboBox.SelectedItem?.ToString();

            string folderPath = GetFolderPath(selectedFolder);

            if (string.IsNullOrEmpty(fileName))
            {
                ShowMessage("Please enter a calendar file name.");
                return;
            }

            string fullPath = System.IO.Path.Combine(folderPath, $"{fileName}.db");

            bool databaseExists = File.Exists(fullPath);

            if (!databaseExists)
            {
                try
                {
                    using (File.Create(fullPath)) { } //creates a new database file
                    ShowMessage("New database created successfully.");
                }
                catch (Exception ex)
                {
                    ShowMessage($"Error creating file: {ex.Message}");
                    return;
                }
            }

            _presenter.fileName = fullPath;
            _presenter.newDB = !databaseExists;

            _presenter.InitializeCalendar(); //makes a home calendar

            var newWindow = new Events_Categories(_presenter); //opens new window to add events
            newWindow.Show();
            Close();
        }

        private string GetFolderPath(string selectedFolder)
        {
            switch (selectedFolder)
            {
                case "Desktop":
                    return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                case "Downloads":
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                case "Documents/Calendars":
                    string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    return System.IO.Path.Combine(myDocuments, selectedFolder);
                default:
                    string selectedFile = ShowFilePicker(_lastUsedDirectory);
                    return selectedFile;
            }
        }

        // Method to toggle the theme based on the current theme setting.
        private void ToggleTheme()
        {
            ThemeManager.IsDarkTheme = !ThemeManager.IsDarkTheme;

            Application.Current.Resources.MergedDictionaries.Clear();
            string themeUri = ThemeManager.IsDarkTheme ? "DarkMode.xaml" : "LightMode.xaml";
            var backgroundColor = ThemeManager.IsDarkTheme ? new SolidColorBrush(Color.FromRgb(30, 30, 30)) : Brushes.LightGray;
            grid.Background = backgroundColor;
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(themeUri, UriKind.Relative) });
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ToggleTheme();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleTheme();
        }


        public void DisplayDatabaseFile()
        {
            throw new NotImplementedException(); //only for events and categories window to show db file
        }

        public void ShowTypes(List<Category.CategoryType> types)
        {
            //does not need this method
        }

    }
}