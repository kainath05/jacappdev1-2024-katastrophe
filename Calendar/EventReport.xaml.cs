using Calendar.views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
    public partial class EventReport : Window, View, ViewForReport, INotifyPropertyChanged
    {
        private readonly Presenter _presenter;
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public EventReport(Presenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            _presenter.SetReportView(this); //adds new view

            DisplayDatabaseFile();

            Closing += EventReport_Closing; // for exit button

            DataContext = this;
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
            if (CategoryComboBox.Items.Count > 0)
            {
                SelectedCategory = (Category)CategoryComboBox.Items[0];
            }
        }

        #region DataGridControls

        private DateTime _startDate = DateTime.Today;
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    NotifyPropertyChanged(nameof(StartDate));
                    LoadEvents();
                }
            }
        }

        private DateTime _endDate = DateTime.Today;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    NotifyPropertyChanged(nameof(EndDate));
                    LoadEvents();
                }
            }
        }

        private bool _filterByCategory;
        public bool FilterByCategory
        {
            get { return _filterByCategory; }
            set
            {
                if (_filterByCategory != value)
                {
                    _filterByCategory = value;
                    NotifyPropertyChanged(nameof(FilterByCategory));
                    LoadEvents();
                }
            }
        }

        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    NotifyPropertyChanged(nameof(SelectedCategory));
                    LoadEvents();
                }
            }
        }

        private ObservableCollection<CalendarItem> _events = new ObservableCollection<CalendarItem>();
        public ObservableCollection<CalendarItem> Events
        {
            get { return _events; }
            set
            {
                if (_events != value)
                {
                    _events = value;
                    NotifyPropertyChanged(nameof(Events));
                }
            }
        }


        public void LoadEvents()
        {
            Events.Clear();
            int categoryId = FilterByCategory ? SelectedCategory.Id : 1; // TODO FIX 1
            List<CalendarItem> events = _presenter.DisplayCalendarItems(StartDate, EndDate, FilterByCategory, categoryId);

            foreach (var ev in events)
            {
                Events.Add(ev);
            }
        }

        #endregion
    }
}
