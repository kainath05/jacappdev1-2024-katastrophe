using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================


namespace Calendar
{
    // ====================================================================
    // CLASS: HomeCalendar
    //        - Combines a Categories Class and an Events Class
    //        - One File defines Category and Events File
    //        - etc
    // ====================================================================

    /// <summary>
    /// Class adds events and categories to our home calendar
    /// For more information, see <see cref="Categories"/> and <see cref="Events"/>
    /// </summary>

    public class HomeCalendar
    {
        private string? _FileName;
        private string? _DirName;
        private Categories _categories;
        private Events _events;

        // ====================================================================
        // Properties
        // ===================================================================

        // Properties (location of files etc)

        /// <summary>
        /// Gets the filename of our calendar
        /// </summary>
        /// <value>
        /// Name of the file
        /// </value>
        public String? FileName { get { return _FileName; } }

        /// <summary>
        /// Gets the directoory name of where our calndar file is stored
        /// </summary>
        /// <value>
        /// Directory name of the file
        /// </value>
        public String? DirName { get { return _DirName; } }

        /// <summary>
        /// Path to find the calendar file
        /// </summary>
        /// <value>
        /// Path of the file
        /// </value>
        public String? PathName
        {
            get
            {
                if (_FileName != null && _DirName != null)
                {
                    return Path.GetFullPath(_DirName + "\\" + _FileName);
                }
                else
                {
                    return null;
                }
            }
        }

        // Properties (categories and events object)

        /// <summary>
        /// Gets the categories from categories class
        /// </summary>
        /// <value>
        /// Categorization of events
        /// </value>
        public Categories categories { get { return _categories; } }

        /// <summary>
        /// Gets the events from events class
        /// </summary>
        /// <value>
        /// Time frame used to commence an activity
        /// </value>
        public Events events { get { return _events; } }


        public HomeCalendar(String databaseFile, String eventsXMLFile, bool newDB = false)
        {
            // if database exists, and user doesn't want a new database, open existing DB
            if (!newDB && File.Exists(databaseFile))
            {
                Database.existingDatabase(databaseFile);
            }

            // file did not exist, or user wants a new database, so open NEW DB
            else
            {
                Database.newDatabase(databaseFile);
                newDB = true;
            }

            // create the category object
            _categories = new Categories(Database.dbConnection, newDB);

            // create the _events course
            _events = new Events();
            _events.ReadFromFile(eventsXMLFile);
        }


        #region OpenNewAndSave
        // ---------------------------------------------------------------
        // Read
        // Throws Exception if any problem reading this file
        // ---------------------------------------------------------------

        /// <summary>
        /// Reads the calendar file
        /// </summary>
        /// <param name="calendarFileName">The name of the calendar file to be read</param>
        /// <exception cref="Exception">If unable to read calendar info</exception>
        public void ReadFromFile(String? calendarFileName)
        {
            // ---------------------------------------------------------------
            // read the calendar file and process
            // ---------------------------------------------------------------
            try
            {
                // get filepath name (throws exception if it doesn't exist)
                calendarFileName = CalendarFiles.VerifyReadFromFileName(calendarFileName, "");

                // If file exists, read it
                string[] filenames = System.IO.File.ReadAllLines(calendarFileName);

                // ----------------------------------------------------------------
                // Save information about the calendar file
                // ----------------------------------------------------------------
                string? folder = Path.GetDirectoryName(calendarFileName);
                _FileName = Path.GetFileName(calendarFileName);

                // read the events and categories from their respective files
                _categories.ReadFromFile(folder + "\\" + filenames[0]);
                _events.ReadFromFile(folder + "\\" + filenames[1]);

                // Save information about calendar file
                _DirName = Path.GetDirectoryName(calendarFileName);
                _FileName = Path.GetFileName(calendarFileName);

            }

            // ----------------------------------------------------------------
            // throw new exception if we cannot get the info that we need
            // ----------------------------------------------------------------
            catch (Exception e)
            {
                throw new Exception("Could not read calendar info: \n" + e.Message);
            }

        }

        // ====================================================================
        // save to a file
        // saves the following files:
        //  filepath_events.evts  # events file
        //  filepath_categories.cats # categories files
        //  filepath # a file containing the names of the events and categories files.
        //  Throws exception if we cannot write to that file (ex: invalid dir, wrong permissions)
        // ====================================================================

