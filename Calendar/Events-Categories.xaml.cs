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

        public void ShowMessage(string message)
        {
            throw new NotImplementedException();
        }

        private void Categories_Button(object sender, RoutedEventArgs e)
        {
            var window = new Categories();
            window.Show();
            Close();
        }
    }
}
