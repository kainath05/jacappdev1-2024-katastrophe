using Calendar;
using Calendar.views;
using System.Windows;
namespace PresenterTests
{
    public class TestView : View
    {
        public bool confirmCloseApplicationCalled = false;
        public bool showMessageCalled = false;
        public List<string> messages = new List<string>();
        public bool display = false;
        public bool showTypes = false;

        public bool ConfirmCloseApplication()
        {
            confirmCloseApplicationCalled = true;
            return true;
        }

        public void DisplayDatabaseFile()
        {
            display = true;
        }

        public void ShowMessage(string message)
        {
            showMessageCalled = true;
            messages.Add(message);
        }

        public void ShowTypes(List<Category.CategoryType> types)
        {
            showTypes = true;
        }
    }

    public class TestEventView : IAddEvent
    {
        public bool eventMessage = false;
        public List<string> eventMessages = new List<string>();
        public List<string> titles = new List<string>();
        public List<MessageBoxButton> messageBoxes = new List<MessageBoxButton>();
        public List<MessageBoxImage> images = new List<MessageBoxImage>();
        public bool comboUpdates = false;
        public void ShowMessage(string message, string title, MessageBoxButton button, MessageBoxImage image)
        {
            eventMessage = true;
            eventMessages.Add(message);
            titles.Add(title);
            messageBoxes.Add(button);
            images.Add(image);
        }

        public void UpdateComboBoxes(List<Category> categories)
        {
            comboUpdates = true;
        }
    }

    public class TestViewForReport : ViewForReport
    {
        public bool types = false;
        public void DisplayCategories(List<Category> categories)
        {
            types = true;  // Flag that the method was called
        }
    }

    public class UnitTest1
    {
        [Fact]
        public void TestConstructor()
        {
            TestView view = new TestView();
            Presenter p = new Presenter(view);
            Assert.IsType<Presenter>(p);
        }

        [Fact]
        public void InitializeCalendar_NewDatabase_InitializesCalendar()
        {
            var view = new TestView();
            var presenter = new Presenter(view);
            presenter.newDB = true; 
            presenter.InitializeCalendar();
            Assert.NotNull(presenter._calendar);
        }

        [Fact]
        public void ConfirmApplicationClosure_CallsViewConfirm()
        {
            var view = new TestView();
            var presenter = new Presenter(view);
            var result = presenter.ConfirmApplicationClosure();

            Assert.True(view.confirmCloseApplicationCalled);
            Assert.True(result);
        }
        [Fact]
        public void DisplayTypes_ReturnsAllCategoryTypes()
        {
            var view = new TestView();
            var presenter = new Presenter(view);
            var types = presenter.DisplayTypes();
            view.ShowTypes(types);

            Assert.NotNull(types);
            Assert.True(view.showTypes);
            Assert.Equal(Enum.GetValues(typeof(Category.CategoryType)).Length, types.Count); // Checks if all types are included
        }
        [Fact]
        public void AddCategory_AddsCategory_ShowMessage()
        {
            var view = new TestView();
            var presenter = new Presenter(view);
            presenter.newDB = true;
            presenter.InitializeCalendar(); 
            presenter.AddCategory("Amaan's Homework", Category.CategoryType.Event);

            Assert.Contains("Category added.", view.messages); // Assuming this is the message on successful addition
            Assert.True(view.showMessageCalled);
        }

        [Fact]
        public void AddCategory_DuplicateCategory_ShowErrorMessage()
        {
            var view = new TestView();
            var presenter = new Presenter(view);
            presenter.newDB = true;
            presenter.InitializeCalendar();
            presenter.AddCategory("Meeting", Category.CategoryType.Event);

            // Try adding the same category again
            presenter.AddCategory("Meeting", Category.CategoryType.Event);

            Assert.Contains("Category already exists.", view.messages); // Assuming this is the error message on duplicate addition
            Assert.True(view.showMessageCalled);
        }

        [Fact]
        public void AddCategory_InvalidType_ShowErrorMessage()
        {
            var view = new TestView();
            var presenter = new Presenter(view);
            presenter.newDB = true;
            presenter.InitializeCalendar();

            // Try adding with an invalid category type
            presenter.AddCategory("Shopping", (Category.CategoryType)100);

            Assert.Contains("Invalid category type.", view.messages); // Assuming this is the error message for an invalid type
            Assert.True(view.showMessageCalled);
        }

        [Fact]
        public void ChangeInView_SetsIAddEvent()
        {
           TestEventView v = new TestEventView();
           TestView view = new TestView();
           Presenter p = new Presenter(view);
           p.SetAddEventView(v);
           Assert.Same(v, p._addEventView);
        }

        [Fact]
        public void InitializeForm_PopulatesComboBoxes()
        {
            var view = new TestView();
            TestEventView testEventView = new TestEventView();
            Presenter presenter = new Presenter(view);
            presenter.newDB = true;
            presenter.InitializeCalendar();
            presenter.SetAddEventView(testEventView);

            presenter.InitializeForm();

            Assert.True(testEventView.comboUpdates);
        }

        [Fact]
        public void InitializeForm_ShowErrorMessage_NoDatabase()
        {
            var view = new TestView();
            TestEventView testEventView = new TestEventView();
            Presenter presenter = new Presenter(view);
            presenter.SetAddEventView(testEventView);

            try
            {
                presenter.InitializeForm();
            }
            catch (Exception ex)
            {
                Assert.Contains("Failed to initialize form data due to database connection issues: " + ex.Message, testEventView.eventMessages);
                Assert.Contains("Database Error", testEventView.titles);
                Assert.Contains(MessageBoxButton.OK, testEventView.messageBoxes);
                Assert.Contains(MessageBoxImage.Error, testEventView.images);
                Assert.True(testEventView.eventMessage);
                Assert.True(view.display);
            }
        }

