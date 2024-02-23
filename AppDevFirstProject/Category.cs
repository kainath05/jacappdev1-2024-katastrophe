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
    // CLASS: Category
    //        - An individual category for Calendar program
    //        - Valid category types: Event,AllDayEvent,Holiday
    // ====================================================================

    /// <summary>
    /// Represents an individual category
    /// </summary>
    public class Category
    {
        // ====================================================================
        // Properties
        // ====================================================================

        /// <summary>
        /// Gets or sets the id for the category
        /// </summary>
        /// <value>
        /// Unique number
        /// </value>
        public int Id { get; }

        /// <summary>
        /// Provides a short description of the category
        /// </summary>
        /// <value>
        /// A short summary of something
        /// </value>
        public String Description { get; }

        /// <summary>
        /// Represents the specific category type
        /// </summary>
        /// <value>
        /// The type of category
        /// </value>
        public CategoryType Type { get; } //We would make it typeid instead of this?

        /// <summary>
        /// Represents the valid category types: Event, AllDayEvent, Holiday
        /// </summary>
        /// <value>
        /// Different category types
        /// </value>
        public enum CategoryType
        {
            /// <summary>
            /// Regular activity
            /// </summary>
            Event,
            /// <summary>
            /// Activity that lasts all day
            /// </summary>
            AllDayEvent,
            /// <summary>
            /// Special event for everyone
            /// </summary>
            Holiday,
            /// <summary>
            /// If an event is available
            /// </summary>
            Availability
        };

        // ====================================================================
        // Constructor
        // ====================================================================

        /// <summary>
        /// Constructor which initializes the Category with params
        /// </summary>
        /// <param name="id">Id of the category</param>
        /// <param name="description">A short description of the category</param>
        /// <param name="type">The specific type of category</param>
        public Category(int id, String description, CategoryType type = CategoryType.Event)
        {
            this.Id = id;
            this.Description = description;
            this.Type = type;
        }

        // ====================================================================
        // Copy Constructor
        // ====================================================================

        /// <summary>
        /// Initializes a category without params
        /// </summary>
        /// <param name="category">Category object which holds all the params from the other constructor</param>
        public Category(Category category)
        {
            this.Id = category.Id;;
            this.Description = category.Description;
            this.Type = category.Type;
        }
        // ====================================================================
        // String version of object
        // ====================================================================

        /// <summary>
        /// method to turn a description into a string
        /// </summary>
        /// <returns>returns the description in string format</returns>
        public override string ToString()
        {
            return Description;
        }

    }
}

