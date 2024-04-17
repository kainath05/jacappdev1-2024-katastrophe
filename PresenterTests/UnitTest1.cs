using Calendar;
namespace PresenterTests
{
    public class TestView : View
    {
        public bool confirmCloseApplicationCalled = false;
        public bool showMessageCalled = false;
        public List<string> messages = new List<string>();

        public bool ConfirmCloseApplication()
        {
            confirmCloseApplicationCalled = true;
            return true;
        }

        public void ShowMessage(string message)
        {
            showMessageCalled = true;
            messages.Add(message);
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
            presenter.newDB = true; // Assuming we are testing with a new database
            presenter.InitializeCalendar();
            Assert.NotNull(presenter._calendar); // Assuming there's a public getter for _calendar or it's internally checkable
        }

        [Fact]
        public void ConfirmApplicationClosure_CallsViewConfirm()
        {
            var view = new TestView();
            var presenter = new Presenter(view);
            var result = presenter.ConfirmApplicationClosure();

            Assert.True(view.confirmCloseApplicationCalled);
            Assert.True(result); // Check if the return value matches the expected behavior
        }
        [Fact]
        public void DisplayTypes_ReturnsAllCategoryTypes()
        {
            var view = new TestView();
            var presenter = new Presenter(view);
            var types = presenter.DisplayTypes();

            Assert.NotNull(types);
            Assert.Equal(Enum.GetValues(typeof(Category.CategoryType)).Length, types.Count); // Checks if all types are included
        }
        [Fact]
        public void AddCategory_AddsCategory_ShowMessageCalled()
        {
            var view = new TestView();
            var presenter = new Presenter(view);
            presenter.InitializeCalendar(); // Ensure calendar is initialized
            presenter.AddCategory("Meeting", Category.CategoryType.Event);

            Assert.Contains("Category added.", view.messages); // Assuming this is the message on successful addition
            Assert.True(view.showMessageCalled);
        }


    }
}