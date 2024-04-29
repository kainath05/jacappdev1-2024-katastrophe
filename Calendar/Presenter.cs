using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Calendar;
using System.IO;
using System.Windows;
using Calendar.views;
using System.Collections.ObjectModel;

namespace Calendar
{
    public class Presenter
    {
        private readonly View _view;
        public IAddEvent _addEventView; //changed the private for test
        public ViewForReport _reportView;
        public HomeCalendar _calendar;
        public string fileName = "newdb.db";
        public bool newDB;

        public Presenter(View view)
        {
            _view = view;
        }

        public void SetAddEventView(IAddEvent addEventView)
        {
            _addEventView = addEventView;
        }

        public void SetReportView(ViewForReport reportView)
        {
            _reportView = reportView;
            _reportView.DisplayCategories(ListOfCategories());
        }


        public void InitializeCalendar()
        {
            // Check for _calendar reinitialization logic if necessary
            _calendar = newDB ? new HomeCalendar(fileName, true) : new HomeCalendar(fileName);
        }

        public bool ConfirmApplicationClosure()
        {
            return _view.ConfirmCloseApplication();
        }

        public void InitializeForm()
        {
            try
            {
                var categories = _calendar.categories.List();
                _addEventView.UpdateComboBoxes(categories);
            }
            catch (Exception ex)
            {
                _addEventView.ShowMessage("Failed to initialize form data due to database connection issues: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AddEvent(DateTime dateTime, int categoryId, double duration, string details)
        {
            try
            {
                _calendar.events.Add(dateTime, categoryId, duration, details);
                _view.ShowMessage("Event successfully added!");
            }
            catch (Exception ex)
            {
                _addEventView.ShowMessage("Failed to create event: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
       
        public List<Category.CategoryType> DisplayTypes()
        {
            List<Category.CategoryType> types = new List<Category.CategoryType>();
            foreach (Category.CategoryType item in Enum.GetValues(typeof(Category.CategoryType)))
            {
                types.Add(item);
            }
            return types;
        }

        private List<Category> ListOfCategories()
        {
            List<Category> list = _calendar.categories.List();
            return list;
        }

        public void AddCategory(string descr, Category.CategoryType type)
        {
            var list = _calendar.categories.List();
            foreach (Category category in list)
            {
                if (descr == category.Description)
                {

                    _view.ShowMessage("Category already exists.");
                    return; 
                }
            }
            if (!Enum.IsDefined(typeof(Category.CategoryType), type))
            {
                _view.ShowMessage("Invalid category type.");
                return; 
            }
            _calendar.categories.Add(descr, type);
            _view.ShowMessage("Category added.");
        }

        public void DeleteEvent(int id)
        {
            try
            {
                _calendar.events.Delete(id);
                _view.ShowMessage("Event deleted.");
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Failed to delete event: " + ex.Message);
            }
        }

        //Need update event too

        public List<CalendarItem> DisplayCalendarItems(DateTime start, DateTime end, bool filter, int categoryId)
        {
            return _calendar.GetCalendarItems(start, end, filter, categoryId);
        }

        public List<CalendarItemsByMonth> DisplayItemsByMonth(DateTime start, DateTime end, bool filter, int categoryId)
        {
            return _calendar.GetCalendarItemsByMonth(start, end, filter, categoryId);
        }

        public List<CalendarItemsByCategory> DisplayItemsByCategory(DateTime start, DateTime end, bool filter, int categoryId)
        {
            return _calendar.GetCalendarItemsByCategory(start, end, filter, categoryId);
        }

        public void DisplayItemsByCategoryAndMonth(DateTime start, DateTime end, bool filter, int categoryId)
        {
            _calendar.GetCalendarDictionaryByCategoryAndMonth(start, end, filter, categoryId);

        }


        
    }
}

