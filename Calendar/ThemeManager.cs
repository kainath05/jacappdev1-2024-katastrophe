using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Calendar
{
    public static class ThemeManager
    {
        public static event EventHandler ThemeChanged;

        private static bool _isDarkTheme;
        public static bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (_isDarkTheme != value)
                {
                    _isDarkTheme = value;
                    ThemeChanged?.Invoke(null, EventArgs.Empty); // This triggers the change on all subscribed windows
                }
            }
        }
    }
}
