using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Calendar
{
    public interface IAddEvent
    {
        void ShowMessage(string message, string title, MessageBoxButton button, MessageBoxImage image);
        void UpdateComboBoxes(List<Category> categories);

    }
}
