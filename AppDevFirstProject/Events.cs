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
    //        - Read / write to database
    //        - etc
    // ====================================================================
    /// <summary>
    /// Represents a collection of Event items with functionality
    /// </summary>
    public class Events
    {

        /// <summary>
        /// A property which initializes a connection to the database
        /// </summary>
        private SQLiteConnection connection;

        /// <summary>
        /// Initializes a new instance of the Events class with the provided database connection.
        /// </summary>
        /// <param name="conn">The SQLite connection to be used.</param>
        /// <param name="newDB">A bool indicating whether to clear existing data in the database.</param>
        /// <exception cref="ArgumentNullException">Thrown when the database connection is null.</exception>
        /// <example>
        /// <code>
        /// string newDB = “events.db”
        /// Database.newDatabase(newDB);
        /// SQLiteConnection conn = Database.dbConnection;
        /// Events eventsManager = new Events(conn, true);
        /// </code>
        /// </example>
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
        /// Adds an event to the collection with specified parameters.
        /// </summary>
        /// <param name="date">The date of the event.</param>
        /// <param name="category">The category of the event.</param>
        /// <param name="duration">The duration of the event.</param>
        /// <param name="details">The details of the event.</param>
        /// <example>
        /// <code>
        /// Events events = new Events(connection, false);
        /// events.Add(DateTime.Now, 1, 60, "Example event details");
        /// </code>
        /// </example>
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

        /// <summary>
        /// Deletes an event at a specific index.
        /// </summary>
        /// <param name="Id">The specific index to be deleted.</param>
        /// <exception cref="Exception">Thrown when the event with the specified ID is not found.</exception>
        /// <example>
        /// <code>
        /// var events = new Events(connection, false);
        /// events.Delete(1);
        /// </code>
        /// </example>
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
        /// Returns a new copy of the list of events, preventing modification of the original list.
        /// </summary>
        /// <returns>A new list containing copies of the events.</returns>
        /// <example>
        /// <code>
        /// var events = new Events(connection, false);
        /// var eventList = events.List();
        /// foreach (var ev in eventList)
        /// {
        ///     Console.WriteLine($"Event ID: {ev.Id}, StartDateTime: {ev.StartDateTime}, Category: {ev.CategoryId}, Duration: {ev.DurationInMinutes} minutes, Details: {ev.Details}");
        /// }
        /// </code>
        /// </example>
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


        /// <summary>
        /// Updates properties of an event with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the event to be updated.</param>
        /// <param name="StartDateTime">The updated start date and time of the event.</param>
        /// <param name="DurationInMinutes">The updated duration of the event in minutes.</param>
        /// <param name="Details">The updated details of the event.</param>
        /// <param name="Category">The updated category of the event.</param>
        /// <example>
        /// <code>
        /// var events = new Events(connection, false);
        /// var firstEvent = events.List()[0];
        /// events.UpdateProperties(firstEvent.Id, DateTime.Now, 90, "Updated details", 2);
        /// </code>
        /// </example>
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

