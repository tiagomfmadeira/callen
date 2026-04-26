using System;
using System.Globalization;
using System.Text;

namespace Callen
{
    internal static class FolderRangeHelper
    {
        public static bool TryNormalizeRange(string input, out string normalizedRange, out string error)
        {
            normalizedRange = string.Empty;
            error = null;

            var value = (input ?? string.Empty).Trim();
            if (value.Length == 0)
            {
                error = Loc.T("ManageArchive.RangeRequired");
                return false;
            }

            if (value.StartsWith("(") && value.EndsWith(")") && value.Length > 2)
                value = value.Substring(1, value.Length - 2).Trim();

            var parts = SplitRange(value);
            if (parts.Length != 2)
            {
                error = Loc.T("ManageArchive.RangeInvalid");
                return false;
            }

            var start = NormalizeTokenForStorage(parts[0]);
            var end = NormalizeTokenForStorage(parts[1]);

            if (start.Length == 0 || end.Length == 0)
            {
                error = Loc.T("ManageArchive.RangeInvalid");
                return false;
            }

            var compareStart = NormalizeComparisonValue(start);
            var compareEnd = NormalizeComparisonValue(end);
            if (compareStart.Length == 0 || compareEnd.Length == 0)
            {
                error = Loc.T("ManageArchive.RangeInvalid");
                return false;
            }

            if (string.Compare(compareStart, compareEnd, StringComparison.Ordinal) > 0)
            {
                error = Loc.T("ManageArchive.RangeOrderInvalid");
                return false;
            }

            normalizedRange = start + "/" + end;
            return true;
        }

        public static string NormalizeRangeOrEmpty(string input)
        {
            string normalized;
            string error;
            return TryNormalizeRange(input, out normalized, out error) ? normalized : string.Empty;
        }

        public static bool IsNameInRange(string name, string normalizedRange)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var bounds = SplitRange(normalizedRange);
            if (bounds.Length != 2)
                return false;

            var normalizedName = NormalizeComparisonValue(name);
            var start = NormalizeComparisonValue(bounds[0]);
            var end = NormalizeComparisonValue(bounds[1]);

            if (normalizedName.Length == 0 || start.Length == 0 || end.Length == 0)
                return false;

            var afterStart = string.Compare(normalizedName, start, StringComparison.Ordinal) >= 0
                || normalizedName.StartsWith(start, StringComparison.Ordinal);
            var beforeEnd = string.Compare(normalizedName, end, StringComparison.Ordinal) <= 0
                || normalizedName.StartsWith(end, StringComparison.Ordinal);

            return afterStart && beforeEnd;
        }

        private static string NormalizeTokenForStorage(string token)
        {
            var input = (token ?? string.Empty).Trim();
            return input.Length == 0 ? string.Empty : input;
        }

        private static string[] SplitRange(string value)
        {
            return (value ?? string.Empty)
                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string NormalizeForComparison(string value)
        {
            return NormalizeComparisonValue(value);
        }

        private static string NormalizeComparisonValue(string value)
        {
            var input = (value ?? string.Empty).Trim().ToUpperInvariant();
            if (input.Length == 0)
                return string.Empty;

            var decomposed = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(decomposed.Length);
            foreach (var ch in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.NonSpacingMark)
                    continue;

                if (!char.IsWhiteSpace(ch))
                    sb.Append(ch);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
