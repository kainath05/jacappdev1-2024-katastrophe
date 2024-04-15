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
        #endregion

        

    }
}
