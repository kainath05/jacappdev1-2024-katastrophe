using Calendar;
namespace PresenterTests
{
    public class TestView : View
    {
        public bool messageWorks;
        
        public bool ConfirmCloseApplication()
        {
            return true;
        }

        public void ShowMessage(string message)
        {
            messageWorks = true;
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
        public void TestDatabase()
        {
            TestView view = new TestView();
            Presenter p = new Presenter(view);
        }
    }
}