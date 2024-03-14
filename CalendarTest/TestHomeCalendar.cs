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
        public void HomeCalendar_AddEvent() //FAILING
        {
            // Arrange
            string databaseFile = "test_database.db";
            HomeCalendar calendar = new HomeCalendar(databaseFile, true);
            DateTime eventDate = DateTime.Now;
            int categoryId = 1;
            double duration = 120;
            string details = "Test Event";

            // Act
            calendar.events.Add(eventDate, categoryId, duration, details);

            // Assert
            Event addedEvent = calendar.events.List().Find(e => e.Details == details && e.Category == categoryId);
            Assert.NotNull(addedEvent);
        }


    }
}