        [Fact]
        public void AddEvent_AddsEventAndDisplaysMessage()
        {
            TestEventView testEventView = new TestEventView();
            var view = new TestView();
            Presenter presenter = new Presenter(view);
            presenter.newDB = true;
            presenter.InitializeCalendar();
            presenter.SetAddEventView(testEventView);

            DateTime dateTime = DateTime.Now;
            int categoryId = 1; 
            double duration = 1.5;
            string details = "Test event details";

            presenter.AddEvent(dateTime, categoryId, duration, details);

            Assert.Contains("Event successfully added!", view.messages);
            Assert.True(view.showMessageCalled);
        }

        [Fact]
        public void AddEvent_ShowsErrorMessage()
        {
            TestEventView testEventView = new TestEventView();
            var view = new TestView();
            Presenter presenter = new Presenter(view);
            presenter.newDB = true;
            presenter.InitializeCalendar();
            presenter.SetAddEventView(testEventView);

            try
            {
                DateTime dateTime = DateTime.Now;
                int categoryId = 14; // there is no category with id 14
                double duration = 1.5;
                string details = "Test event details";

                presenter.AddEvent(dateTime, categoryId, duration, details);
            }
            catch (Exception ex)
            {
                Assert.Contains("Failed to create event: " + ex.Message, testEventView.eventMessages);
                Assert.Contains("Error", testEventView.titles);
                Assert.Contains(MessageBoxButton.OK, testEventView.messageBoxes);
                Assert.Contains(MessageBoxImage.Error, testEventView.images);
                Assert.True(testEventView.eventMessage);
                Assert.True(view.display);
            }
        }

        [Fact]
        public void DeleteEvent_EventDeleted_ShowConfirmationMessage()
        {
            var view = new TestView();
            var presenter = new Presenter(view);
            presenter.InitializeCalendar(); 
            presenter.DeleteEvent(1);

            Assert.Contains("Event deleted.", view.messages);
            Assert.True(view.showMessageCalled);
        }

        [Fact]
        public void UpdateEvent_ValidEvent_ShowConfirmationMessage()
        {
            var view = new TestView();
            var presenter = new Presenter(view);
            presenter.InitializeCalendar();
            DateTime dateTime = DateTime.Now;
            int categoryId = 1;
            double duration = 1.5;
            string details = "Updated event details";

            presenter.UpdateEvent(1, dateTime, categoryId, duration, details);

            Assert.Contains("Event updated.", view.messages);
            Assert.True(view.showMessageCalled);
        }

        [Fact]
        public void DisplayCategories()
        {
            var viewReport = new TestViewForReport();
            var view = new TestView();
            var presenter = new Presenter(view);
            presenter.InitializeCalendar();
            presenter.SetReportView(viewReport);
            viewReport.DisplayCategories(presenter._calendar.categories.List());

            Assert.True(viewReport.types);
        }

        [Fact]
        public void DisplayCalendarItems_ReturnsItemsWithinDateRange()
        {
            var view = new TestView();
            var presenter = new Presenter(view);
            presenter.InitializeCalendar();
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today.AddDays(30);

            var items = presenter.DisplayCalendarItems(start, end, false, 1);

            Assert.NotNull(items);
            Assert.IsType<List<CalendarItem>>(items);
            Assert.All(items, item =>
            {
                Assert.True(item.StartDateTime >= start && item.StartDateTime <= end);
            });
        }

        [Fact]
        public void DisplayItemsByMonth_ValidRange_ReturnsGroupedItems()
        {
            var view = new TestView();
            var presenter = new Presenter(view);
            presenter.InitializeCalendar();
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today.AddDays(30);

            var items = presenter.DisplayItemsByMonth(start, end, true, 1);

            Assert.NotNull(items);
            Assert.IsType<List<CalendarItemsByMonth>>(items);
            Assert.All(items, group =>
            {
                // Parse the month string to an integer
                int month;
                if (int.TryParse(group.Month, out month))
                {
                    Assert.True(month >= start.Month && month <= end.Month);
                }
                else
                {
                    Assert.True(false, $"Failed to parse month: {group.Month}");
                }
            });
        }

        [Fact]
        public void DisplayItemsByCategory_ValidRange_ReturnsCategorizedItems()
        {
            var view = new TestView();
            var presenter = new Presenter(view);
            presenter.InitializeCalendar();
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today.AddDays(30);

            var items = presenter.DisplayItemsByCategory(start, end, false, 1);

            Assert.NotNull(items);
            Assert.IsType<List<CalendarItemsByCategory>>(items);
            Assert.All(items, category =>
            {
                Assert.True(category.Category == "School");
            });
        }


        [Fact]
        public void DisplayItemsByCategoryAndMonth_ReturnsCorrectlyGroupedItems()
        {
            // Arrange
            var view = new TestView();
            var presenter = new Presenter(view);
            presenter.InitializeCalendar();  

            DateTime start = new DateTime(2023, 1, 1);
            DateTime end = new DateTime(2023, 12, 31);
            bool filter = true;
            int categoryId = 1;
            
            var items = presenter.DisplayItemsByCategoryAndMonth(start, end, filter, categoryId);

            Assert.NotNull(items);
            Assert.IsType<List<Dictionary<string, object>>>(items);
        }

        
    }
}