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

namespace Calendar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, View
    {
        private readonly Presenter _presenter;
        public MainWindow()
        {
            InitializeComponent();
            _presenter = new Presenter(this);

            LoadDefaultFolderLocations();
        }

        private void LoadDefaultFolderLocations()
        {
            // Populate the ComboBox with default folder locations
            FolderComboBox.Items.Add("Documents/Calendars");
            FolderComboBox.Items.Add("Desktop");
            FolderComboBox.Items.Add("Downloads");

            FolderComboBox.SelectedIndex = 0; // Set the first item as default
        }

        private void OpenFileExplorer_Click(object sender, RoutedEventArgs e)
        {
            _presenter.ChooseCalendarFile();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Call presenter method to confirm application closure
            if (_presenter.ConfirmApplicationClosure())
            {
                // Close the application
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

        public string ShowFolderPicker(string defaultFolder)
        {
            //var dialog = new System.Windows.Forms.FolderBrowserDialog();
            //dialog.SelectedPath = defaultFolder;

            //if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    return dialog.SelectedPath;
            //}

            return null;
        }

        public void DisplayCategories(List<string> categories)
        {
        }

        public CalendarItem EnterEventData(List<string> categories)
        {
            return null;
        }

        public void ShowEventAddedMessage()
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}