        /// <summary>
        /// Adds events and categories to our calendar file
        /// </summary>
        /// <param name="filepath">filepath of our calendar file</param>
        public void SaveToFile(String filepath)
        {

            // ---------------------------------------------------------------
            // just in case filepath doesn't exist, reset path info
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if we can't write to the file)
            // ---------------------------------------------------------------
            filepath = CalendarFiles.VerifyWriteToFileName(filepath, "");

            String? path = Path.GetDirectoryName(Path.GetFullPath(filepath));
            String file = Path.GetFileNameWithoutExtension(filepath);
            String ext = Path.GetExtension(filepath);

            // ---------------------------------------------------------------
            // construct file names for events and categories
            // ---------------------------------------------------------------
            String eventpath = path + "\\" + file + "_events" + ".evts";
            String categorypath = path + "\\" + file + "_categories" + ".cats";

            // ---------------------------------------------------------------
            // save the events and categories into their own files
            // ---------------------------------------------------------------
            _events.SaveToFile(eventpath);
            _categories.SaveToFile(categorypath);

            // ---------------------------------------------------------------
            // save filenames of events and categories to calendar file
            // ---------------------------------------------------------------
            string[] files = { Path.GetFileName(categorypath), Path.GetFileName(eventpath) };
            System.IO.File.WriteAllLines(filepath, files);

            // ----------------------------------------------------------------
            // save filename info for later use
            // ----------------------------------------------------------------
            _DirName = path;
            _FileName = Path.GetFileName(filepath);
        }
        #endregion OpenNewAndSave

        #region GetList



        // ============================================================================
        // Get all events list
        // ============================================================================

        /// <summary>
        /// Adding a list calendar items in order from starting time
        /// </summary>
        /// <param name="Start">start time of first calendar item</param>
        /// <param name="End">End time for last category item</param>
        /// <param name="FilterFlag">Boolean that returns true if we have a specific category and false if there isn't a specific category</param>
        /// <param name="CategoryID">The specific category id that we use if FilterFlag == true</param>
        /// <returns>A list of calendar items</returns>
        public List<CalendarItem> GetCalendarItems(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // ------------------------------------------------------------------------
            // return joined list within time frame
            // ------------------------------------------------------------------------
            Start = Start ?? new DateTime(1900, 1, 1);
            End = End ?? new DateTime(2500, 1, 1);

            var query = from c in _categories.List()
                        join e in _events.List() on c.Id equals e.Category
                        where e.StartDateTime >= Start && e.StartDateTime <= End
                        select new { CatId = c.Id, EventId = e.Id, e.StartDateTime, Category = c.Description, e.Details, e.DurationInMinutes };

            // ------------------------------------------------------------------------
            // create a CalendarItem list with totals,
            // ------------------------------------------------------------------------
            List<CalendarItem> items = new List<CalendarItem>();
            Double totalBusyTime = 0;

            foreach (var e in query.OrderBy(q => q.StartDateTime))
            {
                // filter out unwanted categories if filter flag is on
                if (FilterFlag && CategoryID != e.CatId)
                {
                    continue;
                }

                // keep track of running totals
                totalBusyTime = totalBusyTime + e.DurationInMinutes;
                // Group all events month by month (sorted by year/month)
                items.Add(new CalendarItem
                {
                    CategoryID = e.CatId,
                    EventID = e.EventId,
                    ShortDescription = e.Details,
                    StartDateTime = e.StartDateTime,
                    DurationInMinutes = e.DurationInMinutes,
                    Category = e.Category,
                    BusyTime = totalBusyTime
                });
            }

            return items;
        }
        /// <example>
        //
        /// <b>Getting a list of ALL calendar items</b>
        /// 
        /// <code>
        /// <![CDATA[
        ///  HomeCalendar calendar = new HomeCalendar();
        ///  calendar.ReadFromFile(filename);
        /// 
        ///   List <CalendarItem> calendarItems = calendar.GetCalendarItems(null, null, false, 0);
        ///             
        ///   // print important information
        ///   foreach (var ci in calendarItems)
        ///   {
        ///     Console.WriteLine(
        ///        String.Format("{0} {1,-20}  {2,8} {3,12}",
        ///            ci.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
        ///            ci.ShortDescription,
        ///            ci.DurationInMinutes, ci.BusyTime)
        ///      );
        ///   }
        ///
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Date               Short Description         Duration     BusyTime
        /// 2018-Jan.-10-10-00 App Dev Homework            40           40
        /// 2018-Jan.-11-10-15 Sprint retrospective        60          100
        /// 2018-Jan.-11-19-30 staff meeting               15          115
        /// 2020-Jan.-01-00-00 New Year's                1440         1555
        /// 2020-Jan.-09-00-00 Honolulu                  1440         2995
        /// 2020-Jan.-10-00-00 Honolulu                  1440         4435
        /// 2020-Jan.-12-00-00 Wendy's birthday          1440         5875
        /// 2020-Jan.-20-11-00 On call security           180         6055
        /// </code>
        /// 
        /// </example>

