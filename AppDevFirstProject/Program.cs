namespace Calendar
{
    internal class Program
    {

        static void Main(string[] args)
        {
            //HomeCalendar calendar = new HomeCalendar();
//            calendar.ReadFromFile("./test.calendar");

//            Console.WriteLine(@"
//1. Get Calendar Items Variations
//2. Get Calendar Items By Month Variations
//3. Get Calendar Items By Category Variations
//4. Get Calendar Dictionary By Month And Category Variations");
//            bool validInput = int.TryParse(Console.ReadLine(), out int input);
//            if (validInput && (input <= 4 && input >= 1))
//            {
//                switch (input)
//                {
//                    case 1:
//                        GetCalendarItemsVariations(calendar);
//                        break;
//                    case 2:

//                        GetCalendarItemsByMonthVariations(calendar);
//                        break;
//                    case 3:
//                        GetCalendarItemsByCategoryVariations(calendar);
//                        break;
//                    case 4:
//                        GetCalendarDictionaryByMonthAndCategoryVariations(calendar);
//                        break;

//                }
//            }
//            else
//            {
//                Console.WriteLine("Must enter a number between 1 and 3");
//            }




        }


//        #region GetCalendarItems
//        public static void GetCalendarItemsVariations(HomeCalendar calendar)
//        {

//            Console.WriteLine(@"
//1. Get Calendar Items
//2. Get Calendar Items With Start and End Date
//3. Get Calendar Items With All Values");

//            bool validInput = false;
//            while (validInput == false)
//            {

//                validInput = int.TryParse(Console.ReadLine(), out int input);
//                if (validInput && (input <= 3 && input >= 1))
//                {
//                    switch (input)
//                    {
//                        case 1:
//                            GetCalendarItems(calendar);
//                            break;
//                        case 2:
//                            GetCalendarItemsWithStartAndEndDate(calendar);
//                            break;
//                        case 3:
//                            GetCalendarItemsWithAllValues(calendar);
//                            break;
//                    }
//                }
//                else
//                {
//                    Console.WriteLine("Must enter a number between 1 and 3");
//                    validInput = false;
//                }

//            }
//        }

//        public static void GetCalendarItems(HomeCalendar calendar)
//        {
//            List<CalendarItem> calendarItems = calendar.GetCalendarItems(null, null, false, 0);

//            Console.WriteLine("Date               Short Description         Duration     BusyTime");
//            foreach (CalendarItem item in calendarItems)
//            {
//                Console.WriteLine(
//                String.Format("{0} {1,-20}  {2,8} {3,12}",
//                    item.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
//                    item.ShortDescription,
//                    item.DurationInMinutes, item.BusyTime)
//              );

//            }
//        }

//        public static void GetCalendarItemsWithStartAndEndDate(HomeCalendar calendar)
//        {
//            List<CalendarItem> calendarItems = calendar.GetCalendarItems(DateTime.Now.AddYears(-5), DateTime.Now, false, 0);

//            Console.WriteLine("Date               Short Description         Duration     BusyTime");
//            foreach (CalendarItem item in calendarItems)
//            {
//                Console.WriteLine(
//                String.Format("{0} {1,-20}  {2,8} {3,12}",
//                    item.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
//                    item.ShortDescription,
//                    item.DurationInMinutes, item.BusyTime)
//              );

//            }
//        }
//        public static void GetCalendarItemsWithAllValues(HomeCalendar calendar)
//        {
//            List<CalendarItem> calendarItems = calendar.GetCalendarItems(DateTime.Now.AddYears(-5), DateTime.Now, true, 8);

//            Console.WriteLine("Date               Short Description         Duration     BusyTime");
//            foreach (CalendarItem item in calendarItems)
//            {
//                Console.WriteLine(
//                String.Format("{0} {1,-20}  {2,8} {3,12}",
//                    item.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
//                    item.ShortDescription,
//                    item.DurationInMinutes, item.BusyTime)
//              );

//            }
//        }
//        #endregion

//        #region GetCalendarItemsbyMonth
//        public static void GetCalendarItemsByMonthVariations(HomeCalendar calendar)
//        {
//            Console.WriteLine(@"
//1. Get Calendar Items by Month
//2. Get Calendar Items by Month With Start and End Date 
//3. Get Calendar Items by Month With All Values");

//            bool validInput = false;
//            while (validInput == false)
//            {

//                validInput = int.TryParse(Console.ReadLine(), out int input);
//                if (validInput && (input <= 3 && input >= 1))
//                {
//                    switch (input)
//                    {
//                        case 1:
//                            GetCalendarItemsByMonth(calendar);
//                            break;
//                        case 2:
//                            GetCalendarItemsByMonthWithStartAndEndDate(calendar);
//                            break;
//                        case 3:
//                            GetCalendarItemsByMonthWithAllValues(calendar);
//                            break;
//                    }
//                }
//                else
//                {
//                    Console.WriteLine("Must enter a number between 1 and 3");
//                    validInput = false;
//                }

//            }
//        }
//        public static void GetCalendarItemsByMonth(HomeCalendar calendar)
//        {

//            List<CalendarItemsByMonth> calendarItemsByMonths = calendar.GetCalendarItemsByMonth(null, null, false, 0);

//            Console.WriteLine("Date               Short Description         Duration     BusyTime");
//            foreach (CalendarItemsByMonth calendarItem in calendarItemsByMonths)
//            {
//                Console.WriteLine("Month:" + calendarItem.Month);

//                foreach (CalendarItem item in calendarItem.Items)
//                {
//                    Console.WriteLine(
//                                   String.Format("{0} {1,-20}  {2,8} {3,12}",
//                                       item.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
//                                       item.ShortDescription,
//                                       item.DurationInMinutes, item.BusyTime)
//                                 );
//                }
//            }
//        }

//        public static void GetCalendarItemsByMonthWithStartAndEndDate(HomeCalendar calendar)
//        {
//            List<CalendarItemsByMonth> calendarItemsByMonths = calendar.GetCalendarItemsByMonth(DateTime.Now.AddYears(-5), DateTime.Now, false, 0);

//            Console.WriteLine("Date               Short Description         Duration     BusyTime");
//            foreach (CalendarItemsByMonth calendarItem in calendarItemsByMonths)
//            {
//                Console.WriteLine("Month:" + calendarItem.Month);

//                foreach (CalendarItem item in calendarItem.Items)
//                {
//                    Console.WriteLine(
//                                   String.Format("{0} {1,-20}  {2,8} {3,12}",
//                                       item.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
//                                       item.ShortDescription,
//                                       item.DurationInMinutes, item.BusyTime)
//                                  );
//                }
//            }
//        }

//        public static void GetCalendarItemsByMonthWithAllValues(HomeCalendar calendar)
//        {
//            List<CalendarItemsByMonth> calendarItemsByMonths = calendar.GetCalendarItemsByMonth(DateTime.Now.AddYears(-5), DateTime.Now, true, 8);

//            Console.WriteLine("Date               Short Description         Duration     BusyTime");
//            foreach (CalendarItemsByMonth calendarItem in calendarItemsByMonths)
//            {
//                Console.WriteLine("Month:" + calendarItem.Month);

//                foreach (CalendarItem item in calendarItem.Items)
//                {
//                    Console.WriteLine(
//                                   String.Format("{0} {1,-20}  {2,8} {3,12}",
//                                       item.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
//                                       item.ShortDescription,
//                                       item.DurationInMinutes, item.BusyTime)
//                              );
//                }
//            }
//        }

//        #endregion

//        #region GetCalendarItemsByCategory
//        public static void GetCalendarItemsByCategoryVariations(HomeCalendar calendar)
//        {
//            Console.WriteLine(@"
//1. Get Calendar Items by Category
//2. Get Calendar Items by Category With Start and End Date 
//3. Get Calendar Items by Category With All Values");

//            bool validInput = false;
//            while (validInput == false)
//            {

//                validInput = int.TryParse(Console.ReadLine(), out int input);
//                if (validInput && (input <= 3 && input >= 1))
//                {
//                    switch (input)
//                    {
//                        case 1:
//                            GetCalendarItemsByCategory(calendar);
//                            break;
//                        case 2:
//                            GetCalendarItemsByCategoryWithStartAndEndDate(calendar);
//                            break;
//                        case 3:
//                            GetCalendarItemsByCategoryWithAllValues(calendar);
//                            break;
//                    }
//                }
//                else
//                {
//                    Console.WriteLine("Must enter a number between 1 and 3");
//                    validInput = false;
//                }

//            }
//        }

//        public static void GetCalendarItemsByCategory(HomeCalendar calendar)
//        {
//            List<CalendarItemsByCategory> calendarItemsByCategories = calendar.GetCalendarItemsByCategory(null, null, false, 0);
//            Console.WriteLine("Date               Short Description         Duration     BusyTime");

//            foreach (CalendarItemsByCategory calendarItem in calendarItemsByCategories)
//            {

//                foreach (CalendarItem item in calendarItem.Items)
//                {
//                    Console.WriteLine(
//                                   String.Format("{0} {1,-20}  {2,8} {3,12}",
//                                       item.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
//                                       item.ShortDescription,
//                                       item.DurationInMinutes, item.BusyTime)
//                               );
//                }
//                Console.WriteLine($"Category: {calendarItem.Category}");
//            }
//        }

//        public static void GetCalendarItemsByCategoryWithStartAndEndDate(HomeCalendar calendar)
//        {
//            List<CalendarItemsByCategory> calendarItemsByCategories = calendar.GetCalendarItemsByCategory(DateTime.Now.AddYears(-5), DateTime.Now, false, 0);
//            Console.WriteLine("Date               Short Description         Duration     BusyTime");

//            foreach (CalendarItemsByCategory calendarItem in calendarItemsByCategories)
//            {

//                foreach (CalendarItem item in calendarItem.Items)
//                {
//                    Console.WriteLine(
//                                   String.Format("{0} {1,-20}  {2,8} {3,12}",
//                                       item.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
//                                       item.ShortDescription,
//                                       item.DurationInMinutes, item.BusyTime)
//                               );
//                }
//                Console.WriteLine($"Category: {calendarItem.Category}");
//            }
//        }

//        public static void GetCalendarItemsByCategoryWithAllValues(HomeCalendar calendar)
//        {
//            List<CalendarItemsByCategory> calendarItemsByCategories = calendar.GetCalendarItemsByCategory(DateTime.Now.AddYears(-5), DateTime.Now, true, 8);
//            Console.WriteLine("Date               Short Description         Duration     BusyTime");

//            foreach (CalendarItemsByCategory calendarItem in calendarItemsByCategories)
//            {

//                foreach (CalendarItem item in calendarItem.Items)
//                {
//                    Console.WriteLine(
//                                   String.Format("{0} {1,-20}  {2,8} {3,12}",
//                                       item.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
//                                       item.ShortDescription,
//                                       item.DurationInMinutes, item.BusyTime)
//                               );
//                }
//                Console.WriteLine($"Category: {calendarItem.Category}");
//            }
//        }
//        #endregion

//        #region GetCalendarDictionary

//        public static void GetCalendarDictionaryByMonthAndCategoryVariations(HomeCalendar calendar)
//        {
//            Console.WriteLine(@"
//1. Get Calendar Dictionary By Month And Category
//2. Get Calendar Dictionary By Month And Category With Start and End Date 
//3. Get Calendar Dictionary By Month And Category With All Values");

//            bool validInput = false;
//            while (validInput == false)
//            {

//                validInput = int.TryParse(Console.ReadLine(), out int input);
//                if (validInput && (input <= 3 && input >= 1))
//                {
//                    switch (input)
//                    {
//                        case 1:
//                            GetCalendarDictionaryByMonthAndCategory(calendar);
//                            break;
//                        case 2:
//                            GetCalendarDictionaryByMonthAndCategoryWithStartAndEndDate(calendar);
//                            break;
//                        case 3:
//                            GetCalendarDictionaryByMonthAndCategoryWithAllValues(calendar);
//                            break;
//                    }
//                }
//                else
//                {
//                    Console.WriteLine("Must enter a number between 1 and 3");
//                    validInput = false;
//                }

//            }
//        }
//        public static void GetCalendarDictionaryByMonthAndCategory(HomeCalendar calendar)
//        {
//            Dictionary<string, Dictionary<string, List<CalendarItem>>> calendarDictionary = new Dictionary<string, Dictionary<string, List<CalendarItem>>>();

//            List<CalendarItem> calendarItems = calendar.GetCalendarItems(null, null, false, 0);

//            foreach (CalendarItem item in calendarItems)
//            {
//                string monthKey = item.StartDateTime.ToString("MM/yyyy");
//                string categoryKey = item.Category;

//                if (!calendarDictionary.ContainsKey(monthKey))
//                {
//                    calendarDictionary[monthKey] = new Dictionary<string, List<CalendarItem>>();
//                }

//                if (!calendarDictionary[monthKey].ContainsKey(categoryKey))
//                {
//                    calendarDictionary[monthKey][categoryKey] = new List<CalendarItem>();
//                }

//                calendarDictionary[monthKey][categoryKey].Add(item);
//            }

//            foreach (var monthEntry in calendarDictionary)
//            {
//                Console.WriteLine($"Month: {monthEntry.Key}");

//                foreach (var categoryEntry in monthEntry.Value)
//                {
//                    Console.WriteLine($"  Category: {categoryEntry.Key}");

//                    foreach (var calendarItem in categoryEntry.Value)
//                    {
//                        Console.WriteLine(
//                            String.Format("{0} {1,-20}  {2,8} {3,12}",
//                                calendarItem.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
//                                calendarItem.ShortDescription,
//                                calendarItem.DurationInMinutes, calendarItem.BusyTime)
//                        );
//                    }
//                }
//            }
//        }
//        public static void GetCalendarDictionaryByMonthAndCategoryWithStartAndEndDate(HomeCalendar calendar)
//        {
//            Dictionary<string, Dictionary<string, List<CalendarItem>>> calendarDictionary = new Dictionary<string, Dictionary<string, List<CalendarItem>>>();

//            List<CalendarItem> calendarItems = calendar.GetCalendarItems(DateTime.Now.AddYears(-5), DateTime.Now, false, 0);

//            foreach (CalendarItem item in calendarItems)
//            {
//                string monthKey = item.StartDateTime.ToString("MM/yyyy");
//                string categoryKey = item.Category;

//                if (!calendarDictionary.ContainsKey(monthKey))
//                {
//                    calendarDictionary[monthKey] = new Dictionary<string, List<CalendarItem>>();
//                }

//                if (!calendarDictionary[monthKey].ContainsKey(categoryKey))
//                {
//                    calendarDictionary[monthKey][categoryKey] = new List<CalendarItem>();
//                }

//                calendarDictionary[monthKey][categoryKey].Add(item);
//            }

//            foreach (var monthEntry in calendarDictionary)
//            {
//                Console.WriteLine($"Month: {monthEntry.Key}");

//                foreach (var categoryEntry in monthEntry.Value)
//                {
//                    Console.WriteLine($"  Category: {categoryEntry.Key}");

//                    foreach (var calendarItem in categoryEntry.Value)
//                    {
//                        Console.WriteLine(
//                            String.Format("{0} {1,-20}  {2,8} {3,12}",
//                                calendarItem.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
//                                calendarItem.ShortDescription,
//                                calendarItem.DurationInMinutes, calendarItem.BusyTime)
//                        );
//                    }
//                }
//            }
//        }
//        public static void GetCalendarDictionaryByMonthAndCategoryWithAllValues(HomeCalendar calendar)
//        {
//            Dictionary<string, Dictionary<string, List<CalendarItem>>> calendarDictionary = new Dictionary<string, Dictionary<string, List<CalendarItem>>>();

//            List<CalendarItem> calendarItems = calendar.GetCalendarItems(DateTime.Now.AddYears(-5), DateTime.Now, true, 8);

//            foreach (CalendarItem item in calendarItems)
//            {
//                string monthKey = item.StartDateTime.ToString("MM/yyyy");
//                string categoryKey = item.Category;

//                if (!calendarDictionary.ContainsKey(monthKey))
//                {
//                    calendarDictionary[monthKey] = new Dictionary<string, List<CalendarItem>>();
//                }

//                if (!calendarDictionary[monthKey].ContainsKey(categoryKey))
//                {
//                    calendarDictionary[monthKey][categoryKey] = new List<CalendarItem>();
//                }

//                calendarDictionary[monthKey][categoryKey].Add(item);
//            }

//            foreach (var monthEntry in calendarDictionary)
//            {
//                Console.WriteLine($"Month: {monthEntry.Key}");

//                foreach (var categoryEntry in monthEntry.Value)
//                {
//                    Console.WriteLine($"  Category: {categoryEntry.Key}");

//                    foreach (var calendarItem in categoryEntry.Value)
//                    {
//                        Console.WriteLine(
//                            String.Format("{0} {1,-20}  {2,8} {3,12}",
//                                calendarItem.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
//                                calendarItem.ShortDescription,
//                                calendarItem.DurationInMinutes, calendarItem.BusyTime)
//                        );
//                    }
//                }
//            }
//        }

//        #endregion
    }



}
