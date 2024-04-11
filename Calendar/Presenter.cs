using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Calendar;
using System.IO;


namespace Calendar
{
    class Presenter
    {
        private readonly View _view;
        private readonly HomeCalendar _calendar;
        private string _lastUsedDirectory;

        public Presenter(View view)
        {
            _view = view;
            _calendar = new HomeCalendar("newdb.db", true); //NEED A DATABASE VARIABLE!
            _lastUsedDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        public bool ConfirmApplicationClosure()
        {
            return _view.ConfirmCloseApplication();
        }

        public void ChooseCalendarFileAndUpdateDirectory()
        {
            string selectedFile = _view.ShowFilePicker(_lastUsedDirectory);
            if (!string.IsNullOrEmpty(selectedFile))
            {
                _view.ShowMessage($"Selected Calendar File: {selectedFile}");
                _lastUsedDirectory = Path.GetDirectoryName(selectedFile);
            }
        }


        //public void LoadCategories()
        //{
        //    // Retrieve categories from the database using the Categories class in the Calendar DLL
        //    List<string> categoryDescriptions = new List<string>();

        //    try
        //    {
        //        Categories categories = new Categories(Database.dbConnection, false); // Use the existing database connection
        //        for (int i = 1; i <= 11; i++) // Assuming category IDs from 1 to 11
        //        {
        //            Category category = categories.GetCategoryFromId(i);
        //            categoryDescriptions.Add(category.Description);
        //        }

        //        _view.DisplayCategories(categoryDescriptions);
        //    }
        //    catch (Exception ex)
        //    {
        //        _view.ShowMessage($"Error loading categories: {ex.Message}");
        //    }
        //}

        //public void AddEvent()
        //{
        //    LoadCategories(); // Display available categories to choose from

        //    List<string> categories = new List<string>(); // Placeholder for category descriptions
        //    CalendarItem eventData = _view.EnterEventData(categories); // Display event entry form

        //    try
        //    {
        //        // Add the event to the calendar using the Events class in the Calendar DLL
        //        _calendar.events.Add(eventData.StartDateTime, eventData.CategoryID, eventData.DurationInMinutes, eventData.ShortDescription);
        //        _view.ShowEventAddedMessage();
        //    }
        //    catch (Exception ex)
        //    {
        //        _view.ShowMessage($"Error adding event: {ex.Message}");
        //    }
        //}
    }
}
