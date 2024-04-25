using Calendar.views;
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
    public partial class EventReport : Window, View, ViewForReport
    {
        private readonly Presenter _presenter;

        public EventReport(Presenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            _presenter.SetReportView(this); //adds new view

            DisplayDatabaseFile();

            Closing += EventReport_Closing; // for exit button
        }

        private void Delete_Event(object sender, RoutedEventArgs e)
        {
            var selected = myDataGrid.SelectedItem as Event;
            if (selected != null)
            {
                _presenter.DeleteEvent(selected.Id);
                ShowMessage("Deleted event");
            }
        }

        private void Update_Event(object sender, RoutedEventArgs e)
        {
            var selected = myDataGrid.SelectedItem as Event;
            if (selected != null)
            {
                //update method
                ShowMessage("Updated event");
            }
        }

        private void myDataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var selected = myDataGrid.SelectedItem as Event;
            if (selected != null)
            {
                myDataGrid.ContextMenu.DataContext = selected;
            }
        }

        private void EventReport_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool shouldClose = ConfirmCloseApplication();

            if (shouldClose)
            {
                // Perform any necessary actions (e.g., save data)
                // Continue with closing the window
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void Add_Events(object sender, RoutedEventArgs e)
        {
            var newWindow = new Events_Categories(_presenter); //opens new window to add events
            newWindow.Show();
        }

        public bool ConfirmCloseApplication()
        {
            MessageBoxResult result = MessageBox.Show("Do you want to save changes and exit?", "Confirm Exit", MessageBoxButton.YesNoCancel);
            return result == MessageBoxResult.Yes;
        }

        public void DisplayDatabaseFile()
        {
            DisplayDatabase.Text = "Database: " + System.IO.Path.GetFileName(_presenter.fileName);
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public void ShowTypes(List<Category.CategoryType> types)
        {
            throw new NotImplementedException(); //dont need for this
        }

        public void DisplayCategories(List<Category> categories)
        {
            foreach (Category category in categories)
            {
                CategoryComboBox.Items.Add(category);
            }
        }
    }
}
