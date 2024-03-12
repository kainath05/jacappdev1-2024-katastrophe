using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Threading;

// ===================================================================
// Very important notes:
// ... To keep everything working smoothly, you should always
//     dispose of EVERY SQLiteCommand even if you recycle a 
//     SQLiteCommand variable later on.
//     EXAMPLE:
//            Database.newDatabase(GetSolutionDir() + "\\" + filename);
//            var cmd = new SQLiteCommand(Database.dbConnection);
//            cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES('Whatever')";
//            cmd.ExecuteNonQuery();
//            cmd.Dispose();
//
// ... also dispose of reader objects
//
// ... by default, SQLite does not impose Foreign Key Restraints
//     so to add these constraints, connect to SQLite something like this:
//            string cs = $"Data Source=abc.sqlite; Foreign Keys=1";
//            var con = new SQLiteConnection(cs);
//
// ===================================================================


namespace Calendar
{
    public class Database
    {
        /// <summary>
        /// Gets the current SQLite database connection.
        /// </summary>
        /// <value>
        /// The SQLiteConnection object representing the open connection to the database. If no connection is open, this value will be null.
        /// </value>
        public static SQLiteConnection dbConnection { get { return _connection; } }
        private static SQLiteConnection _connection;

        // ===================================================================
        // create and open a new database
        // ===================================================================
        /// <summary>
        /// Creates and opens a new SQLite database with the specified filename, sets up the necessary tables, and enables foreign key constraints.
        /// </summary>
        /// <param name="filename">The filename for the new SQLite database file. If a file with this name already exists, it will be overwritten.</param>
        /// <example>
        /// <code>
        /// // Example usage:
        /// string filename = "myNewCalendarDB.sqlite";
        /// Database.newDatabase(filename);
        /// </code>
        /// </example>
        public static void newDatabase(string filename)
        {
            CloseDatabaseAndReleaseFile();

            // Check if file exists, delete if it does
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            SQLiteConnection.CreateFile(filename); // Create new database file
            string cs = $"Data Source={filename}; Foreign Keys=1;"; // Enable foreign keys
            _connection = new SQLiteConnection(cs);
            _connection.Open();

            // Create tables
            CreateTables();
        }

        private static void CreateTables()
        {

            string dropTables = @"DROP TABLE IF EXISTS categoryTypes; DROP TABLE IF EXISTS categories; DROP TABLE IF EXISTS events;";

            string createCategoryTypesTable = @"
    CREATE TABLE IF NOT EXISTS categoryTypes (
        Id INTEGER PRIMARY KEY,
        Description TEXT NOT NULL
    );";


            string createCategoriesTable = @"
    CREATE TABLE IF NOT EXISTS categories (
        Id INTEGER PRIMARY KEY,
        Description TEXT,
        TypeId INTEGER,
        FOREIGN KEY(TypeId) REFERENCES categoryTypes(Id)
    );";

            string createEventsTable = @"
    CREATE TABLE IF NOT EXISTS events (
        Id INTEGER PRIMARY KEY,
        CategoryId INTEGER,
        DurationInMinutes INTEGER,
        StartDateTime DATETIME,
        Details TEXT,
        FOREIGN KEY(CategoryId) REFERENCES categories(Id)
    );";
            ExecuteNonQuery(dropTables);
            ExecuteNonQuery(createCategoryTypesTable);
            ExecuteNonQuery(createCategoriesTable);
            ExecuteNonQuery(createEventsTable);
        }

        private static void ExecuteNonQuery(string sql)
        {
            using (var command = new SQLiteCommand(sql, _connection))
            {
                command.ExecuteNonQuery();
                command.Dispose();
            }
        }

        // ===================================================================
        // open an existing database
        // ===================================================================
        public static void existingDatabase(string filename)
        {
            CloseDatabaseAndReleaseFile();

            string cs = $"Data Source={filename};Foreign Keys=1;"; 
            _connection = new SQLiteConnection(cs);
            _connection.Open();
        }

        // ===================================================================
        // close existing database, wait for garbage collector to
        // release the lock before continuing
        // ===================================================================

        public static void CloseDatabaseAndReleaseFile()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose(); // Proper disposal
                _connection = null; // Ensure the reference is cleared to prevent reuse

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }

}
