﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.SQLite;
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
        private Categories _categories;
        private Events _events;

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

        /// <summary>
        /// Initializes a home calendar using a database. If a database does not exist, create a new one.
        /// </summary>
        /// <param name="databaseFile">The database to use file</param>
        /// <param name="newDB">a bool if we use a new database or not</param>
        /// <example>
        /// Creating a HomeCalendar instance with existing database
        /// <code>
        /// <![CDATA[
        /// HomeCalendar calendar1 = new HomeCalendar("existing_database.db");
        /// ]]>
        /// </code>
        /// Creating a HomeCalendar instance with a new database
        /// <code>
        /// <![CDATA[
        /// HomeCalendar calendar2 = new HomeCalendar("new_database.db", true);
        /// ]]>
        /// </code>
        /// </example>
        public HomeCalendar(String databaseFile, bool newDB = false)
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
            _events = new Events(Database.dbConnection, newDB);
        }

        #region GetList

        //// ============================================================================
        //// Get all events list
        //// ============================================================================

        /// <summary>
        /// Retrieves a list of calendar items within a specified time frame, optionally filtered by a specific category.
        /// </summary>
        /// <param name="Start">The start time from which to begin retrieving calendar items. If null, a default of January 1, 1900, is used.</param>
        /// <param name="End">The end time at which to stop retrieving calendar items. If null, a default of January 1, 2500, is used.</param>
        /// <param name="FilterFlag">Indicates whether the calendar items should be filtered by a specific category. True to enable filtering; false otherwise.</param>
        /// <param name="CategoryID">Specifies the category ID to filter by. This parameter is considered only if FilterFlag is true.</param>
        /// <returns>A list of CalendarItem objects, each representing a calendar event within the specified time frame and optionally filtered by category.</returns>
        /// <example>
        /// <b>Example 1: Retrieving all calendar items without any category filter</b>
        /// <code>
        /// HomeCalendar calendar = new HomeCalendar();
        /// calendar.ReadFromFile(filename);
        /// List<CalendarItem> calendarItems = calendar.GetCalendarItems(null, null, false, 0);
        /// 
        /// foreach (var item in calendarItems)
        /// {
        ///     Console.WriteLine($"{item.StartDateTime:yyyy/MMM/dd/HH/mm} {item.ShortDescription,-20}  {item.DurationInMinutes,8} {item.BusyTime,12}");
        /// }
        /// </code>
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
        /// </example>
        /// <example>
        /// <b>Example 2: Retrieving calendar items within a specified time frame</b>
        /// <code>
        /// List<CalendarItem> calendarItems = calendar.GetCalendarItems(DateTime.Now.AddYears(-5), DateTime.Now, false, 0);
        /// 
        /// foreach (var item in calendarItems)
        /// {
        ///     Console.WriteLine($"{item.StartDateTime:yyyy/MMM/dd/HH/mm} {item.ShortDescription,-20}  {item.DurationInMinutes,8} {item.BusyTime,12}");
        /// }
        /// </code>
        /// Sample output:
        /// <code>
        /// Date               Short Description         Duration     BusyTime
        /// 2020-Jan.-01-00-00 New Year's                1440         1440
        /// 2020-Jan.-09-00-00 Honolulu                  1440         2880
        /// 2020-Jan.-10-00-00 Honolulu                  1440         4320
        /// 2020-Jan.-12-00-00 Wendy's birthday          1440         5760
        /// 2020-Jan.-20-11-00 On call security           180         5940
        /// </code>
        /// </example>
        /// <example>
        /// <b>Example 3: Filtering calendar items by category</b>
        /// <code>
        /// List<CalendarItem> calendarItems = calendar.GetCalendarItems(DateTime.Now.AddYears(-5), DateTime.Now, true, 8);
        /// 
        /// foreach (var item in calendarItems)
        /// {
        ///     Console.WriteLine($"{item.StartDateTime:yyyy/MMM/dd/HH/mm} {item.ShortDescription,-20}  {item.DurationInMinutes,8} {item.BusyTime,12}");
        /// }
        /// </code>
        /// Sample output:
        /// <code>
        /// Date               Short Description         Duration     BusyTime
        /// 2020-Jan-01-00-00 New Year's                1440         1440
        /// </code>
        /// </example>

        public List<CalendarItem> GetCalendarItems(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // ------------------------------------------------------------------------
            // return joined list within time frame
            // ------------------------------------------------------------------------
            Start = Start ?? new DateTime(1900, 1, 1);
            End = End ?? new DateTime(2500, 1, 1);

            string query = @"
        SELECT e.Id AS EventId, e.CategoryId, e.StartDateTime, e.DurationInMinutes, e.Details,
            c.Description AS CategoryDescription
        FROM events e
        JOIN categories c ON e.CategoryId = c.Id
        WHERE e.StartDateTime >= @Start AND e.StartDateTime <= @End";

            if (FilterFlag)
            {
                query += " AND e.CategoryId = @CategoryId";
            }

            query += " ORDER BY e.StartDateTime";

            // ------------------------------------------------------------------------
            // create a CalendarItem list with totals,
            // ------------------------------------------------------------------------
            List<CalendarItem> items = new List<CalendarItem>();
            Double totalBusyTime = 0;

            using (var cmd = new SQLiteCommand(query, Database.dbConnection))
            {
                // Add parameters to the command to avoid SQL injection
                cmd.Parameters.AddWithValue("@Start", Start.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@End", End.Value.ToString("yyyy-MM-dd"));
                if (FilterFlag)
                {
                    cmd.Parameters.AddWithValue("@CategoryId", CategoryID);
                }

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Read the data using the column names, ensuring type safety
                        var eventId = reader.GetInt32(reader.GetOrdinal("EventId"));
                        var categoryId = reader.GetInt32(reader.GetOrdinal("CategoryId"));
                        var startDateTimeString = reader.GetString(reader.GetOrdinal("StartDateTime"));
                        var startDateTime = DateTime.ParseExact(startDateTimeString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                        var duration = reader.GetDouble(reader.GetOrdinal("DurationInMinutes"));
                        var details = reader.GetString(reader.GetOrdinal("Details"));
                        var categoryDescription = reader.GetString(reader.GetOrdinal("CategoryDescription"));

                        // Accumulate the total busy time
                        //Category category = categories.GetCategoryFromId(eventId);
                        //if (category.Type != Category.CategoryType.Availability)
                        //{
                            totalBusyTime += duration;
                        //}

                        // Add a new CalendarItem object to the list
                        items.Add(new CalendarItem
                        {
                            EventID = eventId,
                            CategoryID = categoryId,
                            StartDateTime = startDateTime,
                            DurationInMinutes = duration,
                            ShortDescription = details,
                            Category = categoryDescription,
                            BusyTime = totalBusyTime
                        });

                    }
                }
            }

            return items;
        }



        //// ============================================================================
        //// returns a list of CalendarItemsByMonth which is 
        //// "year/month", list of calendar items, and totalBusyTime for that month
        //// ============================================================================


        /// <summary>
        /// Adding a list of calendar items grouped by month
        /// </summary>
        /// <param name="Start">start of specific calendar item</param>
        /// <param name="End">End of specific calendar item</param>
        /// <param name="FilterFlag">Boolean that filters by category if true</param>
        /// <param name="CategoryID">The specific category id that we use if FilterFlag == true</param>
        /// <returns>A list of calendar items grouped by month</returns>
        /// <example>
        /// Example of retrieving all calendar items by month without any category filter:
        /// <code>
        /// List<CalendarItemsByMonth> calendarItemsByMonths = calendar.GetCalendarItemsByMonth(null, null, false, 0);
        /// Console.WriteLine("Date               Short Description         Duration     BusyTime");
        /// foreach (CalendarItemsByMonth calendarItem in calendarItemsByMonths)
        /// {
        ///     Console.WriteLine("Month:" + calendarItem.Month);
        ///     foreach (CalendarItem item in calendarItem.Items)
        ///     {
        ///         Console.WriteLine(
        ///             $"{item.StartDateTime:yyyy/MMM/dd/HH/mm} {item.ShortDescription,-20}  {item.DurationInMinutes,8} {item.BusyTime,12}");
        ///     }
        /// }
        /// </code>
        /// Sample output:
        /// <code>
        /// Date               Short Description         Duration     BusyTime
        /// Month:2018/01
        /// 2018-Jan-10-10-00 App Dev Homework            40           40
        /// 2018-Jan-11-10-15 Sprint retrospective        60          100
        /// 2018-Jan-11-19-30 Staff meeting               15          115
        /// Month:2020/01
        /// 2020-Jan-01-00-00 New Year's                1440         1555
        /// 2020-Jan-09-00-00 Honolulu                  1440         2995
        /// 2020-Jan-10-00-00 Honolulu                  1440         4435
        /// 2020-Jan-12-00-00 Wendy's birthday          1440         5875
        /// 2020-Jan-20-11-00 On call security           180         6055
        /// </code>
        /// </example>

        public List<CalendarItemsByMonth> GetCalendarItemsByMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID) //need group by for query, distinct looks for pairs?
        {
            var summary = new List<CalendarItemsByMonth>();

            Start = Start ?? new DateTime(1900, 1, 1);
            End = End ?? new DateTime(2500, 12, 31);

            // Prepare the SQL query to select distinct year and month combinations from the events table.
            // The query filters events between the provided start and end dates.
            // If the FilterFlag is true, it further filters events by the specified CategoryID.
            // STRFTIME extracts the year and month from StartDateTime respectively.
            // Lastly, filterFlag is used in a ternary operator to decide whether or not to use it.
            string groupQuery = $@"
SELECT DISTINCT 
    strftime('%Y', StartDateTime) AS Year,
    strftime('%m', StartDateTime) AS Month
FROM events
WHERE StartDateTime BETWEEN @Start AND @End
" + (FilterFlag ? "AND CategoryId = @CategoryId" : "") + @" 
ORDER BY Year, Month;";

            using (var cmd = new SQLiteCommand(groupQuery, Database.dbConnection))
            {
                // Add parameters to the query to protect against SQL injection.
                cmd.Parameters.AddWithValue("@Start", Start.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@End", End.Value.ToString("yyyy-MM-dd"));
                if (FilterFlag) cmd.Parameters.AddWithValue("@CategoryId", CategoryID);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Parse the year and month from the query result.
                        var year = reader.GetString(reader.GetOrdinal("Year"));
                        var month = reader.GetString(reader.GetOrdinal("Month"));

                        // Calculate the start and end DateTime objects for the month.
                        var startOfMonth = new DateTime(int.Parse(year), int.Parse(month), 1); // handle day of month and unit test made
                        var endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1);

                        List<CalendarItem> items = GetCalendarItems(startOfMonth, endOfMonth, FilterFlag, CategoryID);

                        double totalBusyTime = items.Sum(item => item.DurationInMinutes);

                        // Add a new CalendarItemsByMonth object to the summary list for this month,
                        // including the list of items and the total busy time.
                        summary.Add(new CalendarItemsByMonth
                        {
                            Month = $"{year}/{month}",
                            Items = items,
                            TotalBusyTime = totalBusyTime
                        });
                    }
                }
            }

            return summary;
        }

        /// <summary>
        /// Adds a list of calendar items sorted by category
        /// </summary>
        /// <param name="Start">Start time of specific calendar item</param>
        /// <param name="End">End time of specific calendar item</param>
        /// <param name="FilterFlag">Boolean that filters by category if true</param>
        /// <param name="CategoryID">A list of calendar items sorted by category</param>
        /// <returns>A list of category items by category</returns>
        /// <example>
        ///
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
        ///
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
        ///
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
        public List<CalendarItemsByCategory> GetCalendarItemsByCategory(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            Start = Start ?? new DateTime(1900, 1, 1);
            End = End ?? new DateTime(2500, 1, 1);
            List<CalendarItemsByCategory> itemsByCategory = new List<CalendarItemsByCategory>();

            string queryCategory = @"
                SELECT e.CategoryId, c.Description 
                FROM events e 
                INNER JOIN categories c ON e.CategoryId = c.Id 
                WHERE e.StartDateTime >= @Start AND e.StartDateTime <= @End";

            if (FilterFlag)
            {
                queryCategory += " AND e.CategoryId = @CategoryId";
            }
            queryCategory += " GROUP BY c.Id ORDER BY c.Description";
            using (var cmd = new SQLiteCommand(queryCategory, Database.dbConnection))
            {
                cmd.Parameters.AddWithValue("@Start", Start.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@End", End.Value.ToString("yyyy-MM-dd"));

                if (FilterFlag)
                {
                    cmd.Parameters.AddWithValue("@CategoryId", CategoryID);
                }

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int categoryId = Convert.ToInt32(reader["CategoryId"]);
                        string desc = (string)reader["Description"];
                        List<CalendarItem> items = GetCalendarItems(Start, End, true, categoryId);
                        itemsByCategory.Add(new CalendarItemsByCategory { Category = desc, Items = items, TotalBusyTime = items.Count > 0 ? items[items.Count - 1].BusyTime : 0 });
                    }
                }
            }

            return itemsByCategory;
        }

        ///// <example>
        ////
        ///// <b>Getting a calendar dictionary by month and category</b>
        ///// 
        ///// <code>
        ///// <![CDATA[
        /////  Dictionary<string, Dictionary<string, List<CalendarItem>>> calendarDictionary = new Dictionary<string, Dictionary<string, List<CalendarItem>>>();
        /////
        /////List<CalendarItem> calendarItems = calendar.GetCalendarItems(null, null, false, 0);
        /////
        /////    foreach (CalendarItem item in calendarItems)
        /////    {
        /////        string monthKey = item.StartDateTime.ToString("MM/yyyy");
        /////string categoryKey = item.Category;
        /////
        /////        if (!calendarDictionary.ContainsKey(monthKey))
        /////        {
        /////            calendarDictionary[monthKey] = new Dictionary<string, List<CalendarItem>>();
        /////        }
        /////
        /////        if (!calendarDictionary[monthKey].ContainsKey(categoryKey))
        /////        {
        /////            calendarDictionary[monthKey][categoryKey] = new List<CalendarItem>();
        /////        }
        /////
        /////calendarDictionary[monthKey][categoryKey].Add(item);
        /////    }
        /////
        /////            foreach (var monthEntry in calendarDictionary)
        /////{
        /////    Console.WriteLine($"Month: {monthEntry.Key}");
        /////
        /////    foreach (var categoryEntry in monthEntry.Value)
        /////    {
        /////        Console.WriteLine($"  Category: {categoryEntry.Key}");
        /////
        /////        foreach (var calendarItem in categoryEntry.Value)
        /////        {
        /////            Console.WriteLine(
        /////                String.Format("{0} {1,-20}  {2,8} {3,12}",
        /////                    calendarItem.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
        /////                    calendarItem.ShortDescription,
        /////                    calendarItem.DurationInMinutes, calendarItem.BusyTime)
        /////            );
        /////        }
        /////    }
        /////}
        /////
        ///// 
        ///// ]]>
        ///// </code>
        ///// 
        ///// Sample output:
        ///// <code>
        ///// Month: 01-2018
        ///// Category: Fun
        ///// 2018-Jan.-10-10-00 App Dev Homework            40           40
        ///// Category: Work
        ///// 2018-Jan.-11-10-15 Sprint retrospective        60          100
        ///// 2018-Jan.-11-19-30 staff meeting               15          115
        ///// Month: 01-2020
        ///// Category: Canadian Holidays
        ///// 2020-Jan.-01-00-00 New Year's                1440         1555
        ///// Category: Vacation
        ///// 2020-Jan.-09-00-00 Honolulu                  1440         2995
        ///// 2020-Jan.-10-00-00 Honolulu                  1440         4435
        ///// Category: Birthdays
        ///// 2020-Jan.-12-00-00 Wendy's birthday          1440         5875
        ///// Category: On call
        ///// 2020-Jan.-20-11-00 On call security           180         6055
        ///// </code>
        ///// 
        ///// </example>    

        ///// <example>
        /////
        ///// <b>Getting a calendar dictionary by month and category with start and end dates</b>
        ///// 
        ///// <code>
        ///// <![CDATA[
        /////  Dictionary<string, Dictionary<string, List<CalendarItem>>> calendarDictionary = new Dictionary<string, Dictionary<string, List<CalendarItem>>>();
        /////
        /////  List<CalendarItem> calendarItems = calendar.GetCalendarItems(DateTime.Now.AddYears(-5), DateTime.Now, true, 8);
        /////
        /////    foreach (CalendarItem item in calendarItems)
        /////    {
        /////        string monthKey = item.StartDateTime.ToString("MM/yyyy");
        /////        string categoryKey = item.Category;
        /////
        /////        if (!calendarDictionary.ContainsKey(monthKey))
        /////        {
        /////            calendarDictionary[monthKey] = new Dictionary<string, List<CalendarItem>>();
        /////        }
        /////
        /////        if (!calendarDictionary[monthKey].ContainsKey(categoryKey))
        /////        {
        /////            calendarDictionary[monthKey][categoryKey] = new List<CalendarItem>();
        /////        }
        /////
        /////        calendarDictionary[monthKey][categoryKey].Add(item);
        /////   }
        /////
        /////   foreach (var monthEntry in calendarDictionary)
        /////   {
        /////    Console.WriteLine($"Month: {monthEntry.Key}");
        /////
        /////    foreach (var categoryEntry in monthEntry.Value)
        /////    {
        /////        Console.WriteLine($"  Category: {categoryEntry.Key}");
        /////
        /////        foreach (var calendarItem in categoryEntry.Value)
        /////        {
        /////            Console.WriteLine(
        /////                String.Format("{0} {1,-20}  {2,8} {3,12}",
        /////                    calendarItem.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
        /////                    calendarItem.ShortDescription,
        /////                    calendarItem.DurationInMinutes, calendarItem.BusyTime)
        /////            );
        /////        }
        /////    }
        /////}
        /////
        ///// 
        ///// ]]>
        ///// </code>
        ///// 
        ///// Sample output:
        ///// <code>
        ///// Month: 01-2020
        ///// Category: Canadian Holidays
        ///// 2020-Jan.-01-00-00 New Year's                1440         1440
        ///// Category: Vacation
        ///// 2020-Jan.-09-00-00 Honolulu                  1440         2880
        ///// 2020-Jan.-10-00-00 Honolulu                  1440         4320
        ///// Category: Birthdays
        ///// 2020-Jan.-12-00-00 Wendy's birthday          1440         5760
        ///// Category: On call
        ///// 2020-Jan.-20-11-00 On call security           180         5940
        ///// </code>
        ///// 
        ///// </example> 
        ///// 
        ///// <example>
        ////
        ///// <b>Getting a calendar dictionary by month and category with all values</b>
        ///// 
        ///// <code>
        ///// <![CDATA[
        /////  Dictionary<string, Dictionary<string, List<CalendarItem>>> calendarDictionary = new Dictionary<string, Dictionary<string, List<CalendarItem>>>();
        /////
        /////  List<CalendarItem> calendarItems = calendar.GetCalendarItems(DateTime.Now.AddYears(-5), DateTime.Now, true, 8);
        /////
        /////    foreach (CalendarItem item in calendarItems)
        /////    {
        /////        string monthKey = item.StartDateTime.ToString("MM/yyyy");
        /////string categoryKey = item.Category;
        /////
        /////        if (!calendarDictionary.ContainsKey(monthKey))
        /////        {
        /////            calendarDictionary[monthKey] = new Dictionary<string, List<CalendarItem>>();
        /////        }
        /////
        /////        if (!calendarDictionary[monthKey].ContainsKey(categoryKey))
        /////        {
        /////            calendarDictionary[monthKey][categoryKey] = new List<CalendarItem>();
        /////        }
        /////
        /////calendarDictionary[monthKey][categoryKey].Add(item);
        /////    }
        /////
        /////            foreach (var monthEntry in calendarDictionary)
        /////{
        /////    Console.WriteLine($"Month: {monthEntry.Key}");
        /////
        /////    foreach (var categoryEntry in monthEntry.Value)
        /////    {
        /////        Console.WriteLine($"  Category: {categoryEntry.Key}");
        /////
        /////        foreach (var calendarItem in categoryEntry.Value)
        /////        {
        /////            Console.WriteLine(
        /////                String.Format("{0} {1,-20}  {2,8} {3,12}",
        /////                    calendarItem.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
        /////                    calendarItem.ShortDescription,
        /////                    calendarItem.DurationInMinutes, calendarItem.BusyTime)
        /////            );
        /////        }
        /////    }
        /////}
        /////
        ///// 
        ///// ]]>
        ///// </code>
        ///// 
        ///// Sample output:
        ///// <code>
        ///// Month: 01-2020
        ///// Category: Canadian Holidays
        ///// 2020-Jan.-01-00-00 New Year's                1440         1440
        ///// </code>
        ///// 
        ///// </example> 
        /// <summary>
        /// 
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
#endregion GetList

    }
}
