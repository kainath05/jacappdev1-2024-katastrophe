using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.views
{
    public interface ViewForReport
    {
        void DisplayCategories(List<Category> categories);
    }
}
