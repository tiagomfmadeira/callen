using System;
using System.Windows;

namespace Callen
{
    internal static class Loc
    {
        public static string T(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return string.Empty;

            var app = Application.Current;
            if (app == null)
                return key;

            var value = app.TryFindResource(key) as string;
            return string.IsNullOrWhiteSpace(value) ? key : value;
        }

        public static string F(string key, params object[] args)
        {
            var format = T(key);
            try
            {
                return string.Format(format, args);
            }
            catch
            {
                return format;
            }
        }
    }
}
