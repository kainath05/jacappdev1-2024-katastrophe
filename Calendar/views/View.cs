using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Calendar;

namespace Calendar
{
    public interface View
    {
        void ShowMessage(string message);
        bool ConfirmCloseApplication();
        void DisplayDatabaseFile();
    }
}
