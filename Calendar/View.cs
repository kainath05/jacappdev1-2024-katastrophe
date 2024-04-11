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
        // Display a file picker dialog and return the selected file path
        string ShowFilePicker(string initialDirectory);

        // Display a folder picker dialog and return the selected folder path
        string ShowFolderPicker(string defaultFolder);

        // Display a message to the user
        void ShowMessage(string message);

        // Ask the user to confirm closing the application and return the user's decision
        bool ConfirmCloseApplication();

        // Display available categories to choose from
        void DisplayCategories(List<string> categories);

        // Display a form to enter event details and return the entered details
        CalendarItem EnterEventData(List<string> categories);

        // Notify the user of successful event addition
        void ShowEventAddedMessage();
    }
}
