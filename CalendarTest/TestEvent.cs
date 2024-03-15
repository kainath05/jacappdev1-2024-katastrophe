using System;
using Xunit;
using Calendar;

namespace CalendarCodeTests
{
    public class TestEvent
    {
        // ========================================================================

        [Fact]
        public void EventObject_New()
        {

            // Arrange
            DateTime now = DateTime.Now;
            double DurationInMinutes = 24.55;
            string descr = "New Sweater";
            int category = 34;
            int id = 42;

            // Act
            Event Event = new Event(id, now, category, DurationInMinutes, descr);

            // Assert 
            Assert.IsType<Event>(Event);

            Assert.Equal(id, Event.Id);
            Assert.Equal(DurationInMinutes, Event.DurationInMinutes);
            Assert.Equal(descr, Event.Details);
            Assert.Equal(category, Event.Category);
            Assert.Equal(now, Event.StartDateTime);
        }

        // ========================================================================

        [Fact]
        public void EventCopyConstructoryIsDeepCopy()
        {

            // Arrange
            DateTime now = DateTime.Now;
            double DurationInMinutes = 24.55;
            string descr = "New Sweater";
            int category = 34;
            int id = 42;
            Event Event = new Event(id, now, category, DurationInMinutes, descr);

            // Act
            Event copy = new Event(Event);

            // Assert 
            Assert.Equal(id, Event.Id);
            Assert.NotEqual(DurationInMinutes + 15, copy.DurationInMinutes);
            Assert.Equal(Event.DurationInMinutes, copy.DurationInMinutes);
            Assert.Equal(descr, Event.Details);
            Assert.Equal(category, Event.Category);
            Assert.Equal(now, Event.StartDateTime);
        }

        [Fact]
        public void EventObject_PropertiesAreReadOnly() //New test
        {
            // Arrange
            DateTime now = DateTime.Now;
            double mins = 24.50;
            string descr = "Project Coordination";
            int id = 42;
            int category = 1;

            // Act
            Event eve = new Event(id, now, category, mins, descr);

            // Assert 
            Assert.IsType<Event>(eve);
            Assert.True(typeof(Event).GetProperty("Id").CanWrite == false);
            Assert.True(typeof(Event).GetProperty("StartDateTime").CanWrite == false);
            Assert.True(typeof(Event).GetProperty("Category").CanWrite == false);
            Assert.True(typeof(Event).GetProperty("Details").CanWrite == false);
            Assert.True(typeof(Event).GetProperty("DurationInMinutes").CanWrite == false);
        }


        // ========================================================================


    }
}
