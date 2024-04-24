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
    /// Interaction logic for EventReport.xaml
    /// </summary>
    public partial class EventReport : Window, View
    {
        private readonly Presenter _presenter;

        public EventReport(Presenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;

            DisplayDatabaseFile();
        }

        private void Add_Events(object sender, RoutedEventArgs e)
        {
            var newWindow = new Events_Categories(_presenter); //opens new window to add events
            newWindow.Show();
        }

        public bool ConfirmCloseApplication()
        {
            throw new NotImplementedException();
        }

        public void DisplayDatabaseFile()
        {
            DisplayDatabase.Text = "Database: " + System.IO.Path.GetFileName(_presenter.fileName);
        }

        public void ShowMessage(string message)
        {
            throw new NotImplementedException();
        }

        public void ShowTypes(List<Category.CategoryType> types)
        {
            throw new NotImplementedException();
        }
    }
}
