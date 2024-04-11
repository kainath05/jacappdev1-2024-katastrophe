﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;

namespace Calendar
{
    interface View
    {
        string ShowFilePicker(string initialDirectory);

        void ShowMessage(string message);

        bool ConfirmCloseApplication();

        //void DisplayCategories(List<string> categories);

        //CalendarItem EnterEventData(List<string> categories);

        //void ShowEventAddedMessage();
    }
}