        /// <example>
        //
        /// <b>Getting a list of ALL calendar items with start and end date</b>
        /// 
        /// <code>
        /// <![CDATA[
        ///  HomeCalendar calendar = new HomeCalendar();
        ///  calendar.ReadFromFile(filename);
        /// 
        ///   List <CalendarItem> calendarItems = calendar.GetCalendarItems(DateTime.Now.AddYears(-5), DateTime.Now, false, 0);
        ///             
        ///   // print important information
        ///   foreach (var ci in calendarItems)
        ///   {
        ///     Console.WriteLine(
        ///        String.Format("{0} {1,-20}  {2,8} {3,12}",
        ///            ci.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
        ///            ci.ShortDescription,
        ///            ci.DurationInMinutes, ci.BusyTime)
        ///      );
        ///   }
        ///
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Date               Short Description         Duration     BusyTime
        /// 2020-Jan.-01-00-00 New Year's                1440         1440
        /// 2020-Jan.-09-00-00 Honolulu                  1440         2880
        /// 2020-Jan.-10-00-00 Honolulu                  1440         4320
        /// 2020-Jan.-12-00-00 Wendy's birthday          1440         5760
        /// 2020-Jan.-20-11-00 On call security           180         5940
        /// </code>
        /// 
        /// </example>
        /// 
        /// <example>
        //
        /// <b>Getting a list of ALL calendar items with all values</b>
        /// 
        /// <code>
        /// <![CDATA[
        ///  HomeCalendar calendar = new HomeCalendar();
        ///  calendar.ReadFromFile(filename);
        /// 
        ///   List <CalendarItem> calendarItems = calendar.GetCalendarItems(DateTime.Now.AddYears(-5), DateTime.Now, true, 8);
        ///             
        ///   // print important information
        ///   foreach (var ci in calendarItems)
        ///   {
        ///     Console.WriteLine(
        ///        String.Format("{0} {1,-20}  {2,8} {3,12}",
        ///            ci.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
        ///            ci.ShortDescription,
        ///            ci.DurationInMinutes, ci.BusyTime)
        ///      );
        ///   }
        ///
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Date Short Description Duration     BusyTime
        /// 2020-Jan.-01-00-00 New Year's                1440         1440
        /// </code>
        /// 
        /// </example>

        // ============================================================================
        // returns a list of CalendarItemsByMonth which is 
        // "year/month", list of calendar items, and totalBusyTime for that month
        // ============================================================================

        /// <summary>
        /// Adding a list of calendar items grouped by month
        /// </summary>
        /// <param name="Start">start of specific calendar item</param>
        /// <param name="End">End of specific calendar item</param>
        /// <param name="FilterFlag">Boolean that filters by category if true</param>
        /// <param name="CategoryID">The specific category id that we use if FilterFlag == true</param>
        /// <returns>A list of calendar items grouped by month</returns>
        public List<CalendarItemsByMonth> GetCalendarItemsByMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items first
            // -----------------------------------------------------------------------
            List<CalendarItem> items = GetCalendarItems(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // Group by year/month
            // -----------------------------------------------------------------------
            var GroupedByMonth = items.GroupBy(c => c.StartDateTime.Year.ToString("D4") + "/" + c.StartDateTime.Month.ToString("D2"));

            // -----------------------------------------------------------------------
            // create new list
            // -----------------------------------------------------------------------
            var summary = new List<CalendarItemsByMonth>();
            foreach (var MonthGroup in GroupedByMonth)
            {
                // calculate totalBusyTime for this month, and create list of items
                double total = 0;
                var itemsList = new List<CalendarItem>();
                foreach (var item in MonthGroup)
                {
                    total = total + item.DurationInMinutes;
                    itemsList.Add(item);
                }

                // Add new CalendarItemsByMonth to our list
                summary.Add(new CalendarItemsByMonth
                {
                    Month = MonthGroup.Key,
                    Items = itemsList,
                    TotalBusyTime = total
                });
            }

            return summary;
        }

