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
        }

        private void LoadDefaultFolderLocations()
        {
            FolderComboBox.Items.Add("Documents/Calendars");
            FolderComboBox.Items.Add("Desktop");
            FolderComboBox.Items.Add("Downloads");

            FolderComboBox.SelectedIndex = 0; // Set the first item as default
        }

        private void OpenFileExplorer_Click(object sender, RoutedEventArgs e)
        {
            string selectedFile = ShowFilePicker(_lastUsedDirectory);
            if (!string.IsNullOrEmpty(selectedFile))
            {
                ShowMessage($"Selected Calendar File: {selectedFile}");
                _lastUsedDirectory = System.IO.Path.GetDirectoryName(selectedFile);

                FolderComboBox.Items.Add(_lastUsedDirectory); 
                FileNameTextBox.Text = System.IO.Path.GetFileNameWithoutExtension(selectedFile);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (_presenter.ConfirmApplicationClosure())
            {
                Application.Current.Shutdown();
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = initialDirectory;
            openFileDialog.Filter = "All Files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
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
                    using (File.Create(fullPath)) { }
                    ShowMessage("New database created successfully.");
                }
                catch (Exception ex)
                {
                    ShowMessage($"Error creating file: {ex.Message}");
                    return;
                }
            }

            // Set presenter properties based on whether a new database was created or an existing one is used
            _presenter.fileName = fullPath;
            _presenter.newDB = !databaseExists;
            
            var newWindow = new Events_Categories(fullPath);
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

    }
}