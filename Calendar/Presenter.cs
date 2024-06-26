﻿using System;
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
using System.Windows.Markup;
using System.Data.Entity;
using System.Drawing;
using System.Threading.Channels;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Calendar
{
    public class Presenter
    {
        /// <summary>
        /// The view associated with this presenter.This is used to interact directly with the user interface.
        /// </summary>
        private readonly View _view;

        /// <summary>
        /// Interface for adding events, exposed for unit testing.Allows injection of a mock or alternate implementation for testing purposes.
        /// </summary>
        public IAddEvent _addEventView; //changed the private for test
        
        /// <summary>
        /// View for generating and displaying reports.This view is responsible for presenting statistical and analytical data.
        ///</summary>
        public ViewForReport _reportView;

        /// <summary>
        /// Represents the main calendar model.This handles all data and operations related to calendar events and categories.
        /// </summary>
        public HomeCalendar _calendar;

        /// <summary>
        /// File name of the database where calendar data is stored.Defaults to 'newdb.db'.
        /// </summary>
        /// <value>
        /// The default file name is 'newdb.db', but this can be changed to point to a different database file as needed.
        /// </value>
        public string fileName = "newdb.db";

        /// <summary>
        /// Indicates whether a new database should be initialized or an existing one should be used.This affects how the HomeCalendar is initialized.
        /// </summary>
        /// <value>
        /// True if a new database should be created; false if the existing database should be used.
        /// </value>
        public bool newDB;

        /// <summary>
        /// Constructor for Presenter.Initializes a new instance of the Presenter class with the specified view.
        /// </summary>
        /// <param name = "view" > The view that this presenter will manage.</param>

        public Presenter(View view)
        {
            _view = view;
        }

        /// summary>
        /// Assigns a view responsible for adding events.
        /// </summary>
        /// <param name = "addEventView" > The view to be used for adding events.</param>
        /// <example>
        /// Here is how you can use this method:
        /// <code>
        /// AddEventView addEventView = new AddEventView();
        /// presenter.SetAddEventView(addEventView);
        /// </code>
        /// </example>
        public void SetAddEventView(IAddEvent addEventView)
        {
            _addEventView = addEventView;
        }

        /// <summary>
        /// Assigns a view responsible for reporting.Also initializes the report view with a list of categories.
        /// </summary>
        /// <param name = "reportView" > The report view to set.</param>
        /// <example>
        /// Here is how you can use this method:
        /// <code>
        /// ReportView reportView = new ReportView();
        /// presenter.SetReportView(reportView);
        /// </code>
        /// </example>
        public void SetReportView(ViewForReport reportView)
        {
            _reportView = reportView;
            _reportView.DisplayCategories(ListOfCategories());
        }

        /// <summary>
        /// Initializes the calendar based on the value of<see cref= "newDB" />.
        /// </summary>
        /// <example>
        /// Here is how you can use this method:
        /// <code>
        /// presenter.newDB = true; // To initialize a new database
        /// presenter.InitializeCalendar();
        /// </code>
        /// </example>
        public void InitializeCalendar()
        {
            // Check for _calendar reinitialization logic if necessary
            _calendar = newDB ? new HomeCalendar(fileName, true) : new HomeCalendar(fileName);
        }

        /// <summary>
        /// Confirms with the user through the view if the application should be closed.This can be used to prompt for unsaved changes or final confirmations.
        /// </summary>
        /// <returns>Returns true if the application closure is confirmed, otherwise false.</returns>
        /// <example>
        /// <code>
        /// bool shouldClose = presenter.ConfirmApplicationClosure();
        /// if (shouldClose)
        /// {
        ///     Application.Current.Shutdown();
        /// }
        /// else
        /// {
        ///     // Handle the case where the user decides not to close the application
        ///     Console.WriteLine("Closure cancelled by user.");
        /// }
        /// </code>
        /// </example>
        public bool ConfirmApplicationClosure()
        {
            return _view.ConfirmCloseApplication();
        }

        /// <summary>
        /// Initializes form elements such as combo boxes with data loaded from the calendar model.This method populates UI components with current data.
        /// </summary>
        /// <exception cref="Exception">Thrown when there is an issue with database connectivity.</exception>
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

        /// <summary>
        /// Adds an event to the calendar.
        /// </summary>
        /// <param name="dateTime">The date and time of the event.</param>
        /// <param name="categoryId">The identifier of the category under which the event is classified.</param>
        /// <param name="duration">The duration of the event in hours.</param>
        /// <param name="details">A detailed description of the event.</param>
        /// <exception cref="Exception">Thrown when the event cannot be created due to database or other issues.</exception>
        /// <example>
        /// Here is how you can use this method:
        /// <code>
        /// DateTime eventDateTime = new DateTime(2024, 12, 25, 10, 0, 0);
        /// int categoryId = 1; // Assuming 1 is the ID for 'Holiday'
        /// double duration = 2.0; // 2 hours
        /// string details = "Christmas Celebration";
        /// presenter.AddEvent(eventDateTime, categoryId, duration, details);
        /// </code>
        /// </example>
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

        /// <summary>
        /// Retrieves all types of categories defined in the Category.CategoryType enumeration.
        /// </summary>
        /// <returns>A list containing all the category types.</returns>
        /// <example>
        /// <code>
        /// List<Category.CategoryType> categoryTypes = presenter.DisplayTypes();
        /// foreach (var type in categoryTypes)
        /// {
        ///     Console.WriteLine(type);
        ///     // Add type to a dropdown or any other UI component
        /// }
        /// </code>
        /// </example>
        public List<Category.CategoryType> DisplayTypes()
        {
            List<Category.CategoryType> types = new List<Category.CategoryType>();
            foreach (Category.CategoryType item in Enum.GetValues(typeof(Category.CategoryType)))
            {
                types.Add(item);
            }
            return types;
        }


        /// <summary>
        /// Gets a list of all categories from the calendar model.
        /// </summary>
        /// <returns>A list of categories.</returns>
        private List<Category> ListOfCategories()
        {
            List<Category> list = _calendar.categories.List();
            return list;
        }

        /// <summary>
        /// Adds a new category to the calendar.
        /// </summary>
        /// <param name="descr">The description of the new category.</param>
        /// <param name="type">The type of the new category, as defined in the Category.CategoryType enumeration.</param>
        /// <exception cref="Exception">Thrown if the category already exists or if an invalid type is specified.</exception>
        /// <example>
        /// Here is how you can use this method:
        /// <code>
        /// string description = "Meeting";
        /// Category.CategoryType type = Category.CategoryType.Work;
        /// presenter.AddCategory(description, type);
        /// </code>
        /// </example>
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

        /// <summary>
        /// Deletes an event from the calendar.
        /// </summary>
        /// <param name="id">The identifier of the event to be deleted.</param>
        /// <exception cref="Exception">Thrown when the event cannot be deleted due to database or other issues.</exception>
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

        /// <summary>
        /// Retrieves a list of calendar items within a specific date range and optional category filtering.
        /// </summary>
        /// <param name="start">The start date of the range.</param>
        /// <param name="end">The end date of the range.</param>
        /// <param name="filter">Indicates whether to apply category filtering.</param>
        /// <param name="categoryId">The category identifier to filter by, if filtering is enabled.</param>
        /// <returns>A list of calendar items that fall within the specified range and, if specified, the category.</returns>
        /// <example>
        /// Here is how you can use this method:
        /// <code>
        /// int eventIdToUpdate = 102;
        /// DateTime newDate = new DateTime(2024, 6, 15, 10, 0, 0);
        /// int newCategoryId = 2;
        /// double newDuration = 1.5;
        /// string newDetails = "Updated Team Meeting";
        /// presenter.UpdateEvent(eventIdToUpdate, newDate, newCategoryId, newDuration, newDetails);
        /// </code>
        /// </example>
        public void UpdateEvent(int eventId, DateTime dateTime, int categoryId, double duration, string details)
        {
                try
                {
                    _calendar.events.UpdateProperties(eventId, dateTime, categoryId, duration, details);
                    _view.ShowMessage("Event updated.");
                }
                catch (Exception ex)
                {
                    _view.ShowMessage("Failed to update: " + ex.Message);
                }
            
        }

        /// <summary>
        /// Retrieves a list of calendar items within a specified date range and optional category filtering.
        /// </summary>
        /// <param name="start">The start date of the range.</param>
        /// <param name="end">The end date of the range.</param>
        /// <param name="filter">Indicates whether to apply category filtering.</param>
        /// <param name="categoryId">The category identifier to filter by, if filtering is enabled.</param>
        /// <returns>A list of calendar items that fall within the specified range and, if specified, the category.</returns>
        /// <example>
        /// Here is how you can use this method:
        /// <code>
        /// DateTime startDate = new DateTime(2024, 1, 1);
        /// DateTime endDate = new DateTime(2024, 1, 31);
        /// bool applyFilter = true;
        /// int categoryId = 5; // Assuming '5' is an existing category ID
        /// var items = presenter.DisplayCalendarItems(startDate, endDate, applyFilter, categoryId);
        /// </code>
        /// </example>
        public List<CalendarItem> DisplayCalendarItems(DateTime start, DateTime end, bool filter, int categoryId)
        {
            return _calendar.GetCalendarItems(start, end, filter, categoryId);
        }

        /// <summary>
        /// Retrieves a list of calendar items by month within a specified date range and optional category filtering.
        /// </summary>
        /// <param name="start">The start date of the range.</param>
        /// <param name="end">The end date of the range.</param>
        /// <param name="filter">Indicates whether to apply category filtering.</param>
        /// <param name="categoryId">The category identifier to filter by, if filtering is enabled.</param>
        /// <returns>A list of calendar items grouped by month that meet the specified criteria.</returns>
        /// <example>
        /// Here is how you can use this method:
        /// <code>
        /// DateTime start = new DateTime(2024, 1, 1);
        /// DateTime end = new DateTime(2024, 12, 31);
        /// bool filterByCategory = false;
        /// int categoryId = 0; // Category ID is irrelevant here since filtering is not applied
        /// var monthlyItems = presenter.DisplayItemsByMonth(start, end, filterByCategory, categoryId);
        /// </code>
        /// </example>
        public List<CalendarItemsByMonth> DisplayItemsByMonth(DateTime start, DateTime end, bool filter, int categoryId)
        {
            return _calendar.GetCalendarItemsByMonth(start, end, filter, categoryId);
        }

        /// <summary>
        /// Retrieves a list of calendar items by category within a specified date range and optional category filtering.
        /// </summary>
        /// <param name="start">The start date of the range.</param>
        /// <param name="end">The end date of the range.</param>
        /// <param name="filter">Indicates whether to apply category filtering.</param>
        /// <param name="categoryId">The category identifier to filter by, if filtering is enabled.</param>
        /// <returns>A list of calendar items grouped by category that meet the specified criteria.</returns>
        /// <example>
        /// Here is how you can use this method:
        /// <code>
        /// DateTime start = new DateTime(2024, 5, 1);
        /// DateTime end = new DateTime(2024, 5, 31);
        /// bool filterByCategory = true;
        /// int categoryId = 2; // Assuming '2' is a valid category for filtering
        /// var categoryItems = presenter.DisplayItemsByCategory(start, end, filterByCategory, categoryId);
        /// </code>
        /// </example>
        public List<CalendarItemsByCategory> DisplayItemsByCategory(DateTime start, DateTime end, bool filter, int categoryId)
        {
            return _calendar.GetCalendarItemsByCategory(start, end, filter, categoryId);
        }

        /// <summary>
        /// Retrieves a list of calendar items by category and month within a specified date range and optional category filtering.
        /// </summary>
        /// <param name="start">The start date of the range.</param>
        /// <param name="end">The end date of the range.</param>
        /// <param name="filter">Indicates whether to apply category filtering.</param>
        /// <param name="categoryId">The category identifier to filter by, if filtering is enabled.</param>
        /// <returns>A dictionary representing calendar items categorized both by month and category based on the specified criteria.</returns>
        /// <example>
        /// Here is how you can use this method:
        /// <code>
        /// DateTime start = new DateTime(2024, 5, 1);
        /// DateTime end = new DateTime(2024, 5, 31);
        /// bool filterByCategory = true;
        /// int categoryId = 3; // Assuming '3' is a valid category for filtering
        /// var categorizedItems = presenter.DisplayItemsByCategoryAndMonth(start, end, filterByCategory, categoryId);
        /// </code>
        /// </example>
        public List<Dictionary<string, object>> DisplayItemsByCategoryAndMonth(DateTime start, DateTime end, bool filter, int categoryId)
        {
            return _calendar.GetCalendarDictionaryByCategoryAndMonth(start, end, filter, categoryId);
        }



        
    }
}

