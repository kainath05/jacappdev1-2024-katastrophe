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
        public HomeCalendar _calendar;
        public string fileName = "newdb.db";
        public bool newDB;

        public Presenter(View view)
        {
            _view = view;
            //if (_calendar != null) return;
            //InitializeCalendar();
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
                _view.UpdateCategoryList(categories);
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Failed to initialize form data due to database connection issues: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                _view.ShowMessage("Failed to create event: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
