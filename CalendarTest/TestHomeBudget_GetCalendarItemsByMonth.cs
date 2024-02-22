﻿using System;
using Xunit;
using System.IO;
using System.Collections.Generic;
using Calendar;

namespace CalendarCodeTests
{
    public class TestHomeCalendar_GetCalendarItemsByMonth
    {
        string testInputFile = TestConstants2.testCalendarFile;
        


        // ========================================================================
        // Get Events By Month Method tests
        // ========================================================================

        [Fact]
        public void HomeCalendarMethod_GetCalendarItemsByMonth_NoStartEnd_NoFilter()
        {
            // Arrange
            string inFile = TestConstants2.GetSolutionDir() + "\\" + testInputFile;
            HomeCalendar homeCalendar = new HomeCalendar(inFile);
            int maxRecords = TestConstants2.CalendarItemsByMonth_MaxRecords;
            CalendarItemsByMonth firstRecord = TestConstants2.CalendarItemsByMonth_FirstRecord;

            // Act
            List<CalendarItemsByMonth> CalendarItemsByMonth = homeCalendar.GetCalendarItemsByMonth(null, null, false, 9);
            CalendarItemsByMonth firstRecordTest = CalendarItemsByMonth[0];

            // Assert
            Assert.Equal(maxRecords, CalendarItemsByMonth.Count);

            // verify 1st record
            Assert.Equal(firstRecord.Month, firstRecordTest.Month);
            Assert.Equal(firstRecord.TotalBusyTime, firstRecordTest.TotalBusyTime);
            Assert.Equal(firstRecord.Items.Count, firstRecordTest.Items.Count);
            for (int record = 0; record < firstRecord.Items.Count; record++)
            {
                CalendarItem validItem = firstRecord.Items[record];
                CalendarItem testItem = firstRecordTest.Items[record];
                Assert.Equal(validItem.DurationInMinutes, testItem.DurationInMinutes);
                Assert.Equal(validItem.CategoryID, testItem.CategoryID);
                Assert.Equal(validItem.EventID, testItem.EventID);

            }
        }

        // ========================================================================

        [Fact]
        public void HomeCalendarMethod_GetCalendarItemsByMonth_NoStartEnd_FilterbyCategory()
        {
            // Arrange
            string inFile = TestConstants2.GetSolutionDir() + "\\" + testInputFile;
            HomeCalendar homeCalendar = new HomeCalendar(inFile);
            int maxRecords = TestConstants2.CalendarItemsByMonth_FilteredByCat9_number;
            CalendarItemsByMonth firstRecord = TestConstants2.CalendarItemsByMonth_FirstRecord_FilteredCat9;

            // Act
            List<CalendarItemsByMonth> CalendarItemsByMonth = homeCalendar.GetCalendarItemsByMonth(null, null, true, 9);
            CalendarItemsByMonth firstRecordTest = CalendarItemsByMonth[0];

            // Assert
            Assert.Equal(maxRecords, CalendarItemsByMonth.Count);

            // verify 1st record
            Assert.Equal(firstRecord.Month, firstRecordTest.Month);
            Assert.Equal(firstRecord.TotalBusyTime, firstRecordTest.TotalBusyTime);
            Assert.Equal(firstRecord.Items.Count, firstRecordTest.Items.Count);
            for (int record = 0; record < firstRecord.Items.Count; record++)
            {
                CalendarItem validItem = firstRecord.Items[record];
                CalendarItem testItem = firstRecordTest.Items[record];
                Assert.Equal(validItem.DurationInMinutes, testItem.DurationInMinutes);
                Assert.Equal(validItem.CategoryID, testItem.CategoryID);
                Assert.Equal(validItem.EventID, testItem.EventID);

            }
        }
        // ========================================================================

        [Fact]
        public void HomeCalendarMethod_GetCalendarItemsByMonth_2020_filterDateAndCat9()
        {
            // Arrange
            string inFile = TestConstants2.GetSolutionDir() + "\\" + testInputFile;
            HomeCalendar homeCalendar = new HomeCalendar(inFile);
            List<Category> listCategories = homeCalendar.categories.List();
            List<CalendarItemsByMonth> validCalendarItemsByMonth = TestConstants2.getCalendarItemsBy2020_01_filteredByCat9();
            CalendarItemsByMonth firstRecord = TestConstants2.CalendarItemsByMonth_FirstRecord_FilteredCat9;

            // Act
            List<CalendarItemsByMonth> CalendarItemsByMonth = homeCalendar.GetCalendarItemsByMonth(new DateTime(2020, 1, 1), new DateTime(2020, 12, 31), true, 9);
            CalendarItemsByMonth firstRecordTest = CalendarItemsByMonth[0];

            // Assert
            Assert.Equal(validCalendarItemsByMonth.Count, CalendarItemsByMonth.Count);

            // verify 1st record
            Assert.Equal(firstRecord.Month, firstRecordTest.Month);
            Assert.Equal(firstRecord.TotalBusyTime, firstRecordTest.TotalBusyTime);
            Assert.Equal(firstRecord.Items.Count, firstRecordTest.Items.Count);
            for (int record = 0; record < firstRecord.Items.Count; record++)
            {
                CalendarItem validItem = firstRecord.Items[record];
                CalendarItem testItem = firstRecordTest.Items[record];
                Assert.Equal(validItem.DurationInMinutes, testItem.DurationInMinutes);
                Assert.Equal(validItem.CategoryID, testItem.CategoryID);
                Assert.Equal(validItem.EventID, testItem.EventID);

            }
        }


        // ========================================================================

        [Fact]
        public void HomeCalendarMethod_GetCalendarItemsByMonth_2018_filterDate()
        {
            // Arrange
            string inFile = TestConstants2.GetSolutionDir() + "\\" + testInputFile;
            HomeCalendar homeCalendar = new HomeCalendar(inFile);
            List<CalendarItemsByMonth> validCalendarItemsByMonth = TestConstants2.getCalendarItemsBy2018_01();
            CalendarItemsByMonth firstRecord = validCalendarItemsByMonth[0];


            // Act
            List<CalendarItemsByMonth> CalendarItemsByMonth = homeCalendar.GetCalendarItemsByMonth(new DateTime(2018, 1, 1), new DateTime(2018, 12, 31), false, 9);
            CalendarItemsByMonth firstRecordTest = CalendarItemsByMonth[0];

            // Assert
            Assert.Equal(validCalendarItemsByMonth.Count, CalendarItemsByMonth.Count);

            // verify 1st record
            Assert.Equal(firstRecord.Month, firstRecordTest.Month);
            Assert.Equal(firstRecord.TotalBusyTime, firstRecordTest.TotalBusyTime);
            Assert.Equal(firstRecord.Items.Count, firstRecordTest.Items.Count);
            for (int record = 0; record < firstRecord.Items.Count; record++)
            {
                CalendarItem validItem = firstRecord.Items[record];
                CalendarItem testItem = firstRecordTest.Items[record];
                Assert.Equal(validItem.DurationInMinutes, testItem.DurationInMinutes);
                Assert.Equal(validItem.CategoryID, testItem.CategoryID);
                Assert.Equal(validItem.EventID, testItem.EventID);

            }
        }
    }
}

