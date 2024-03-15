using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Data.SQLite;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Calendar
{
    // ====================================================================
    // CLASS: Events
    //        - A collection of Event items,
    //        - Read / write to file
    //        - etc
    // ====================================================================
    /// <summary>
    /// Represents a collection of Event items with functionality for reading and writing to a file
    /// </summary>
    public class Events
    {

        /// <summary>
        /// A property which initializes a connection to the database
        /// </summary>
        private SQLiteConnection connection;

        public Events(SQLiteConnection conn, bool newDB)
        {
            if (conn == null)
            {
                throw new ArgumentNullException(nameof(conn), "The database connection cannot be null.");
            }

            this.connection = conn;
            if (newDB)
            {
                using (SQLiteCommand clearCommand = new SQLiteCommand(connection))
                {
                    clearCommand.CommandText = "DELETE FROM events";
                    clearCommand.ExecuteNonQuery();
                    clearCommand.CommandText = "DELETE FROM categories";
                    clearCommand.ExecuteNonQuery();
                }
            }
        }

        // ====================================================================
        // Add Event
        // ====================================================================

        /// <summary>
        /// Adds an event to the collection with specified parameters
        /// </summary>
        /// <param name="date">The date of the event</param>
        /// <param name="category">The category of the event</param>
        /// <param name="duration">The duration of the event</param>
        /// <param name="details">The details of the event</param>
        public void Add(DateTime date, int category, Double duration, String details)
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                    cmd.CommandText = "INSERT INTO events(CategoryId, DurationInMinutes, StartDateTime, Details) VALUES (@CategoryId, @DurationInMinutes, @StartDateTime, @Details)";
                    cmd.Parameters.AddWithValue("@CategoryId", category);
                    cmd.Parameters.AddWithValue("@DurationInMinutes", duration);
                    cmd.Parameters.AddWithValue("@StartDateTime", date); // M/d/yyyy h:mm:ss tt cultureojrgjhewrgjwrjklbg
                    cmd.Parameters.AddWithValue("@Details", details);
                    cmd.ExecuteNonQuery();
            }
        }

        // ====================================================================
        // Delete Event
        // ====================================================================

        /// <summary>Deletes an event at a specific index
        /// </summary>
        /// <param name="Id">the specific index to be deleted</param>
        public void Delete(int Id)
        {
            bool eventDeleted = false;
            var delete = "DELETE FROM events WHERE Id = @Id";
            using (var cmd = new SQLiteCommand(delete, connection))
            {
                cmd.Parameters.AddWithValue("@Id", Id);
                int rowsAffected = cmd.ExecuteNonQuery();

                // Check if any rows were affected
                if (rowsAffected > 0)
                {
                    eventDeleted = true;
                }
            }

            if (!eventDeleted)
            {
                throw new Exception($"ID {Id} not found");
            }
        }

        // ====================================================================
        // Return list of Events
        // Note:  make new copy of list, so user cannot modify what is part of
        //        this instance
        // ====================================================================

        /// <summary>
        /// Returns a new copy of the list of events, preventing modification of the original list
        /// </summary>
        /// <returns>A new list containing copies of the events</returns>
        public List<Event> List()
        {
            List<Event> events = new List<Event>();
            string query = "SELECT Id, CategoryId, DurationInMinutes, StartDateTime, Details FROM events ORDER BY Id";
            //e JOIN categories c ON e.CategoryId = c.Id
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["Id"]);
                        int categoryId = Convert.ToInt32(reader["CategoryId"]);
                        double duration = Convert.ToDouble(reader["DurationInMinutes"]);
                        DateTime startDateTime = Convert.ToDateTime(reader["StartDateTime"]);
                        string details = Convert.ToString(reader["Details"]);

                        events.Add(new Event(id, startDateTime, categoryId, duration, details));
                    }
                }
            }

            return events;
        }

        public void UpdateProperties(int id, DateTime StartDateTime, Double DurationInMinutes, String Details, int Category) // What is Date property?
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "UPDATE events SET StartDateTime = @StartDateTime, DurationInMinutes = @DurationInMinutes, Details = @Details, CategoryId = @Category WHERE Id = @Id";
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@StartDateTime", StartDateTime);
                cmd.Parameters.AddWithValue("@DurationInMinutes", DurationInMinutes);
                cmd.Parameters.AddWithValue("@Details", Details);
                cmd.Parameters.AddWithValue("@Category", Category);
                cmd.ExecuteNonQuery();
            }
        }



    }
}

