using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Calendar;
using System.IO;

 
namespace Calendar
{
    public class Presenter
    {
        private readonly View _view;
        public HomeCalendar _calendar;
        public string fileName = "newdb.db";
        public bool newDB;

        public Presenter(View view)
        {
            _view = view;
            //if (_calendar != null) return;
            //InitializeCalendar();
        }
        public void InitializeCalendar()
        {
            // Check for _calendar reinitialization logic if necessary
            _calendar = newDB ? new HomeCalendar(fileName, true) : new HomeCalendar(fileName);
        }

        public bool ConfirmApplicationClosure()
        {
            return _view.ConfirmCloseApplication();
        }

        public List<Category.CategoryType> DisplayTypes()
        {
            List<Category.CategoryType> types = new List<Category.CategoryType>();
            foreach (Category.CategoryType item in Enum.GetValues(typeof(Category.CategoryType)))
            {
                types.Add(item);
            }
            return types;
        }

        public void AddCategory(string descr, Category.CategoryType type)
        {
            var list = _calendar.categories.List();
            foreach (Category category in list)
            {
                if (descr == category.Description)
                {

                    _view.ShowMessage("Category already exists.");
                    return; 
                }
            }
            if (!Enum.IsDefined(typeof(Category.CategoryType), type))
            {
                _view.ShowMessage("Invalid category type.");
                return; 
            }
            _calendar.categories.Add(descr, type);
            _view.ShowMessage("Category added.");
        }
    }
}
