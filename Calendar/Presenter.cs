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
    class Presenter
    {
        private readonly View _view;
        public HomeCalendar _calendar;
        public string fileName = "newdb.db";
        public bool newDB;

        public Presenter(View view)
        {
            _view = view;
            if (_calendar != null) return;

            if (newDB)
            {
                _calendar = new HomeCalendar(fileName, true);
            }
            else
            {
                _calendar = new HomeCalendar(fileName);
            }
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
    }
}