        /// <example>
        //
        /// <b>Getting a list of ALL calendar items by month</b>
        /// 
        /// <code>
        /// <![CDATA[
        ///  
        ///List<CalendarItemsByMonth> calendarItemsByMonths = calendar.GetCalendarItemsByMonth(null, null, false, 0);
        ///
        ///Console.WriteLine("Date               Short Description         Duration     BusyTime");
        ///    foreach (CalendarItemsByMonth calendarItem in calendarItemsByMonths)
        ///    {
        ///        Console.WriteLine("Month:" + calendarItem.Month);
        ///
        ///        foreach (CalendarItem item in calendarItem.Items)
        ///        {
        ///            Console.WriteLine(
        ///                           String.Format("{0} {1,-20}  {2,8} {3,12}",
        ///                               item.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
        ///                               item.ShortDescription,
        ///                               item.DurationInMinutes, item.BusyTime)
        ///                         );
        ///        }
        ///            }
        ///
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Date               Short Description         Duration     BusyTime
        /// Month:2018/01
        /// 2018-Jan.-10-10-00 App Dev Homework            40           40
        /// 2018-Jan.-11-10-15 Sprint retrospective        60          100
        /// 2018-Jan.-11-19-30 staff meeting               15          115
        /// Month:2020/01
        /// 2020-Jan.-01-00-00 New Year's                1440         1555
        /// 2020-Jan.-09-00-00 Honolulu                  1440         2995
        /// 2020-Jan.-10-00-00 Honolulu                  1440         4435
        /// 2020-Jan.-12-00-00 Wendy's birthday          1440         5875
        /// 2020-Jan.-20-11-00 On call security           180         6055
        /// </code>
        /// 
        /// </example>

        /// <example>
        //
        /// <b>Getting a list of ALL calendar items by month with start and end date</b>
        /// 
        /// <code>
        /// <![CDATA[
        ///  
        ///List<CalendarItemsByMonth> calendarItemsByMonths = calendar.GetCalendarItemsByMonth(DateTime.Now.AddYears(-5), DateTime.Now, false, 0);
        ///
        ///Console.WriteLine("Date               Short Description         Duration     BusyTime");
        ///    foreach (CalendarItemsByMonth calendarItem in calendarItemsByMonths)
        ///    {
        ///        Console.WriteLine("Month:" + calendarItem.Month);
        ///
        ///        foreach (CalendarItem item in calendarItem.Items)
        ///        {
        ///            Console.WriteLine(
        ///                           String.Format("{0} {1,-20}  {2,8} {3,12}",
        ///                               item.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
        ///                               item.ShortDescription,
        ///                               item.DurationInMinutes, item.BusyTime)
        ///                         );
        ///        }
        ///            }
        ///
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Date Short Description Duration     BusyTime
        /// Month:2020/01
        /// 2020-Jan.-01-00-00 New Year's                1440         1440
        /// 2020-Jan.-09-00-00 Honolulu                  1440         2880
        /// 2020-Jan.-10-00-00 Honolulu                  1440         4320
        /// 2020-Jan.-12-00-00 Wendy's birthday          1440         5760
        /// 2020-Jan.-20-11-00 On call security           180         5940
        /// </code>
        /// 
        /// </example>       /// <example>
        //
        /// <b>Getting a list of ALL calendar items by month with all values</b>
        /// 
        /// <code>
        /// <![CDATA[
        ///  
        ///List<CalendarItemsByMonth> calendarItemsByMonths = calendar.GetCalendarItemsByMonth(DateTime.Now.AddYears(-5), DateTime.Now, true, 8);
        ///
        ///Console.WriteLine("Date               Short Description         Duration     BusyTime");
        ///    foreach (CalendarItemsByMonth calendarItem in calendarItemsByMonths)
        ///    {
        ///        Console.WriteLine("Month:" + calendarItem.Month);
        ///
        ///        foreach (CalendarItem item in calendarItem.Items)
        ///        {
        ///            Console.WriteLine(
        ///                           String.Format("{0} {1,-20}  {2,8} {3,12}",
        ///                               item.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
        ///                               item.ShortDescription,
        ///                               item.DurationInMinutes, item.BusyTime)
        ///                         );
        ///        }
        ///            }
        ///
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Date               Short Description         Duration     BusyTime
        /// Month:2020/01
        /// 2020-Jan.-01-00-00 New Year's                1440         1440
        /// </code>
        /// 
        /// </example>

