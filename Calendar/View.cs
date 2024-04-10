using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;

namespace Calendar
{
    interface View
    {
        void CreateNewCategory();

        void ChoosingCalendar(); //file explorer, new

        void DisplayData(List<CalendarItem> calendarItems);

        void SortByMonth(List<CalendarItemsByMonth> calendarItemsByMonth);

        void SortByCategory(List<CalendarItemsByCategory> calendarItemsByCategory);


    }
}
