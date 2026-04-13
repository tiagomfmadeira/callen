using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Callen
{
    internal static class PrintListStore
    {
        private const string PrintListKey = "PrintList";

        public static List<Calendar> GetOrCreate()
        {
            var printList = Application.Current?.Properties[PrintListKey] as List<Calendar>;
            if (printList != null)
                return printList;

            printList = new List<Calendar>();
            if (Application.Current != null)
                Application.Current.Properties[PrintListKey] = printList;

            return printList;
        }

        public static bool AddIfMissing(Calendar calendar)
        {
            if (calendar == null || calendar.id <= 0)
                return false;

            var printList = GetOrCreate();
            if (printList.Any(item => item != null && item.id == calendar.id))
                return false;

            printList.Add(calendar);
            return true;
        }
    }
}
