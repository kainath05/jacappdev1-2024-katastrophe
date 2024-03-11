using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Calendar
{
    // ====================================================================
    // CLASS: CalendarItem
    //        A single calendar item, includes a Category and an Event
    // ====================================================================

    /// <summary>
    /// Class describes different attributes of a calendar item that a user can add
    /// </summary>
    public class CalendarItem
    {
        /// <summary>
        /// The categoryID of the calendar item to be added(11 categories)
        /// </summary>
        /// <value>
        /// To identify a category
        /// </value>
        public int CategoryID { get; set; }

        /// <summary>
        /// Adds an eventID to each calendarItem to get the number of calendarID and access specific ones
        /// </summary>
        /// <value>
        /// to identify an event
        /// </value>
        public int EventID { get; set; }

        /// <summary>
        /// Property for startTime of the calendarItem
        /// </summary>
        /// <value>
        /// The start time of the specific calendar item
        /// </value>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// Name of each category, associated with its categoryID
        /// </summary>
        /// <value>
        /// Group of events destinguished by the categoryId
        /// </value>
        public String? Category { get; set; }

        /// <summary>
        /// Short description of the calendar Item to be added
        /// </summary>
        /// <value>
        /// Details of the event in human readable format
        /// </value>
        public String? ShortDescription { get; set; }

        /// <summary>
        /// The length in minutes of the calendar item
        /// </summary>
        /// <value>
        /// Time represented in minutes
        /// </value>
        public Double DurationInMinutes { get; set; }

        /// <summary>
        /// Accumulation of time already used for other calendar items
        /// </summary>
        /// <value>
        /// Total amount of minutes used
        /// </value>
        public Double BusyTime { get; set; }

    }

    /// <summary>
    /// List of calendar items for the month
    /// </summary>
    public class CalendarItemsByMonth
    {

        /// <summary>
        /// The month in which we have a list of calendar items
        /// </summary>
        /// <value>
        /// The month of the year
        /// </value>
        public String? Month { get; set; }

        /// <summary>
        /// Property for the list of items in our calndar for the month
        /// </summary>
        /// <value>
        /// List of calendar items
        /// </value>
        public List<CalendarItem>? Items { get; set; }

        /// <summary>
        /// Total time occupied with calendar items in the month
        /// </summary>
        /// <value>
        /// Total amount of minutes used
        /// </value>
        public Double TotalBusyTime { get; set; }
    }

    /// <summary>
    /// List of items sorted by category
    /// </summary>
    public class CalendarItemsByCategory
    {

        /// <summary>
        /// the category name that we group the calendar items by
        /// </summary>
        /// <value>
        /// Group of event types distinguished by categoryId
        /// </value>
        public String? Category { get; set; }

        /// <summary>
        /// list of calendar items in each specific category
        /// </summary>
        /// <value>
        /// List of calendar items
        /// </value>
        public List<CalendarItem>? Items { get; set; }

        /// <summary>
        /// Total time used with calendar items per category
        /// </summary>
        /// <value>
        /// Total amount of minutes used
        /// </value>
        public Double TotalBusyTime { get; set; }

    }


}
