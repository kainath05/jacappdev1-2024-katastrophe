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
                _presenter.newDB = false;
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

            //openFileDialog.InitialDirectory = _lastUsedDirectory;

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

            if (string.IsNullOrEmpty(fileName))
            {
                ShowMessage("Please enter a calendar file name.");
                return;
            }

            if (string.IsNullOrEmpty(selectedFolder))
            {
                ShowMessage("Please select a folder location.");
                return;
            }

            string folderPath = selectedFolder switch
            {
                "Desktop" => Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "Downloads" => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                _ => System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), selectedFolder)
            };

            string fullPath = System.IO.Path.Combine(folderPath, $"{fileName}.db");

            _presenter.fileName = fullPath;
            _presenter.newDB = !File.Exists(fullPath);

            if (_presenter.newDB)
            {
                try
                {
                    File.Create(fullPath).Close();
                    ShowMessage("New database created successfully.");
                }
                catch (Exception ex)
                {
                    ShowMessage($"Error creating file: {ex.Message}");
                    return;
                }
            }

            _presenter.InitializeCalendar();

            // Assuming Events_Categories is correctly prepared to use the initialized calendar
            var newWindow = new Events_Categories();
            newWindow.Show();
            this.Close();
        }

    }
}