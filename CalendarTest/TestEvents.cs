using System;
using Xunit;
using System.IO;
using System.Collections.Generic;
using Calendar;
using System.Data.SQLite;

namespace CalendarCodeTests
{
    public class TestEvents
    {
        int numberOfEventsInFile = TestConstants.numberOfEventsInFile;
        String testInputFile = TestConstants.testEventsInputFile;
        int maxIDInEventFile = TestConstants.maxIDInEventFile;
        Event firstEventInFile = new Event(1, new DateTime(2021, 1, 10), 3, 40, "App Dev Homework");


        // ========================================================================

        [Fact]
        public void EventsObject_New() //HI
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            Database.newDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;

            // Act
            Events Events = new Events(conn, true);

            // Assert 
            Assert.IsType<Events>(Events);

        }


        // ========================================================================

        [Fact]
        public void EventsMethod_ReadFromDatabase_ValidateCorrectDataWasRead()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String existingDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.existingDatabase(existingDB);
            SQLiteConnection conn = Database.dbConnection;

            // Act
            Events events = new Events(conn, false);
            List<Event> list = events.List();
            Event firstEvent = list[0];

            // Assert
            Assert.Equal(numberOfEventsInFile, list.Count);
            Assert.Equal(firstEventInFile.Id, firstEvent.Id);
            Assert.Equal(firstEventInFile.DurationInMinutes, firstEvent.DurationInMinutes);
            Assert.Equal(firstEventInFile.Details, firstEvent.Details);
            Assert.Equal(firstEventInFile.Category, firstEvent.Category);


        }

        // ========================================================================

        [Fact]
        public void EventsMethod_List_ReturnsListOfEvents()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.existingDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn, false);

            // Act
            List<Event> list = events.List();

            // Assert
            Assert.Equal(numberOfEventsInFile, list.Count);

        }

        // ========================================================================

        [Fact]
        public void EventsMethod_List_ModifyListDoesNotModifyEventsInstance()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.existingDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn, false);
            List<Event> list = events.List();

            // Act
            list[0].DurationInMinutes = list[0].DurationInMinutes + 21.03; 

            // Assert
            Assert.NotEqual(list[0].DurationInMinutes, events.List()[0].DurationInMinutes);

        }

        // ========================================================================

        [Fact]
        public void EventsMethod_Add()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.existingDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn, false);
            int category = 57;
            double DurationInMinutes = 98.1;

            // Act
            events.Add(DateTime.Now,category,DurationInMinutes,"new Event");
            List<Event> EventsList = events.List();
            int sizeOfList = events.List().Count;

            // Assert
            Assert.Equal(numberOfEventsInFile+1, sizeOfList);
            Assert.Equal(maxIDInEventFile + 1, EventsList[sizeOfList - 1].Id);
            Assert.Equal(DurationInMinutes, EventsList[sizeOfList - 1].DurationInMinutes);

        }

        // ========================================================================

        [Fact]
        public void EventsMethod_Delete()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.existingDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn, false);
            List<Event> list = events.List();
            int IdToDelete = 3;

            // Act
            events.Delete(IdToDelete);
            List<Event> EventsList = events.List();
            int sizeOfList = EventsList.Count;

            // Assert
            Assert.Equal(numberOfEventsInFile - 1, sizeOfList);
            Assert.False(EventsList.Exists(e => e.Id == IdToDelete), "correct Event item deleted");

        }

        public void EventsMethod_Update()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.existingDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn, false);
            List<Event> list = events.List();
            int IdToUpdate = 3;
            DateTime start = DateTime.Now;
            double duration = 120;
            string details = "Updated Event";
            int category = 6;


            // Act
            events.UpdateProperties(IdToUpdate, start, duration, details, category);

            // Assert
            Assert.Equal(list[0].Id, IdToUpdate);
            Assert.Equal(list[0].StartDateTime, start);
            Assert.Equal(list[0].DurationInMinutes, duration);
            Assert.Equal(list[0].Details, details);
            Assert.Equal(list[0].Category, category);

        }

        // ========================================================================

        [Fact]
        public void EventsMethod_Delete_InvalidIDDoesntCrash()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.existingDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn, false);
            List<Event> list = events.List();
            int IdToDelete = 1006;
            int sizeOfList = events.List().Count;

            // Act
            try
            {
                events.Delete(IdToDelete);
                Assert.Equal(sizeOfList, events.List().Count);
            }

            // Assert
            catch
            {
                Assert.True(false, "Invalid ID causes Delete to break");
            }
        }

        

        // ========================================================================

        //[Fact]
        //public void EventMethod_WriteToFile()
        //{
        //    // Arrange
        //    String folder = TestConstants.GetSolutionDir();
        //    String newDB = $"{folder}\\{TestConstants.testDBInputFile}";
        //    Database.existingDatabase(newDB);
        //    SQLiteConnection conn = Database.dbConnection;
        //    Events events = new Events(conn, false);
        //    List<Event> list = events.List();
        //    File.Delete(outputFile);

        //    // Act
        //    Events.SaveToFile(outputFile);

        //    // Assert
        //    Assert.True(File.Exists(outputFile), "output file created");
        //    Assert.True(TestConstants.FileEquals(dir + "\\" + testInputFile, outputFile), "Input /output files are the same");
        //    String fileDir = Path.GetFullPath(Path.Combine(Events.DirName, ".\\"));
        //    Assert.Equal(dir, fileDir);
        //    Assert.Equal(fileName, Events.FileName);

        //    // Cleanup
        //    if (TestConstants.FileEquals(dir + "\\" + testInputFile, outputFile))
        //    {
        //        File.Delete(outputFile);
        //    }

        //}

        //// ========================================================================

        //[Fact]
        //public void EventMethod_WriteToFile_VerifyNewEventWrittenCorrectly()
        //{
        //    // Arrange
        //    String dir = TestConstants.GetSolutionDir();
        //    Events Events = new Events();
        //    Events.ReadFromFile(dir + "\\" + testInputFile);
        //    string fileName = TestConstants.EventOutputTestFile;
        //    String outputFile = dir + "\\" + fileName;
        //    File.Delete(outputFile);

        //    // Act
        //    Events.Add(DateTime.Now, 14, 35.27, "McDonalds");
        //    List<Event> listBeforeSaving = Events.List();
        //    Events.SaveToFile(outputFile);
        //    Events.ReadFromFile(outputFile);
        //    List<Event> listAfterSaving = Events.List();

        //    Event beforeSaving = listBeforeSaving[listBeforeSaving.Count - 1];
        //    Event afterSaving = listAfterSaving.Find(e => e.Id == beforeSaving.Id);

        //    // Assert
        //    Assert.Equal(beforeSaving.Id, afterSaving.Id);
        //    Assert.Equal(beforeSaving.Category, afterSaving.Category);
        //    Assert.Equal(beforeSaving.Details, afterSaving.Details);
        //    Assert.Equal(beforeSaving.DurationInMinutes, afterSaving.DurationInMinutes);

        //}

        //// ========================================================================

        //[Fact]
        //public void EventMethod_WriteToFile_WriteToLastFileWrittenToByDefault()
        //{
        //    // Arrange
        //    String dir = TestConstants.GetSolutionDir();
        //    Events Events = new Events();
        //    Events.ReadFromFile(dir + "\\" + testInputFile);
        //    string fileName = TestConstants.EventOutputTestFile;
        //    String outputFile = dir + "\\" + fileName;
        //    File.Delete(outputFile);
        //    Events.SaveToFile(outputFile); // output file is now last file that was written to.
        //    File.Delete(outputFile);  // Delete the file

        //    // Act
        //    Events.SaveToFile(); // should write to same file as before

        //    // Assert
        //    Assert.True(File.Exists(outputFile), "output file created");
        //    String fileDir = Path.GetFullPath(Path.Combine(Events.DirName, ".\\"));
        //    Assert.Equal(dir, fileDir);
        //    Assert.Equal(fileName, Events.FileName);

        //    // Cleanup
        //    if (TestConstants.FileEquals(dir + "\\" + testInputFile, outputFile))
        //    {
        //        File.Delete(outputFile);
        //    }

        //}
    }
}