        // ============================================================================
        // Group all events by category (ordered by category name)
        // ============================================================================

        /// <summary>
        /// Adds a list of calendar items sorted by category
        /// </summary>
        /// <param name="Start">Start time of specific calendar item</param>
        /// <param name="End">End time of specific calendar item</param>
        /// <param name="FilterFlag">Boolean that filters by category if true</param>
        /// <param name="CategoryID">The specific category id that we use if FilterFlag == true</param>
        /// <returns>A list of calendar items sorted by category</returns>
        public List<CalendarItemsByCategory> GetCalendarItemsByCategory(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items first
            // -----------------------------------------------------------------------
            List<CalendarItem> filteredItems = GetCalendarItems(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // Group by Category
            // -----------------------------------------------------------------------
            var GroupedByCategory = filteredItems.GroupBy(c => c.Category);

            // -----------------------------------------------------------------------
            // create new list
            // -----------------------------------------------------------------------
            var summary = new List<CalendarItemsByCategory>();
            foreach (var CategoryGroup in GroupedByCategory.OrderBy(g => g.Key))
            {
                // calculate totalBusyTime for this category, and create list of items
                double total = 0;
                var items = new List<CalendarItem>();
                foreach (var item in CategoryGroup)
                {
                    total = total + item.DurationInMinutes;
                    items.Add(item);
                }

                // Add new CalendarItemsByCategory to our list
                summary.Add(new CalendarItemsByCategory
                {
                    Category = CategoryGroup.Key,
                    Items = items,
                    TotalBusyTime = total
                });
            }

            return summary;
        }

        /// <example>
        //
        /// <b>Getting a list of ALL calendar items by category</b>
        /// 
        /// <code>
        /// <![CDATA[
        ///  List<CalendarItemsByCategory> calendarItemsByCategories = calendar.GetCalendarItemsByCategory(null, null, false, 0);
        ///  Console.WriteLine("Date               Short Description         Duration     BusyTime");
        ///
        ///  foreach (CalendarItemsByCategory calendarItem in calendarItemsByCategories)
        ///  {
        ///
        ///      foreach (CalendarItem item in calendarItem.Items)
        ///      {
        ///            Console.WriteLine(
        ///                           String.Format("{0} {1,-20}  {2,8} {3,12}",
        ///                               item.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
        ///                               item.ShortDescription,
        ///                               item.DurationInMinutes, item.BusyTime)
        ///                       );
        ///      }
        ///      Console.WriteLine($"Category: {calendarItem.Category}");
        ///  }
        ///
        /// 
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Date               Short Description         Duration     BusyTime
        /// 2020-Jan.-12-00-00 Wendy's birthday          1440         5875
        /// Category: Birthdays
        /// 2020-Jan.-01-00-00 New Year's                1440         1555
        /// Category: Canadian Holidays
        /// 2018-Jan.-10-10-00 App Dev Homework            40           40
        /// Category: Fun
        /// 2020-Jan.-20-11-00 On call security           180         6055
        /// Category: On call
        /// 2020-Jan.-09-00-00 Honolulu                  1440         2995
        /// 2020-Jan.-10-00-00 Honolulu                  1440         4435
        /// Category: Vacation
        /// 2018-Jan.-11-10-15 Sprint retrospective        60          100
        /// 2018-Jan.-11-19-30 staff meeting               15          115
        /// Category: Work
        /// </code>
        /// 
        /// </example>
        /// 
        /// <example>
        //
        /// <b>Getting a list of ALL calendar items by category with start and end date</b>
        /// 
        /// <code>
        /// <![CDATA[
        ///  List<CalendarItemsByCategory> calendarItemsByCategories = calendar.GetCalendarItemsByCategory(DateTime.Now.AddYears(-5), DateTime.Now, false, 0);
        ///  Console.WriteLine("Date               Short Description         Duration     BusyTime");
        ///
        ///  foreach (CalendarItemsByCategory calendarItem in calendarItemsByCategories)
        ///  {
        ///
        ///      foreach (CalendarItem item in calendarItem.Items)
        ///      {
        ///            Console.WriteLine(
        ///                           String.Format("{0} {1,-20}  {2,8} {3,12}",
        ///                               item.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
        ///                               item.ShortDescription,
        ///                               item.DurationInMinutes, item.BusyTime)
        ///                       );
        ///      }
        ///      Console.WriteLine($"Category: {calendarItem.Category}");
        ///  }
        ///
        /// 
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Date               Short Description         Duration     BusyTime
        /// 2020-Jan.-12-00-00 Wendy's birthday          1440         5760
        /// Category: Birthdays
        /// 2020-Jan.-01-00-00 New Year's                1440         1440
        /// Category: Canadian Holidays
        /// 2020-Jan.-20-11-00 On call security           180         5940
        /// Category: On call
        /// 2020-Jan.-09-00-00 Honolulu                  1440         2880
        /// 2020-Jan.-10-00-00 Honolulu                  1440         4320
        /// Category: Vacation
        /// </code>
        /// 
        /// </example>
        /// 
        /// <example>
        //
        /// <b>Getting a list of ALL calendar items by category with all values</b>
        /// 
        /// <code>
        /// <![CDATA[
        ///  List<CalendarItemsByCategory> calendarItemsByCategories = calendar.GetCalendarItemsByCategory(DateTime.Now.AddYears(-5), DateTime.Now, true, 8);
        ///  Console.WriteLine("Date               Short Description         Duration     BusyTime");
        ///
        ///  foreach (CalendarItemsByCategory calendarItem in calendarItemsByCategories)
        ///  {
        ///
        ///      foreach (CalendarItem item in calendarItem.Items)
        ///      {
        ///            Console.WriteLine(
        ///                           String.Format("{0} {1,-20}  {2,8} {3,12}",
        ///                               item.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
        ///                               item.ShortDescription,
        ///                               item.DurationInMinutes, item.BusyTime)
        ///                       );
        ///      }
        ///      Console.WriteLine($"Category: {calendarItem.Category}");
        ///  }
        ///
        /// 
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Date               Short Description         Duration     BusyTime
        /// 2020-Jan.-01-00-00 New Year's                1440         1440
        /// Category: Canadian Holidays
        /// </code>
        /// 
        /// </example>
        /// 



        // ============================================================================
        // Group all events by category and Month
        // creates a list of Dictionary objects with:
        //          one dictionary object per month,
        //          and one dictionary object for the category total busy times
        // 
        // Each per month dictionary object has the following key value pairs:
        //           "Month", <name of month>
        //           "TotalBusyTime", <the total durations for the month>
        //             for each category for which there is an event in the month:
        //             "items:category", a List<CalendarItem>
        //             "category", the total busy time for that category for this month
        // The one dictionary for the category total busy times has the following key value pairs:
        //             for each category for which there is an event in ANY month:
        //             "category", the total busy time for that category for all the months
        // ============================================================================

        /// <summary>
        /// Creates a list of dictionary objects that are grouped by month and by category
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <param name="FilterFlag"></param>
        /// <param name="CategoryID"></param>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetCalendarDictionaryByCategoryAndMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
        // -----------------------------------------------------------------------
        // get all items by month 
        // -----------------------------------------------------------------------
        List<CalendarItemsByMonth> GroupedByMonth = GetCalendarItemsByMonth(Start, End, FilterFlag, CategoryID);

        // -----------------------------------------------------------------------
        // loop over each month
        // -----------------------------------------------------------------------
        var summary = new List<Dictionary<string, object>>();
        var totalBusyTimePerCategory = new Dictionary<String, Double>();

        foreach (var MonthGroup in GroupedByMonth)
        {
            // create record object for this month
            Dictionary<string, object> record = new Dictionary<string, object>();
            record["Month"] = MonthGroup.Month;
            record["TotalBusyTime"] = MonthGroup.TotalBusyTime;

            // break up the month items into categories
            var GroupedByCategory = MonthGroup.Items.GroupBy(c => c.Category);

            // -----------------------------------------------------------------------
            // loop over each category
            // -----------------------------------------------------------------------
            foreach (var CategoryGroup in GroupedByCategory.OrderBy(g => g.Key))
            {

                // calculate totals for the cat/month, and create list of items
                double totalCategoryBusyTimeForThisMonth = 0;
                var details = new List<CalendarItem>();

                foreach (var item in CategoryGroup)
                {
                    totalCategoryBusyTimeForThisMonth = totalCategoryBusyTimeForThisMonth + item.DurationInMinutes;
                    details.Add(item);
                }

                // add new properties and values to our record object
                record["items:" + CategoryGroup.Key] = details;
                record[CategoryGroup.Key] = totalCategoryBusyTimeForThisMonth;

                // keep track of totals for each category
                if (totalBusyTimePerCategory.TryGetValue(CategoryGroup.Key, out Double currentTotalBusyTimeForCategory))
                {
                    totalBusyTimePerCategory[CategoryGroup.Key] = currentTotalBusyTimeForCategory + totalCategoryBusyTimeForThisMonth;
                }
                else
                {
                    totalBusyTimePerCategory[CategoryGroup.Key] = totalCategoryBusyTimeForThisMonth;
                }
            }

            // add record to collection
            summary.Add(record);
        }
        // ---------------------------------------------------------------------------
        // add final record which is the totals for each category
        // ---------------------------------------------------------------------------
        Dictionary<string, object> totalsRecord = new Dictionary<string, object>();
        totalsRecord["Month"] = "TOTALS";

        foreach (var cat in categories.List())
        {
            try
            {
                totalsRecord.Add(cat.Description, totalBusyTimePerCategory[cat.Description]);
            }
            catch { }
        }
        summary.Add(totalsRecord);


        return summary;
        }
        /// <example>
        //
        /// <b>Getting a calendar dictionary by month and category</b>
        /// 
        /// <code>
        /// <![CDATA[
        ///  Dictionary<string, Dictionary<string, List<CalendarItem>>> calendarDictionary = new Dictionary<string, Dictionary<string, List<CalendarItem>>>();
        ///
        ///List<CalendarItem> calendarItems = calendar.GetCalendarItems(null, null, false, 0);
        ///
        ///    foreach (CalendarItem item in calendarItems)
        ///    {
        ///        string monthKey = item.StartDateTime.ToString("MM/yyyy");
        ///string categoryKey = item.Category;
        ///
        ///        if (!calendarDictionary.ContainsKey(monthKey))
        ///        {
        ///            calendarDictionary[monthKey] = new Dictionary<string, List<CalendarItem>>();
        ///        }
        ///
        ///        if (!calendarDictionary[monthKey].ContainsKey(categoryKey))
        ///        {
        ///            calendarDictionary[monthKey][categoryKey] = new List<CalendarItem>();
        ///        }
        ///
        ///calendarDictionary[monthKey][categoryKey].Add(item);
        ///    }
        ///
        ///            foreach (var monthEntry in calendarDictionary)
        ///{
        ///    Console.WriteLine($"Month: {monthEntry.Key}");
        ///
        ///    foreach (var categoryEntry in monthEntry.Value)
        ///    {
        ///        Console.WriteLine($"  Category: {categoryEntry.Key}");
        ///
        ///        foreach (var calendarItem in categoryEntry.Value)
        ///        {
        ///            Console.WriteLine(
        ///                String.Format("{0} {1,-20}  {2,8} {3,12}",
        ///                    calendarItem.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
        ///                    calendarItem.ShortDescription,
        ///                    calendarItem.DurationInMinutes, calendarItem.BusyTime)
        ///            );
        ///        }
        ///    }
        ///}
        ///
        /// 
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Month: 01-2018
        /// Category: Fun
        /// 2018-Jan.-10-10-00 App Dev Homework            40           40
        /// Category: Work
        /// 2018-Jan.-11-10-15 Sprint retrospective        60          100
        /// 2018-Jan.-11-19-30 staff meeting               15          115
        /// Month: 01-2020
        /// Category: Canadian Holidays
        /// 2020-Jan.-01-00-00 New Year's                1440         1555
        /// Category: Vacation
        /// 2020-Jan.-09-00-00 Honolulu                  1440         2995
        /// 2020-Jan.-10-00-00 Honolulu                  1440         4435
        /// Category: Birthdays
        /// 2020-Jan.-12-00-00 Wendy's birthday          1440         5875
        /// Category: On call
        /// 2020-Jan.-20-11-00 On call security           180         6055
        /// </code>
        /// 
        /// </example>    

