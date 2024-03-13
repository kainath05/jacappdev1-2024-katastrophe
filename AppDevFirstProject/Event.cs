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
    // CLASS: Event
    //        - An individual event for calendar program
    // ====================================================================

    /// <summary>
    /// Class event defines attributes of an event, represents an event
    /// </summary>
    public class Event
    {
        // ====================================================================
        // Properties
        // ====================================================================

        /// <summary>
        ///  Gets the id of the event
        /// </summary>
        /// <value>
        /// Unique number
        /// </value>
        public int Id { get; }

        /// <summary>
        /// Gets the start date of the event
        /// </summary>
        /// <value>
        /// starting time of an event
        /// </value>
        public DateTime StartDateTime { get;  }

        /// <summary>
        /// Length of the event in minutes
        /// </summary>
        /// <value>
        /// Length in minutes of an event
        /// </value>
        public Double DurationInMinutes { get; }

        /// <summary>
        /// Get and set details of the event in string format
        /// </summary>
        /// <value>
        /// Short description of an event
        /// </value>
        public String Details { get;  }

        /// <summary>
        /// Get and set the id of the event
        /// </summary>
        /// <value>
        /// Categorization of events
        /// </value>
        public int Category { get; }

        // ====================================================================
        // Constructor
        //    NB: there is no verification the event category exists in the
        //        categories object
        // ====================================================================

        /// <summary>
        /// Initializes a new instance of the event class with params
        /// </summary>
        /// <param name="id">The id of the event</param>
        /// <param name="date">The date of the event</param>
        /// <param name="category">The category of the event</param>
        /// <param name="duration">The duration of the event in minutes</param>
        /// <param name="details">the details of the event(string)</param>
        public Event(int id, DateTime date, int category, Double duration, String details)
        {
            this.Id = id;
            this.StartDateTime = date;
            this.Category = category;
            this.DurationInMinutes = duration;
            this.Details = details;
        }

        // ====================================================================
        // Copy constructor - does a deep copy
        // ====================================================================

        /// <summary>
        /// Initializes an instance of an event with obj param
        /// </summary>
        /// <param name="obj">Event object passed in for initialization</param>
        public Event (Event obj)
        {
            this.Id = obj.Id;
            this.StartDateTime = obj.StartDateTime;
            this.Category = obj.Category;
            this.DurationInMinutes = obj.DurationInMinutes;
            this.Details = obj.Details;
           
        }
    }
}
