using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Calendar;
using System.IO;
using System.Windows;

namespace Calendar
{
    public class Presenter
    {
        private readonly View _view;
        private IAddEvent _addEventView;
        public HomeCalendar _calendar;
        public string fileName = "newdb.db";
        public bool newDB;

        public Presenter(View view)
        {
            _view = view;
            //if (_calendar != null) return;
            //InitializeCalendar();
        }

        public void SetAddEventView(IAddEvent addEventView)
        {
            _addEventView = addEventView;
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
       
        public List<Category.CategoryType> DisplayTypes()
        {
            List<Category.CategoryType> types = new List<Category.CategoryType>();
            foreach (Category.CategoryType item in Enum.GetValues(typeof(Category.CategoryType)))
            {
                types.Add(item);
            }
            return types;
        }

        public void AddCategory(string descr, Category.CategoryType type)
        {
            _calendar.categories.Add(descr, type);  
        }
    }
}
