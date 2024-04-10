using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Calendar;


namespace Calendar
{
    class Presenter //add view
    {
        private readonly View _view;
        private readonly HomeCalendar _model;

        public Presenter(View view)
        {
            _view = view;
            _model = new HomeCalendar("newdb.db", true);
        }
    }
}
