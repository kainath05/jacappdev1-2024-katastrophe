using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Data.SQLite;
using System.Security.Cryptography;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Calendar
{
    // ====================================================================
    // CLASS: categories
    //        - A collection of category items,
    //        - Read / write to file
    //        - etc
    // ====================================================================
    /// <summary>
    ///Categories class represents a collection of category items, providing functionality to read/write to a database
    /// </summary>
    public class Categories
    {
        private static String DefaultFileName = "calendarCategories.txt";
        private List<Category> _Categories = new List<Category>();
        private string? _FileName;
        private string? _DirName;
        private SQLiteConnection connection;


        // ====================================================================
        // Properties
        // ====================================================================

        /// <summary>
        /// gets the file name associated with the categories
        /// </summary>
        /// <value>
        /// Name of file
        /// </value>
        public String? FileName { get { return _FileName; } }

        /// <summary>
        /// Gets the directory name associated with the categories
        /// </summary>
        /// <value>
        /// Directory name of file
        /// </value>
        public String? DirName { get { return _DirName; } }

        // ====================================================================
        // Constructor
        // ====================================================================
        /// <summary>
        /// Constructor for the Categories class
        /// </summary>
        public Categories(SQLiteConnection conn, bool newDB)
        {
            if (conn == null)
            {
                throw new ArgumentNullException(nameof(conn), "The database connection cannot be null.");
            }

            this.connection = conn;
            if (newDB)
            {
                SetCategoriesToDefaults(); // Now connection should not be null when used here
            }
        }

        // ====================================================================
        // get a specific category from the list where the id is the one specified
        // ====================================================================

        /// <summary>
        /// Gets a specific category from the id associated to it
        /// </summary>
        /// <param name="i">The id of the category to retrieve</param>
        /// <returns>The category with the specified id</returns>
        /// <exception cref="Exception">Thrown if the category is equal to null</exception>
        public Category GetCategoryFromId(int i)
        {
            Category category = null;

            string query = "SELECT Id, Description, TypeId FROM categories WHERE Id = @Id";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", i);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["Id"]);
                        string description = Convert.ToString(reader["Description"]);
                        int typeId = Convert.ToInt32(reader["TypeId"]);
                        Category.CategoryType type = (Category.CategoryType)typeId; 
                        category = new Category(id, description, type);
                    }
                }
            }

            if (category == null)
            {
                throw new Exception();
            }

            return category;
        }

        // ====================================================================
        // populate categories from a file
        // if filepath is not specified, read/save in AppData file
        // Throws System.IO.FileNotFoundException if file does not exist
        // Throws System.Exception if cannot read the file correctly (parsing XML)
        // ====================================================================

        /// <summary>
        /// Populates categories from a file. If filepath is not specified, read/save in AppData file.
        /// </summary>
        /// <param name="filepath">The file path to read from</param>
        public void ReadFromFile(String? filepath = null)
        {

            // ---------------------------------------------------------------
            // reading from file resets all the current categories,
            // ---------------------------------------------------------------
            _Categories.Clear();

            // ---------------------------------------------------------------
            // reset default dir/filename to null 
            // ... filepath may not be valid, 
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if it doesn't exist)
            // ---------------------------------------------------------------
            filepath = CalendarFiles.VerifyReadFromFileName(filepath, DefaultFileName);

            // ---------------------------------------------------------------
            // If file exists, read it
            // ---------------------------------------------------------------
            _ReadXMLFile(filepath);
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);
        }

        // ====================================================================
        // save to a file
        // if filepath is not specified, read/save in AppData file
        // ====================================================================

        /// <summary>
        /// Saves categories to a file. If filepath is not specified, read/save in AppData file.
        /// </summary>
        /// <param name="filepath">The file path to save to</param>
        public void SaveToFile(String? filepath = null)
        {
            // ---------------------------------------------------------------
            // if file path not specified, set to last read file
            // ---------------------------------------------------------------
            if (filepath == null && DirName != null && FileName != null)
            {
                filepath = DirName + "\\" + FileName;
            }

            // ---------------------------------------------------------------
            // just in case filepath doesn't exist, reset path info
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if it doesn't exist)
            // ---------------------------------------------------------------
            filepath = CalendarFiles.VerifyWriteToFileName(filepath, DefaultFileName);

            // ---------------------------------------------------------------
            // save as XML
            // ---------------------------------------------------------------
            _WriteXMLFile(filepath);

            // ----------------------------------------------------------------
            // save filename info for later use
            // ----------------------------------------------------------------
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);
        }

        // ====================================================================
        // set categories to default
        // ====================================================================

        /// <summary>
        /// Sets categories to default values
        /// </summary>
        public void SetCategoriesToDefaults()
        {
            // ---------------------------------------------------------------
            // reset any current categories,
            // ---------------------------------------------------------------
            using (SQLiteCommand clearCommand = new SQLiteCommand(connection))
            {
                clearCommand.CommandText = "DELETE FROM categories";
                clearCommand.ExecuteNonQuery();
                clearCommand.CommandText = "DELETE FROM categoryTypes";
                clearCommand.ExecuteNonQuery();

                //clearCommand.CommandText = @"INSERT INTO categoryTypes (Id, Description) VALUES (0, 'Event'), (1, 'AllDayEvent'), (2, 'Holiday'), (3, 'Availability'); ";
                clearCommand.CommandText = @"INSERT INTO categoryTypes(Description) VALUES (@desc)";
                clearCommand.Parameters.AddWithValue("@desc", "Event");
                clearCommand.ExecuteNonQuery();

                clearCommand.CommandText = @"INSERT INTO categoryTypes(Description) VALUES (@desc)";
                clearCommand.Parameters.AddWithValue("@desc", "AllDayEvent");
                clearCommand.ExecuteNonQuery();

                clearCommand.CommandText = @"INSERT INTO categoryTypes(Description) VALUES (@desc)";
                clearCommand.Parameters.AddWithValue("@desc", "Holiday");
                clearCommand.ExecuteNonQuery();

                clearCommand.CommandText = @"INSERT INTO categoryTypes(Description) VALUES (@desc)";
                clearCommand.Parameters.AddWithValue("@desc", "Availability");
                clearCommand.ExecuteNonQuery();
            }


            // ---------------------------------------------------------------
            // Add Defaults
            // ---------------------------------------------------------------
            Add("School", Category.CategoryType.Event);
            Add("Personal", Category.CategoryType.Event);
            Add("VideoGames", Category.CategoryType.Event);
            Add("Medical", Category.CategoryType.Event);
            Add("Sleep", Category.CategoryType.Event);
            Add("Vacation", Category.CategoryType.AllDayEvent);
            Add("Travel days", Category.CategoryType.AllDayEvent);
            Add("Canadian Holidays", Category.CategoryType.Holiday);
            Add("US Holidays", Category.CategoryType.Holiday);
        }

        // ====================================================================
        // Add category
        // ====================================================================

        /// <summary>
        /// Adds a category
        /// </summary>
        /// <param name="category">The category to add</param>
        private void Add(Category category)
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                int typeId = (int)category.Type; // Get the corresponding TypeId
                cmd.CommandText = "INSERT INTO categories (Description, TypeId) VALUES (@Description, @TypeId)";
                cmd.Parameters.AddWithValue("@Description", category.Description);
                cmd.Parameters.AddWithValue("@TypeId", typeId);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Adds a category with the specified description and type
        /// </summary>
        /// <param name="desc">The description of the category</param>
        /// <param name="type">The type of the category</param>

        public void Add(string desc, Category.CategoryType type)
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "INSERT INTO categories (Description, TypeId) VALUES (@Description, @TypeId)";
                cmd.Parameters.AddWithValue("@Description", desc);
                cmd.Parameters.AddWithValue("@TypeId", (int)type);
                cmd.ExecuteNonQuery();
            }
        }

       

        // ====================================================================
        // Delete category
        // ====================================================================

        /// <summary>
        /// Deletes a category with the specified id
        /// </summary>
        /// <param name="id">The id of the category to be deleted</param>
        public void Delete(int id)
        {
            using (var transaction = connection.BeginTransaction())
            {
                var deleteEventsCommandText = "DELETE FROM events WHERE CategoryId = @Id";
                var deleteCategoryCommandText = "DELETE FROM categories WHERE Id = @Id";

                // Delete referencing rows from events
                using (var cmd = new SQLiteCommand(deleteEventsCommandText, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }

                // Delete the category
                using (var cmd = new SQLiteCommand(deleteCategoryCommandText, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }

        /// <summary>
        /// Updates the properties of a specific category
        /// </summary>
        /// <param name="id">The id of the category to be updated</param>
        /// <param name="newDescr">the new description to be added to the category</param>
        /// <param name="type">The type of category to be changed</param>
        public void UpdateProperties(int id, string newDescr, Category.CategoryType type = Category.CategoryType.Event)
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "UPDATE Categories SET Description = @Description, TypeId = @TypeId WHERE Id = @Id";
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Description", newDescr);
                cmd.Parameters.AddWithValue("@TypeId", (int)type);
                cmd.ExecuteNonQuery();
            }
        }

        // ====================================================================
        // Return list of categories
        // Note:  make new copy of list, so user cannot modify what is part of
        //        this instance
        // ====================================================================

        /// <summary>
        /// Returns a list of categories 
        /// </summary>
        /// <returns>A new copy of the list of categories.</returns>
        public List<Category> List()
        {
            List<Category> categories = new List<Category>();

            // Query the database for all categories
            string query = "SELECT Id, Description, TypeId FROM categories";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["Id"]);
                        string description = Convert.ToString(reader["Description"]);
                        int typeId = Convert.ToInt32(reader["TypeId"]);
                        Category.CategoryType type = (Category.CategoryType)typeId;
                        categories.Add(new Category(id, description, type));
                    }
                }
            }
            return categories;
        }


        // ====================================================================
        // read from an XML file and add categories to our categories list
        // ====================================================================

        /// <summary>
        /// Reads categories from an XML file and adds them to the categories list
        /// </summary>
        /// <param name="filepath">The file path to read from.</param>
        /// <exception cref="Exception">Exception thrown if there is an error trying to read the XML file</exception>
        private void _ReadXMLFile(String filepath)
        {

            // ---------------------------------------------------------------
            // read the categories from the xml file, and add to this instance
            // ---------------------------------------------------------------
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filepath);

                foreach (XmlNode category in doc.DocumentElement.ChildNodes)
                {
                    String id = (((XmlElement)category).GetAttributeNode("ID")).InnerText;
                    String typestring = (((XmlElement)category).GetAttributeNode("type")).InnerText;
                    String desc = ((XmlElement)category).InnerText;

                    Category.CategoryType type;
                    switch (typestring.ToLower())
                    {
                        case "event":
                            type = Category.CategoryType.Event;
                            break;
                        case "alldayevent":
                            type = Category.CategoryType.AllDayEvent;
                            break;
                        case "holiday":
                            type = Category.CategoryType.Holiday;
                            break;
                        case "availability":
                            type = Category.CategoryType.Availability;
                            break;
                        default:
                            type = Category.CategoryType.Event;
                            break;
                    }
                    this.Add(new Category(int.Parse(id), desc, type));
                }

            }
            catch (Exception e)
            {
                throw new Exception("ReadXMLFile: Reading XML " + e.Message);
            }

        }


        // ====================================================================
        // write all categories in our list to XML file
        // ====================================================================

        /// <summary>
        /// Writes all categories in the list to an XML file
        /// </summary>
        /// <param name="filepath">The file path to write to</param>
        /// <exception cref="Exception">Exception thrown if there is an error trying to write to the XML file</exception>
        private void _WriteXMLFile(String filepath)
        {
            try
            {
                // create top level element of categories
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<Categories></Categories>");

                // foreach Category, create an new xml element
                foreach (Category cat in _Categories)
                {
                    XmlElement ele = doc.CreateElement("Category");
                    XmlAttribute attr = doc.CreateAttribute("ID");
                    attr.Value = cat.Id.ToString();
                    ele.SetAttributeNode(attr);
                    XmlAttribute type = doc.CreateAttribute("type");
                    type.Value = cat.Type.ToString();
                    ele.SetAttributeNode(type);

                    XmlText text = doc.CreateTextNode(cat.Description);
                    doc.DocumentElement.AppendChild(ele);
                    doc.DocumentElement.LastChild.AppendChild(text);

                }

                // write the xml to FilePath
                doc.Save(filepath);

            }
            catch (Exception e)
            {
                throw new Exception("_WriteXMLFile: Reading XML " + e.Message);
            }
        }

    }
}

