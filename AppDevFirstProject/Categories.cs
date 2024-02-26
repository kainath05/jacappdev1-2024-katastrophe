using System;
using System.Collections.Generic;
using System.Linq;
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
    // CLASS: categories
    //        - A collection of category items,
    //        - Read / write to file
    //        - etc
    // ====================================================================
    /// <summary>
    ///Categories class represents a collection of category items, providing functionality to read/write to a file and manage categories
    /// </summary>
    public class Categories
    {
        private string connectionString = @"URI=file:test?";

        private static String DefaultFileName = "calendarCategories.txt";
        private List<Category> _Categories = new List<Category>();
        private string? _FileName;
        private string? _DirName;

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
            if (newDB == true)
            {
                SetCategoriesToDefaults();
            }
            else
            {
                using SQLiteConnection connection = new SQLiteConnection(conn);
                connection.Open();


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
        /// <exception cref="Exception">Thrown if the specific id is not found associated to a category</exception>
        public Category GetCategoryFromId(int i)
        {
            //Category? c = _Categories.Find(x => x.Id == i);
            //if (c == null)
            //{
            //    throw new Exception("Cannot find category with id " + i.ToString());
            //}
            //return c
            Category category = null; // Initialize outside the using block

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
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
                            Category.CategoryType type = (Category.CategoryType)typeId; //IDK
                            category = new Category(id, description, type);
                        }
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
        /// <exception cref="FileNotFoundException">Thrown if the specified file does not exist</exception>
        /// <exception cref="Exception">Thrown if the file cannot be read correctly (parsing XML)</exception>
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
            _Categories.Clear();

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
            //_Categories.Add(category);
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO categories (Description, TypeId) VALUES (@Description, @TypeId)";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Description", category.Description);
                    command.Parameters.AddWithValue("@TypeId", (int)category.Type); //would this work?
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Adds a category with the specified description and type
        /// </summary>
        /// <param name="desc">The description of the category</param>
        /// <param name="type">The type of the category</param>
        public void Add(String desc, Category.CategoryType type)
        {
            //int new_num = 1;
            //if (_Categories.Count > 0)
            //{
            //    new_num = (from c in _Categories select c.Id).Max();
            //    new_num++;
            //}
            //_Categories.Add(new Category(new_num, desc, type));
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO categories (Description, TypeId) VALUES (@Description, @TypeId)";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Description", desc);
                    command.Parameters.AddWithValue("@TypeId", type); //would this work?
                    command.ExecuteNonQuery();
                }
            }
        }

        // ====================================================================
        // Delete category
        // ====================================================================

        /// <summary>
        /// Deletes a category with the specified id
        /// </summary>
        /// <param name="Id">The id of the category to be delete</param>
        public void Delete(int Id)
        {
            //try
            //{

            //int i = _Categories.FindIndex(x => x.Id == Id);
            //_Categories.RemoveAt(i);
            //}catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM categories WHERE Id = @Id";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", Id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateProperties(int Id, string newDescr, Category.CategoryType type = Category.CategoryType.Event)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string selectTypeQuery = "SELECT Type FROM Category where Id = @id";
                using (SQLiteCommand command = new SQLiteCommand(selectTypeQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", Id);
                }
                string query = "UPDATE category SET Description = @newdescr, Type = @type";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@newDescr", newDescr);
                    command.Parameters.AddWithValue("@type", type);
                    command.ExecuteNonQuery();
                }
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
            List<Category> newList = new List<Category>();
            foreach (Category category in _Categories)
            {
                newList.Add(new Category(category));
            }
            return newList;
        }

        // ====================================================================
        // read from an XML file and add categories to our categories list
        // ====================================================================

        /// <summary>
        /// Reads categories from an XML file and adds them to the categories list
        /// </summary>
        /// <param name="filepath">The file path to read from.</param>
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

