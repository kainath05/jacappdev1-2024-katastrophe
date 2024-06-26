﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Data.SQLite;
using System.Security.Cryptography;
using static Calendar.Category;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Calendar
{
    // ====================================================================
    // CLASS: categories
    //        - A collection of category items
    // ====================================================================
    /// <summary>
    /// Categories class represents a collection of category items, providing functionality to read/write to a database
    /// </summary>
    public class Categories
    {
        /// <summary>
        /// Database connection for categories
        /// </summary>
        private SQLiteConnection connection;

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
        /// </summary
        /// <param name="i">The id of the category to retrieve</param>
        /// <returns>The category with the specified id</returns>
        /// <exception cref="Exception">Thrown if the category is equal to null</exception> 
        /// <example>
        /// <code>
        /// try
        /// {
        ///     int categoryId = 1; // Assuming you want to get the category with ID 1
        ///     Category category = categories.GetCategoryFromId(categoryId);
        ///
        ///     Console.WriteLine($"Category Found: ID = {category.Id}, Description = {category.Description}, Type = {category.Type}");
        /// }
        /// catch (Exception ex)
        /// {
        ///     Console.WriteLine("Error retrieving category: " + ex.Message);
        /// }
        /// </code>
        /// </example>
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
        // set categories to default
        // ====================================================================

        /// <summary>
        /// Sets categories to default values
        /// </summary>
        /// <example>
        /// <code>
        /// // Resets categories to default values
        /// categories.SetCategoriesToDefaults();
        /// 
        /// Console.WriteLine("Categories reset to default values.");
        /// </code>
        /// </example>
        public void SetCategoriesToDefaults()
        {
            // ---------------------------------------------------------------
            // reset any current categories and category types
            // ---------------------------------------------------------------
            using (SQLiteCommand clearCommand = new SQLiteCommand(connection))
            {
                clearCommand.CommandText = "DELETE FROM categories";
                clearCommand.ExecuteNonQuery();
                clearCommand.CommandText = "DELETE FROM categoryTypes";
                clearCommand.ExecuteNonQuery();

                foreach (CategoryType categoryType in Enum.GetValues(typeof(CategoryType)))
                {
                    // Prepare the SQL command for inserting into the categoryTypes table
                    clearCommand.CommandText = @"INSERT INTO categoryTypes(Description) VALUES (@desc)";
                    clearCommand.Parameters.AddWithValue("@desc", categoryType.ToString());
                    clearCommand.ExecuteNonQuery();
                }
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
        /// Adds a category with the specified description and type
        /// </summary>
        /// <param name="desc">The description of the category</param>
        /// <param name="type">The type of the category</param>
        /// <example>
        /// <code>
        /// string description = "Work";
        /// Category.CategoryType type = Category.CategoryType.Event;
        /// 
        /// // Add new category
        /// categories.Add(description, type);
        /// 
        /// Console.WriteLine($"Added new category: {description}");
        /// </code>
        /// </example>
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
        /// <example>
        /// <code>
        /// int categoryIdToDelete = 2; // Assuming you want to delete the category with ID 2
        /// 
        /// try
        /// {
        ///     categories.Delete(categoryIdToDelete);
        ///     Console.WriteLine($"Category with ID {categoryIdToDelete} deleted successfully.");
        /// }
        /// catch (Exception ex)
        /// {
        ///     Console.WriteLine($"Error deleting category: {ex.Message}");
        /// }
        /// </code>
        /// </example>
        public void Delete(int id)
        {
            bool eventDeleted = false;
            bool categoryDeleted = false;

            var deleteEventsCommandText = "DELETE FROM events WHERE CategoryId = @Id";
            var deleteCategoryCommandText = "DELETE FROM categories WHERE Id = @Id";

            // Delete referencing rows from events
            using (var cmd = new SQLiteCommand(deleteEventsCommandText, connection))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                int rowsAffected = cmd.ExecuteNonQuery();

                // Check if any rows were affected
                if (rowsAffected > 0)
                {
                    eventDeleted = true;
                }
            }
            using (var cmd = new SQLiteCommand(deleteCategoryCommandText, connection))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                int rowsAffected = cmd.ExecuteNonQuery();

                // Check if any rows were affected
                if (rowsAffected > 0)
                {
                    categoryDeleted = true;
                }
            }

            // Check if either the event or category was deleted
            if (!eventDeleted && !categoryDeleted)
            {
                throw new Exception($"ID {id} not found.");
            }
        }

        /// <summary>
        /// Updates the properties of a specific category
        /// </summary>
        /// <param name="id">The id of the category to be updated</param>
        /// <param name="newDescr">the new description to be added to the category</param>
        /// <param name="type">The type of category to be changed, defaulted to Event category type.</param>
        /// <example>
        /// <code>
        /// int categoryIdToUpdate = 3; // Assuming you want to update the category with ID 3
        /// string newDescription = "Work - Updated";
        /// Category.CategoryType newType = Category.CategoryType.Event;
        /// 
        /// categories.UpdateProperties(categoryIdToUpdate, newDescription, newType);
        /// 
        /// Console.WriteLine($"Category with ID {categoryIdToUpdate} updated. New Description: {newDescription}, New Type: {newType}");
        /// </code>
        /// </example>
        public void UpdateProperties(int id, string newDescr, Category.CategoryType type = Category.CategoryType.Event)
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "UPDATE Categories SET Description = @Description, TypeId = @TypeId WHERE Id = @Id";
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Description", newDescr);
                cmd.Parameters.AddWithValue("@TypeId", (int)type); //converting the enum into int with casting
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
        /// <example>
        /// <code>
        /// List<Category> categoryList = categories.List();
        /// 
        /// Console.WriteLine("Categories List:");
        /// foreach (var category in categoryList)
        /// {
        ///     Console.WriteLine($"ID: {category.Id}, Description: {category.Description}, Type: {category.Type}");
        /// }
        /// </code>
        /// </example>
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
                        Category.CategoryType type = (Category.CategoryType)Enum.Parse(typeof(Category.CategoryType), Convert.ToString(reader["TypeId"])); //Converting the type id to enum and find the category type
                        categories.Add(new Category(id, description, type));
                    }
                }
            }
            return categories;
        }
    }
}

