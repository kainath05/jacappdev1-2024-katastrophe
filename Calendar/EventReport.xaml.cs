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

            DataContext = this;

            DisplayDatabaseFile();

            var categories = _presenter._calendar.categories.List();
            DisplayCategories(categories);

            if (categories.Count > 0)
            {
                SelectedCategory = categories[0]; // Set the first category as the default selected category
            }

            // Load events
            LoadEvents();

            Closing += EventReport_Closing;
        }

        private void Delete_Event(object sender, RoutedEventArgs e)
        {
            var selected = sender as Event;
            if (selected != null)
            {
                _presenter.DeleteEvent(selected.Id);
                ShowMessage("Deleted event");
            }
        }

        private void Update_Event(object sender, RoutedEventArgs e)
        {
            var selected = sender as Event;
            if (selected != null)
            {
                //update method
                ShowMessage("Updated event");
            }
        }

        private void myDataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid != null)
            {
                var selected = dataGrid.SelectedItem as Event;
                if (selected != null)
                {
                    dataGrid.ContextMenu.DataContext = selected;
                }
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
        private ObservableCollection<CalendarItemsByMonth> _eventsByMonth = new ObservableCollection<CalendarItemsByMonth>();
        private ObservableCollection<CalendarItemsByCategory> _eventsByCategory = new ObservableCollection<CalendarItemsByCategory>();

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
        public ObservableCollection<CalendarItemsByMonth> EventsByMonth
        {
            get { return _eventsByMonth; }
            set
            {
                if (_eventsByMonth != value)
                {
                    _eventsByMonth = value;
                    NotifyPropertyChanged(nameof(EventsByMonth));
                }
            }
        }
        public ObservableCollection<CalendarItemsByCategory> EventsByCategory
        {
            get { return _eventsByCategory; }
            set
            {
                if (_eventsByCategory != value)
                {
                    _eventsByCategory = value;
                    NotifyPropertyChanged(nameof(EventsByCategory));
                }
            }
        }
        private bool _summaryByMonth;
        public bool SummaryByMonth
        {
            get => _summaryByMonth;
            set
            {
                if (_summaryByMonth != value)
                {
                    _summaryByMonth = value;
                    if (_summaryByMonth)
                        SummaryByCategory = false;
                    NotifyPropertyChanged(nameof(SummaryByMonth));
                    LoadEvents();
                }
            }
        }

        private bool _summaryByCategory;
        public bool SummaryByCategory
        {
            get => _summaryByCategory;
            set
            {
                if (_summaryByCategory != value)
                {
                    _summaryByCategory = value;
                    if (_summaryByCategory)
                        SummaryByMonth = false;
                    NotifyPropertyChanged(nameof(SummaryByCategory));
                    LoadEvents();
                }
            }
        }



        public void LoadEvents()
        {
            if (SummaryByMonth)
            {
                LoadEventsByMonth();
            }
            else if (SummaryByCategory)
            {
                LoadEventsByCategory();
            }
            else
            {
                LoadStandardEvents();
            }
        }



        private void LoadStandardEvents()
        {
            Events.Clear();
            int categoryId = (FilterByCategory && SelectedCategory != null) ? SelectedCategory.Id : 1;
            List<CalendarItem> events = _presenter.DisplayCalendarItems(StartDate, EndDate, FilterByCategory, categoryId);
            foreach (var ev in events)
            {
                Events.Add(ev);
            }
        }

        private void LoadEventsByMonth()
        {
            EventsByMonth.Clear();
            int categoryId = (FilterByCategory && SelectedCategory != null) ? SelectedCategory.Id : 1;
            List<CalendarItemsByMonth> events = _presenter.DisplayItemsByMonth(StartDate, EndDate, FilterByCategory, categoryId);
            foreach (var ev in events)
            {
                EventsByMonth.Add(ev);
            }
        }

        private void LoadEventsByCategory()
        {
            EventsByCategory.Clear();
            int categoryId = (FilterByCategory && SelectedCategory != null) ? SelectedCategory.Id : 1;
            List<CalendarItemsByCategory> events = _presenter.DisplayItemsByCategory(StartDate, EndDate, FilterByCategory, categoryId);
            foreach (var ev in events)
            {
                EventsByCategory.Add(ev);
            }
        }

        #endregion
    }
}
