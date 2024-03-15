using System;
using Xunit;
using System.IO;
using System.Collections.Generic;
using Calendar;
using System.Data.SQLite;

namespace CalendarCodeTests
{
    public class TestHomeCalendar
    {
        // ========================================================================

        [Fact]
        public void HomeCalendar_InitializeWithExistingDatabase()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";

            // Act
            HomeCalendar calendar = new HomeCalendar(newDB);

            // Assert
            Assert.NotNull(calendar);
            Assert.NotNull(calendar.categories);
            Assert.NotNull(calendar.events);
        }

        
        [Fact]
        public void HomeCalendar_InitializeWithNewDatabase()
        {
            // Arrange
            string databaseFile = "new_database.db";

            // Act
            HomeCalendar calendar = new HomeCalendar(databaseFile, true);

            // Assert
            Assert.NotNull(calendar);
            Assert.NotNull(calendar.categories);
            Assert.NotNull(calendar.events);
        }

        [Fact]
        public void HomeCalendar_AddCategory()
        {
            // Arrange
            string databaseFile = "test_database.db";
            HomeCalendar calendar = new HomeCalendar(databaseFile, true);

            // Act
            calendar.categories.Add("Test", Category.CategoryType.Event);

            // Assert
            Category addedCategory = calendar.categories.List().Find(c => c.Description == "Test");
            Assert.NotNull(addedCategory);
        }

        [Fact]
        public void HomeCalendar_InitializeWithFakeDatabase()
        {
            // Arrange
            string db = "existing.db"; //Should make new database when cannot find this.

            // Act
            HomeCalendar calendar = new HomeCalendar(db, true);

            // Assert
            Assert.NotNull(calendar);
            Assert.IsType<HomeCalendar>(calendar);
        }


    }
}