        /// <example>
        //
        /// <b>Getting a calendar dictionary by month and category with start and end dates</b>
        /// 
        /// <code>
        /// <![CDATA[
        ///  Dictionary<string, Dictionary<string, List<CalendarItem>>> calendarDictionary = new Dictionary<string, Dictionary<string, List<CalendarItem>>>();
        ///
        ///  List<CalendarItem> calendarItems = calendar.GetCalendarItems(DateTime.Now.AddYears(-5), DateTime.Now, true, 8);
        ///
        ///    foreach (CalendarItem item in calendarItems)
        ///    {
        ///        string monthKey = item.StartDateTime.ToString("MM/yyyy");
        ///        string categoryKey = item.Category;
        ///
        ///        if (!calendarDictionary.ContainsKey(monthKey))
        ///        {
        ///            calendarDictionary[monthKey] = new Dictionary<string, List<CalendarItem>>();
        ///        }
        ///
        ///        if (!calendarDictionary[monthKey].ContainsKey(categoryKey))
        ///        {
        ///            calendarDictionary[monthKey][categoryKey] = new List<CalendarItem>();
        ///        }
        ///
        ///        calendarDictionary[monthKey][categoryKey].Add(item);
        ///   }
        ///
        ///   foreach (var monthEntry in calendarDictionary)
        ///   {
        ///    Console.WriteLine($"Month: {monthEntry.Key}");
        ///
        ///    foreach (var categoryEntry in monthEntry.Value)
        ///    {
        ///        Console.WriteLine($"  Category: {categoryEntry.Key}");
        ///
        ///        foreach (var calendarItem in categoryEntry.Value)
        ///        {
        ///            Console.WriteLine(
        ///                String.Format("{0} {1,-20}  {2,8} {3,12}",
        ///                    calendarItem.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
        ///                    calendarItem.ShortDescription,
        ///                    calendarItem.DurationInMinutes, calendarItem.BusyTime)
        ///            );
        ///        }
        ///    }
        ///}
        ///
        /// 
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Month: 01-2020
        /// Category: Canadian Holidays
        /// 2020-Jan.-01-00-00 New Year's                1440         1440
        /// Category: Vacation
        /// 2020-Jan.-09-00-00 Honolulu                  1440         2880
        /// 2020-Jan.-10-00-00 Honolulu                  1440         4320
        /// Category: Birthdays
        /// 2020-Jan.-12-00-00 Wendy's birthday          1440         5760
        /// Category: On call
        /// 2020-Jan.-20-11-00 On call security           180         5940
        /// </code>
        /// 
        /// </example> 
        /// 
        /// <example>
        //
        /// <b>Getting a calendar dictionary by month and category with all values</b>
        /// 
        /// <code>
        /// <![CDATA[
        ///  Dictionary<string, Dictionary<string, List<CalendarItem>>> calendarDictionary = new Dictionary<string, Dictionary<string, List<CalendarItem>>>();
        ///
        ///  List<CalendarItem> calendarItems = calendar.GetCalendarItems(DateTime.Now.AddYears(-5), DateTime.Now, true, 8);
        ///
        ///    foreach (CalendarItem item in calendarItems)
        ///    {
        ///        string monthKey = item.StartDateTime.ToString("MM/yyyy");
        ///string categoryKey = item.Category;
        ///
        ///        if (!calendarDictionary.ContainsKey(monthKey))
        ///        {
        ///            calendarDictionary[monthKey] = new Dictionary<string, List<CalendarItem>>();
        ///        }
        ///
        ///        if (!calendarDictionary[monthKey].ContainsKey(categoryKey))
        ///        {
        ///            calendarDictionary[monthKey][categoryKey] = new List<CalendarItem>();
        ///        }
        ///
        ///calendarDictionary[monthKey][categoryKey].Add(item);
        ///    }
        ///
        ///            foreach (var monthEntry in calendarDictionary)
        ///{
        ///    Console.WriteLine($"Month: {monthEntry.Key}");
        ///
        ///    foreach (var categoryEntry in monthEntry.Value)
        ///    {
        ///        Console.WriteLine($"  Category: {categoryEntry.Key}");
        ///
        ///        foreach (var calendarItem in categoryEntry.Value)
        ///        {
        ///            Console.WriteLine(
        ///                String.Format("{0} {1,-20}  {2,8} {3,12}",
        ///                    calendarItem.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
        ///                    calendarItem.ShortDescription,
        ///                    calendarItem.DurationInMinutes, calendarItem.BusyTime)
        ///            );
        ///        }
        ///    }
        ///}
        ///
        /// 
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Month: 01-2020
        /// Category: Canadian Holidays
        /// 2020-Jan.-01-00-00 New Year's                1440         1440
        /// </code>
        /// 
        /// </example> 

        #endregion GetList

    }
}
