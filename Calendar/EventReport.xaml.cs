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
using System.Xml.Serialization;

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
            var selected = sender as MenuItem;
            if (selected != null)
            {
                ContextMenu menu = selected.Parent as ContextMenu;
                if (menu != null)
                {
                    DataGrid grid = menu.PlacementTarget as DataGrid;
                    if (grid != null)
                    {
                        CalendarItem item = grid.SelectedItem as CalendarItem;
                        if (item != null)
                        {
                            _presenter.DeleteEvent(item.EventID);
                            LoadEvents();
                        }
                    }
                }
            }
        }

        private void Update_Event(object sender, RoutedEventArgs e)
        {

            var selected = sender as MenuItem;
            if (selected != null)
            {
                ContextMenu menu = selected.Parent as ContextMenu;
                if (menu != null)
                {
                    DataGrid grid = menu.PlacementTarget as DataGrid;
                    if (grid != null)
                    {
                        CalendarItem item = grid.SelectedItem as CalendarItem;
                        if (item != null)
                        {
                            var newWindow = new UpdateEvents(_presenter, item.EventID, item.StartDateTime, item.CategoryID, item.DurationInMinutes, item.ShortDescription); //opens new window to update events
                            newWindow.Show();
                            LoadEvents();
                        }
                    }
                }
            }
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid != null)
            {
                if (dataGrid.SelectedItem is CalendarItem selected)
                {
                    var newWindow = new UpdateEvents(_presenter, selected.EventID, selected.StartDateTime, selected.CategoryID, selected.DurationInMinutes, selected.ShortDescription); //opens new window to update events
                    newWindow.Show();
                    LoadEvents();
                }
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
            CategoryComboBox.ItemsSource = categories;
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
        private ObservableCollection<Dictionary<string, object>> _eventsByCategoryAndMonth = new ObservableCollection<Dictionary<string, object>>(); //newww

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
        public ObservableCollection<Dictionary<string, object>> EventsByCategoryAndMonth
        {
            get { return _eventsByCategoryAndMonth; }
            set
            {
                if (_eventsByCategoryAndMonth != value)
                {
                    _eventsByCategoryAndMonth = value;
                    NotifyPropertyChanged(nameof(EventsByCategoryAndMonth));
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
                    NotifyPropertyChanged(nameof(SummaryByCategory));
                    LoadEvents();
                }
            }
        }

        private bool _summaryByCategoryAndMonth;
        public bool SummaryByCategoryAndMonth
        {
            get => _summaryByCategoryAndMonth;
            set
            {
                if (_summaryByCategoryAndMonth != value)
                {
                    _summaryByCategoryAndMonth = value;
                    NotifyPropertyChanged(nameof(SummaryByCategoryAndMonth));
                    LoadEvents();
                }
            }
        }

        public void LoadEvents()
        {
            if (SummaryByMonth && SummaryByCategory)
            {
                SummaryByCategoryAndMonth = true;
                LoadEventsByCategoryAndMonth();
            }
            else if (SummaryByCategory)
            {
                LoadEventsByCategory();
            }
            else if (SummaryByMonth)
            {
                LoadEventsByMonth();
            }
            else
            {
                LoadStandardEvents();
            }
        }

        private void LoadStandardEvents()
        {
            Events.Clear();
            LoadCalendarItemColumns();
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
            LoadCalendarItemsByMonth();
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
            LoadCalendarItemsByCategory();
            int categoryId = (FilterByCategory && SelectedCategory != null) ? SelectedCategory.Id : 1;
            List<CalendarItemsByCategory> events = _presenter.DisplayItemsByCategory(StartDate, EndDate, FilterByCategory, categoryId);
            foreach (var ev in events)
            {
                EventsByCategory.Add(ev);
            }
        }

        private void LoadEventsByCategoryAndMonth()
        {
            EventsByCategoryAndMonth.Clear();
            LoadCalendarItemsByCategoryAndMonth();
            int categoryId = (FilterByCategory && SelectedCategory != null) ? SelectedCategory.Id : 1;
            List<Dictionary<string, object>> items = _presenter.DisplayItemsByCategoryAndMonth(StartDate, EndDate, FilterByCategory, categoryId);
            foreach (var ev in items)
            {
                EventsByCategoryAndMonth.Add(ev);
            }
        }

        #endregion

        private void LoadCalendarItemColumns()
        {
            regularDataGrid.Columns.Clear();
            regularDataGrid.AutoGenerateColumns = false;
            var date = new DataGridTextColumn();
            date.Header = "Start Date";
            date.Binding = new Binding("StartDateTime");
            date.Binding.StringFormat = "dd/MM/yyyy";
            regularDataGrid.Columns.Add(date);

            var time = new DataGridTextColumn();
            time.Header = "Start Time";
            time.Binding = new Binding("StartDateTime");
            time.Binding.StringFormat = "HH:mm:ss";
            regularDataGrid.Columns.Add(time);

            var category = new DataGridTextColumn();
            category.Header = "Category";
            category.Binding = new Binding("Category");
            regularDataGrid.Columns.Add(category);

            var description = new DataGridTextColumn();
            description.Header = "Description";
            description.Binding = new Binding("ShortDescription");
            regularDataGrid.Columns.Add(description);

            var columnDuration = new DataGridTextColumn();
            columnDuration.Header = "Duration";
            columnDuration.Binding = new Binding("DurationInMinutes");
            regularDataGrid.Columns.Add(columnDuration);


            var columnBusyTime = new DataGridTextColumn();
            columnBusyTime.Header = "Busy Time";
            columnBusyTime.Binding = new Binding("BusyTime");
            regularDataGrid.Columns.Add(columnBusyTime);
        }

        private void LoadCalendarItemsByMonth()
        {
            monthlyDataGrid.Columns.Clear();
            monthlyDataGrid.AutoGenerateColumns = false;
            var month = new DataGridTextColumn();
            month.Header = "Month";
            month.Binding = new Binding("Month");
            monthlyDataGrid.Columns.Add(month);

            var columnTotal = new DataGridTextColumn();
            columnTotal.Header = "Total Busy Time";
            columnTotal.Binding = new Binding("TotalBusyTime");
            monthlyDataGrid.Columns.Add(columnTotal);
        }

        private void LoadCalendarItemsByCategory()
        {
            categoryDataGrid.Columns.Clear();
            categoryDataGrid.AutoGenerateColumns = false;
            var category = new DataGridTextColumn();
            category.Header = "Category";
            category.Binding = new Binding("Category");
            categoryDataGrid.Columns.Add(category);

            var columnTotal = new DataGridTextColumn();
            columnTotal.Header = "Total Busy Time";
            columnTotal.Binding = new Binding("TotalBusyTime");
            categoryDataGrid.Columns.Add(columnTotal);
        }

        private void LoadCalendarItemsByCategoryAndMonth()
        {
            dictionaryDataGrid.Columns.Clear();
            dictionaryDataGrid.AutoGenerateColumns = false;

            var month = new DataGridTextColumn(); 
            month.Header = "Month";
            month.Binding = new Binding("[Month]");
            dictionaryDataGrid.Columns.Add(month);

            List<Category> categories = _presenter._calendar.categories.List();

            // Create a column for each category
            foreach (Category category in categories)
            {
                var categoryColumn = new DataGridTextColumn
                {
                    Header = category,
                    Binding = new Binding("[" + category + "]") 
                };
                dictionaryDataGrid.Columns.Add(categoryColumn);
            }

            var columnTotal = new DataGridTextColumn();
            columnTotal.Header = "Total Busy Time";
            columnTotal.Binding = new Binding("[TotalBusyTime]");
            dictionaryDataGrid.Columns.Add(columnTotal);
        }
    }
}
