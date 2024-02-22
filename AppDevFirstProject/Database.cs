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

        public static SQLiteConnection dbConnection { get { return _connection; } }
        private static SQLiteConnection _connection;

        // ===================================================================
        // create and open a new database
        // ===================================================================
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
        private static void CreateTables()
        {
            // Commands to create tables and foreign keys
            string[] commands = new string[]
            {
                "CREATE TABLE categoryTypes (Id INTEGER PRIMARY KEY, Description TEXT NOT NULL)",
                "CREATE TABLE categories (Id INTEGER PRIMARY KEY, Description TEXT NOT NULL, TypeId INTEGER, FOREIGN KEY(TypeId) REFERENCES categoryTypes(Id))",
                "CREATE TABLE events (Id INTEGER PRIMARY KEY, CategoryId INTEGER, DurationInMinutes INTEGER, StartDateTime TEXT, Details TEXT, FOREIGN KEY(CategoryId) REFERENCES categories(Id))"
            };

            foreach (var cmdText in commands)
            {
                using (var cmd = new SQLiteCommand(cmdText, _connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

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
