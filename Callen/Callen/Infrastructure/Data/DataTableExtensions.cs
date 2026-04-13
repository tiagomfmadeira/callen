using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace Callen
{
    internal static class DataTableExtensions
    {
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            var properties = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();

            foreach (PropertyDescriptor property in properties)
            {
                table.Columns.Add(
                    property.Name,
                    Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            foreach (var item in data)
            {
                var row = table.NewRow();
                foreach (PropertyDescriptor property in properties)
                    row[property.Name] = property.GetValue(item) ?? DBNull.Value;

                table.Rows.Add(row);
            }

            return table;
        }
    }
}
