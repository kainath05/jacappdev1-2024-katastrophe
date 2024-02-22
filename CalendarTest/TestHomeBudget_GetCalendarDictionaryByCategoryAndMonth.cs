﻿using System;
using Xunit;
using System.IO;
using System.Collections.Generic;
using Calendar;
using System.Dynamic;

namespace CalendarCodeTests
{
    public class TestHomeCalendar_GetCalendarDictionaryByCategoryAndMonth
    {
        string testInputFile = TestConstants2.testCalendarFile;



        // ========================================================================
        // Get Events By Month Method tests
        // ========================================================================

        [Fact]
        public void HomeCalendarMethod_GetCalendarDictionaryByCategoryAndMonth_NoStartEnd_NoFilter_VerifyNumberOfRecords()
        {
            // Arrange
            string inFile = TestConstants2.GetSolutionDir() + "\\" + testInputFile;
            HomeCalendar homeCalendar = new HomeCalendar(inFile);
            int maxRecords = TestConstants2.CalendarItemsByCategoryAndMonth_MaxRecords;
            Dictionary<string, object> firstRecord = TestConstants2.getCalendarItemsByCategoryAndMonthFirstRecord();

            // Act
            List<Dictionary<string, object>> CalendarItemsByCategoryAndMonth = homeCalendar.GetCalendarDictionaryByCategoryAndMonth(null, null, false, 9);

            // Assert
            Assert.Equal(maxRecords+1,CalendarItemsByCategoryAndMonth.Count);

        }

        // ========================================================================

        [Fact]
        public void HomeCalendarMethod_GetCalendarDictionaryByCategoryAndMonth_NoStartEnd_NoFilter_VerifyFirstRecord()
        {
            // Arrange
            string inFile = TestConstants2.GetSolutionDir() + "\\" + testInputFile;
            HomeCalendar homeCalendar = new HomeCalendar(inFile);
            Dictionary<string,object> firstRecord = TestConstants2.getCalendarItemsByCategoryAndMonthFirstRecord();

            // Act
            List<Dictionary<string,object>> CalendarItemsByCategoryAndMonth = homeCalendar.GetCalendarDictionaryByCategoryAndMonth(null, null, false, 9);
            Dictionary<string,object> firstRecordTest = CalendarItemsByCategoryAndMonth[0];

            // Assert
            Assert.True(AssertDictionaryForEventByCategoryAndMonthIsOK(firstRecord,firstRecordTest));
            
        }

        // ========================================================================

        [Fact]
        public void HomeCalendarMethod_GetCalendarDictionaryByCategoryAndMonth_NoStartEnd_NoFilter_VerifyTotalsRecord()
        {
            // Arrange
            string inFile = TestConstants2.GetSolutionDir() + "\\" + testInputFile;
            HomeCalendar homeCalendar = new HomeCalendar(inFile);
            Dictionary<string, object> totalsRecord = TestConstants2.getCalendarItemsByCategoryAndMonthTotalsRecord();

            // Act
            List<Dictionary<string, object>> CalendarItemsByCategoryAndMonth = homeCalendar.GetCalendarDictionaryByCategoryAndMonth(null, null, false, 9);
            Dictionary<string, object> totalsRecordTest = CalendarItemsByCategoryAndMonth[CalendarItemsByCategoryAndMonth.Count - 1];

            // Assert
            // ... loop over all key/value pairs 
            Assert.True(AssertDictionaryForEventByCategoryAndMonthIsOK(totalsRecord, totalsRecordTest), "Totals Record is Valid");

        }

        // ========================================================================

        [Fact]
        public void HomeCalendarMethod_GetCalendarDictionaryByCategoryAndMonth_NoStartEnd_FilterbyCategory()
        {
            // Arrange
            string inFile = TestConstants2.GetSolutionDir() + "\\" + testInputFile;
            HomeCalendar homeCalendar = new HomeCalendar(inFile);
            List<Dictionary<string, object>> expectedResults =TestConstants2.getCalendarItemsByCategoryAndMonthCat2();

            // Act
            List<Dictionary<string, object>> gotResults = homeCalendar.GetCalendarDictionaryByCategoryAndMonth(null, null, true, 2);

            // Assert
            Assert.Equal(expectedResults.Count, gotResults.Count);
            for (int record = 0; record < expectedResults.Count; record++)
            {
                Assert.True(AssertDictionaryForEventByCategoryAndMonthIsOK(expectedResults[record],
                    gotResults[record]), "Record:" + record + " is Valid");

            }
        }

        // ========================================================================

        [Fact]
        public void HomeCalendarMethod_GetCalendarDictionaryByCategoryAndMonth_2020()
        {
            // Arrange
            string inFile = TestConstants2.GetSolutionDir() + "\\" + testInputFile;
            HomeCalendar homeCalendar = new HomeCalendar(inFile);
            List<Dictionary<string, object>> expectedResults = TestConstants2.getCalendarItemsByCategoryAndMonth2020();

            // Act
            List<Dictionary<string, object>> gotResults = homeCalendar.GetCalendarDictionaryByCategoryAndMonth(new DateTime(2020,1,1), new DateTime(2020,12,31), false, 10);

            // Assert
            Assert.Equal(expectedResults.Count, gotResults.Count);
            for (int record = 0; record < expectedResults.Count; record++)
            {
                Assert.True(AssertDictionaryForEventByCategoryAndMonthIsOK(expectedResults[record],
                    gotResults[record]), "Record:" + record + " is Valid");

            }
        }




        // ========================================================================

        // -------------------------------------------------------
        // helpful functions, ... they are not tests
        // -------------------------------------------------------

    
        Boolean AssertDictionaryForEventByCategoryAndMonthIsOK(Dictionary<string,object> recordExpeted, Dictionary<string,object> recordGot)
        {
            try
            {
                foreach (var kvp in recordExpeted)
                {
                    String key = kvp.Key as String;
                    Object recordExpectedValue = kvp.Value;
                    Object recordGotValue = recordGot[key];


                    // ... validate the Calendar items
                    if (recordExpectedValue != null && recordExpectedValue.GetType() == typeof(List<CalendarItem>))
                    {
                        List<CalendarItem> expectedItems = recordExpectedValue as List<CalendarItem>;
                        List<CalendarItem> gotItems = recordGotValue as List<CalendarItem>;
                        for (int CalendarItemNumber = 0; CalendarItemNumber < expectedItems.Count; CalendarItemNumber++)
                        {
                            Assert.Equal(expectedItems[CalendarItemNumber].DurationInMinutes, gotItems[CalendarItemNumber].DurationInMinutes);
                            Assert.Equal(expectedItems[CalendarItemNumber].CategoryID, gotItems[CalendarItemNumber].CategoryID);
                            Assert.Equal(expectedItems[CalendarItemNumber].EventID, gotItems[CalendarItemNumber].EventID);
                        }
                    }

                    // else ... validate the value for the specified key
                    else
                    {
                        Assert.Equal(recordExpectedValue, recordGotValue);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

