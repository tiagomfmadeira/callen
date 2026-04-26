using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SQLite;
using Callen.Windows;

namespace Callen
{
    public sealed class FolderSplitRequest
    {
        public string SourceCode { get; set; }
        public string TargetCodeA { get; set; }
        public string TargetRangeA { get; set; }
        public string TargetCodeB { get; set; }
        public string TargetRangeB { get; set; }
    }

    public sealed class FolderSplitResult
    {
        public int TotalCalendarsMoved { get; set; }
        public int CalendarsMovedToA { get; set; }
        public int CalendarsMovedToB { get; set; }
        public int UnmatchedMovedToA { get; set; }
        public List<string> WarningSamples { get; } = new List<string>();
    }

    public sealed class FolderSplitSuggestion
    {
        public string SourceRange { get; set; }
        public string SuggestedRangeA { get; set; }
        public string SuggestedRangeB { get; set; }
    }

    public class DBConnect
    {
        private const string CalendarSelectWithArchive = @"
            SELECT
                Calendar.id,
                Calendar.name,
                Calendar.description,
                Calendar.year,
                Calendar.collection,
                Calendar.matrix,
                Calendar.pic_path,
                Calendar.archive_id,
                Archive.code AS code,
                Archive.theme AS theme
            FROM Calendar
            LEFT JOIN Archive ON Calendar.archive_id = Archive.id
            WHERE 1 = 1";

        public static DataTable SearchCalendar(Calendar calendar)
        {
            var query = BuildCalendarSearchQuery(calendar, null, null, out var parameters);

            using (var conn = new SQLiteConnection(App.databasePath))
            {
                var result = conn.Query<Calendar>(query, parameters.ToArray());
                return result.ToDataTable();
            }
        }

        public static DataTable SearchCalendarPaged(Calendar calendar, int limit, int offset)
        {
            if (limit <= 0)
                limit = 1;
            if (offset < 0)
                offset = 0;

            var query = BuildCalendarSearchQuery(calendar, limit, offset, out var parameters);

            using (var conn = new SQLiteConnection(App.databasePath))
            {
                var result = conn.Query<Calendar>(query, parameters.ToArray());
                return result.ToDataTable();
            }
        }

        public static void RemoveCalendar(int calendarId)
        {
            using (var conn = new SQLiteConnection(App.databasePath))
            {
                conn.Execute("DELETE FROM Calendar WHERE id = ?", calendarId);
            }
        }

        public static void AddCalendar(Calendar calendar, ImageSource image, string imageBasePath)
        {
            using (var conn = new SQLiteConnection(App.databasePath))
            {
                conn.Execute(
                    "INSERT INTO Calendar (name, description, year, matrix, collection, pic_path, archive_id) VALUES (?, ?, ?, ?, ?, ?, ?)",
                    calendar.name,
                    calendar.description,
                    calendar.year,
                    calendar.matrix,
                    calendar.collection,
                    calendar.pic_path ?? string.Empty,
                    calendar.archive_id);

                var calendarId = conn.ExecuteScalar<int>("SELECT last_insert_rowid()");
                calendar.id = calendarId;

                if (image == null || string.IsNullOrWhiteSpace(imageBasePath))
                    return;

                var bitmap = image as BitmapImage;
                var sourcePath = bitmap != null && bitmap.UriSource != null
                    ? bitmap.UriSource.LocalPath
                    : null;

                if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                    return;

                Directory.CreateDirectory(imageBasePath);

                var picPathWithoutExtension = Path.Combine(imageBasePath, "Calendar_" + calendarId);
                var targetPath = picPathWithoutExtension + ".jpeg";

                File.Copy(sourcePath, targetPath, true);
                conn.Execute("UPDATE Calendar SET pic_path = ? WHERE id = ?", picPathWithoutExtension, calendarId);
                calendar.pic_path = picPathWithoutExtension;
            }
        }

        public static void CreateFolder(string folderName, string theme)
        {
            try
            {
                using (var conn = new SQLiteConnection(App.databasePath))
                {
                    var code = (folderName ?? string.Empty).Trim();

                    conn.Execute(
                        "INSERT INTO Archive (code, theme) VALUES (?, ?)",
                        code,
                        theme);
                }
            }
            catch (Exception ex)
            {
                ShowDbDialog(Loc.F("Msg.DbCreateFolderError", ex.Message), string.Empty);
            }
        }

        public static bool DeleteArchiveEntry(int archiveId)
        {
            using (var conn = new SQLiteConnection(App.databasePath))
            {
                if (GetArchiveUsageCount(archiveId) > 0)
                    return false;

                conn.Execute("DELETE FROM Archive WHERE id = ?", archiveId);
                return true;
            }
        }

        public static List<Archive> GetArchiveEntriesByCode(string code)
        {
            using (var conn = new SQLiteConnection(App.databasePath))
            {
                return conn.Query<Archive>(
                    @"SELECT
                          Archive.id,
                          Archive.code,
                          Archive.theme,
                          COUNT(Calendar.id) AS usage_count
                      FROM Archive
                      LEFT JOIN Calendar ON Calendar.archive_id = Archive.id
                      WHERE Archive.code = ?
                      GROUP BY Archive.id, Archive.code, Archive.theme
                      ORDER BY Archive.theme",
                    code);
            }
        }

        public static bool UpdateCalendarInfo(Calendar calendar)
        {
            try
            {
                using (var conn = new SQLiteConnection(App.databasePath))
                {
                    conn.Execute(
                        "UPDATE Calendar SET name = ?, description = ?, year = ?, matrix = ?, collection = ?, pic_path = ?, archive_id = ? WHERE id = ?",
                        calendar.name,
                        calendar.description,
                        calendar.year,
                        calendar.matrix,
                        calendar.collection,
                        calendar.pic_path,
                        calendar.archive_id,
                        calendar.id);

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static DataTable GetPicItems()
        {
            try
            {
                using (var conn = new SQLiteConnection(App.databasePath))
                {
                    var picItems = conn.Query<PicItem>("SELECT * FROM PicItems");
                    return picItems.ToDataTable();
                }
            }
            catch (Exception ex)
            {
                ShowDbDialog(Loc.F("Msg.DbGetPicItemsError", ex.Message), string.Empty);
                return new DataTable("PicItems");
            }
        }

        public static Archive GetArchiveById(int archiveId)
        {
            using (var conn = new SQLiteConnection(App.databasePath))
            {
                return conn.Find<Archive>(archiveId);
            }
        }

        public static int GetArchiveUsageCount(int archiveId)
        {
            using (var conn = new SQLiteConnection(App.databasePath))
            {
                return conn.ExecuteScalar<int>("SELECT COUNT(*) FROM Calendar WHERE archive_id = ?", archiveId);
            }
        }

        public static Calendar GetCalendarInfo(string id)
        {
            int calendarId;
            if (!int.TryParse(id, out calendarId))
                throw new ArgumentException("Invalid calendar id: " + id);

            using (var conn = new SQLiteConnection(App.databasePath))
            {
                var calendars = conn.Query<Calendar>(CalendarSelectWithArchive + " AND Calendar.id = ?", calendarId);
                return calendars.Count > 0 ? calendars[0] : null;
            }
        }

        public static DataView GetItemsInfo()
        {
            using (var conn = new SQLiteConnection(App.databasePath))
            {
                var calendars = conn.Query<Calendar>(CalendarSelectWithArchive);
                return calendars.ToDataTable().DefaultView;
            }
        }

        public static List<Archive> GetFolders()
        {
            using (var conn = new SQLiteConnection(App.databasePath))
            {
                return conn.Query<Archive>(
                    "SELECT code, MIN(theme) AS theme FROM Archive GROUP BY code");
            }
        }

        public static List<Archive> GetFoldersThemes(string code)
        {
            using (var conn = new SQLiteConnection(App.databasePath))
            {
                return conn.Query<Archive>("SELECT id, code, theme FROM Archive WHERE code = ?", code);
            }
        }

        public static void RenameArchiveTheme(int archiveId, string theme)
        {
            using (var conn = new SQLiteConnection(App.databasePath))
            {
                conn.Execute("UPDATE Archive SET theme = ? WHERE id = ?", theme, archiveId);
            }
        }

        public static void RenameFolder(string currentCode, string newCode)
        {
            using (var conn = new SQLiteConnection(App.databasePath))
            {
                conn.Execute("UPDATE Archive SET code = ? WHERE code = ?", newCode, currentCode);
            }
        }

        public static bool FolderCodeExists(string code)
        {
            using (var conn = new SQLiteConnection(App.databasePath))
            {
                return conn.ExecuteScalar<int>("SELECT COUNT(*) FROM Archive WHERE code = ?", code) > 0;
            }
        }

        public static FolderSplitResult SplitFolder(FolderSplitRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var sourceCode = (request.SourceCode ?? string.Empty).Trim();
            var targetCodeA = (request.TargetCodeA ?? string.Empty).Trim();
            var targetCodeB = (request.TargetCodeB ?? string.Empty).Trim();

            if (sourceCode.Length == 0 || targetCodeA.Length == 0 || targetCodeB.Length == 0)
                throw new InvalidOperationException(Loc.T("ManageArchive.SplitMissingFields"));

            if (string.Equals(targetCodeA, targetCodeB, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(Loc.T("ManageArchive.SplitCodesMustDiffer"));
            if (string.Equals(targetCodeA, sourceCode, StringComparison.OrdinalIgnoreCase)
                || string.Equals(targetCodeB, sourceCode, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(Loc.T("ManageArchive.SplitTargetCodeCannotMatchSource"));

            string normalizedRangeA;
            string normalizedRangeB;
            string rangeError;
            if (!FolderRangeHelper.TryNormalizeRange(request.TargetRangeA, out normalizedRangeA, out rangeError))
                throw new InvalidOperationException(rangeError);
            if (!FolderRangeHelper.TryNormalizeRange(request.TargetRangeB, out normalizedRangeB, out rangeError))
                throw new InvalidOperationException(rangeError);

            using (var conn = new SQLiteConnection(App.databasePath))
            {
                if (!string.Equals(sourceCode, targetCodeA, StringComparison.OrdinalIgnoreCase)
                    && FolderCodeExists(targetCodeA))
                    throw new InvalidOperationException(Loc.F("ManageArchive.SplitCodeExists", targetCodeA));

                if (!string.Equals(sourceCode, targetCodeB, StringComparison.OrdinalIgnoreCase)
                    && FolderCodeExists(targetCodeB))
                    throw new InvalidOperationException(Loc.F("ManageArchive.SplitCodeExists", targetCodeB));

                var sourceThemes = conn.Query<Archive>(
                    "SELECT id, code, theme FROM Archive WHERE code = ? ORDER BY theme",
                    sourceCode);

                if (sourceThemes.Count == 0)
                    throw new InvalidOperationException(Loc.T("ManageArchive.NoFolders"));

                var themeNames = sourceThemes
                    .Select(item => item.theme ?? string.Empty)
                    .Distinct(StringComparer.Ordinal)
                    .ToList();

                var mapA = new Dictionary<string, int>(StringComparer.Ordinal);
                var mapB = new Dictionary<string, int>(StringComparer.Ordinal);

                var result = new FolderSplitResult();
                var warnings = new List<string>();

                conn.RunInTransaction(() =>
                {
                    foreach (var theme in themeNames)
                    {
                        conn.Execute(
                            "INSERT INTO Archive (code, theme) VALUES (?, ?)",
                            targetCodeA,
                            theme);
                        mapA[theme] = conn.ExecuteScalar<int>("SELECT last_insert_rowid()");

                        conn.Execute(
                            "INSERT INTO Archive (code, theme) VALUES (?, ?)",
                            targetCodeB,
                            theme);
                        mapB[theme] = conn.ExecuteScalar<int>("SELECT last_insert_rowid()");
                    }

                    var calendars = conn.Query<CalendarSplitRow>(
                        @"SELECT
                              Calendar.id,
                              Calendar.name,
                              Archive.theme
                          FROM Calendar
                          INNER JOIN Archive ON Calendar.archive_id = Archive.id
                          WHERE Archive.code = ?",
                        sourceCode);

                    foreach (var calendar in calendars)
                    {
                        var theme = calendar.theme ?? string.Empty;
                        var inA = FolderRangeHelper.IsNameInRange(calendar.name, normalizedRangeA);
                        var inB = FolderRangeHelper.IsNameInRange(calendar.name, normalizedRangeB);

                        var targetMap = mapA;
                        var movedToA = true;

                        if (!inA && inB)
                        {
                            targetMap = mapB;
                            movedToA = false;
                        }
                        else if (!inA && !inB)
                        {
                            warnings.Add(calendar.name ?? string.Empty);
                        }

                        if (!targetMap.TryGetValue(theme, out var targetArchiveId))
                            targetArchiveId = movedToA ? mapA.Values.First() : mapB.Values.First();

                        conn.Execute("UPDATE Calendar SET archive_id = ? WHERE id = ?", targetArchiveId, calendar.id);

                        result.TotalCalendarsMoved++;
                        if (movedToA)
                        {
                            result.CalendarsMovedToA++;
                            if (!inA && !inB)
                                result.UnmatchedMovedToA++;
                        }
                        else
                        {
                            result.CalendarsMovedToB++;
                        }
                    }

                    conn.Execute("DELETE FROM Archive WHERE code = ?", sourceCode);
                });

                foreach (var name in warnings.Where(item => !string.IsNullOrWhiteSpace(item)).Take(5))
                    result.WarningSamples.Add(name);

                return result;
            }
        }

        public static FolderSplitSuggestion GetFolderSplitSuggestion(string sourceCode)
        {
            var code = (sourceCode ?? string.Empty).Trim();
            var fallback = new FolderSplitSuggestion
            {
                SourceRange = "A/Z",
                SuggestedRangeA = "A/M",
                SuggestedRangeB = "M/Z"
            };

            if (code.Length == 0)
                return fallback;

            using (var conn = new SQLiteConnection(App.databasePath))
            {
                var names = conn.Query<CalendarNameRow>(
                        @"SELECT Calendar.name
                          FROM Calendar
                          INNER JOIN Archive ON Calendar.archive_id = Archive.id
                          WHERE Archive.code = ?",
                        code)
                    .Select(item => (item.name ?? string.Empty).Trim())
                    .Where(item => item.Length > 0)
                    .Distinct(StringComparer.Ordinal)
                    .Select(item => new SortableName
                    {
                        Raw = item,
                        Normalized = FolderRangeHelper.NormalizeForComparison(item)
                    })
                    .Where(item => item.Normalized.Length > 0)
                    .OrderBy(item => item.Normalized, StringComparer.Ordinal)
                    .ThenBy(item => item.Raw, StringComparer.Ordinal)
                    .ToList();

                if (names.Count == 0)
                    return fallback;

                var letters = names
                    .Select(item => FindFirstAtoZ(item.Normalized))
                    .Where(ch => ch != '\0')
                    .ToList();

                if (letters.Count == 0)
                    return fallback;

                var startChar = letters.Min();
                var endChar = letters.Max();
                if (startChar > endChar)
                    return fallback;

                var middleChar = (char)((startChar + endChar) / 2);

                var start = startChar.ToString();
                var middle = middleChar.ToString();
                var end = endChar.ToString();

                string sourceRange;
                if (!FolderRangeHelper.TryNormalizeRange(start + "/" + end, out sourceRange, out _))
                    sourceRange = fallback.SourceRange;

                string rangeA;
                if (!FolderRangeHelper.TryNormalizeRange(start + "/" + middle, out rangeA, out _))
                    rangeA = fallback.SuggestedRangeA;

                string rangeB;
                if (!FolderRangeHelper.TryNormalizeRange(middle + "/" + end, out rangeB, out _))
                    rangeB = fallback.SuggestedRangeB;

                return new FolderSplitSuggestion
                {
                    SourceRange = sourceRange,
                    SuggestedRangeA = rangeA,
                    SuggestedRangeB = rangeB
                };
            }
        }

        private static char FindFirstAtoZ(string text)
        {
            var value = (text ?? string.Empty).Trim();
            for (var i = 0; i < value.Length; i++)
            {
                var ch = value[i];
                if (ch >= 'A' && ch <= 'Z')
                    return ch;
            }

            return '\0';
        }

        private static void AddLikeCondition(
            ICollection<string> conditions,
            ICollection<object> parameters,
            string columnName,
            string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            if (TryParseExactSearchTerm(value, out var exactValue))
            {
                conditions.Add(columnName + " = ?");
                parameters.Add(exactValue);
                return;
            }

            conditions.Add(columnName + " LIKE ?");
            parameters.Add("%" + value + "%");
        }

        private static bool TryParseExactSearchTerm(string value, out string exactValue)
        {
            exactValue = string.Empty;
            var text = (value ?? string.Empty).Trim();
            if (text.Length < 2)
                return false;

            var startsWithDouble = text.StartsWith("\"", StringComparison.Ordinal);
            var endsWithDouble = text.EndsWith("\"", StringComparison.Ordinal);
            var startsWithSingle = text.StartsWith("'", StringComparison.Ordinal);
            var endsWithSingle = text.EndsWith("'", StringComparison.Ordinal);

            if ((!startsWithDouble || !endsWithDouble) && (!startsWithSingle || !endsWithSingle))
                return false;

            exactValue = text.Substring(1, text.Length - 2);
            return true;
        }

        private static string BuildCalendarSearchQuery(
            Calendar calendar,
            int? limit,
            int? offset,
            out List<object> parameters)
        {
            var query = CalendarSelectWithArchive;
            var conditions = new List<string>();
            parameters = new List<object>();

            if (calendar.id > 0)
            {
                conditions.Add("Calendar.id = ?");
                parameters.Add(calendar.id);
            }

            AddLikeCondition(conditions, parameters, "Calendar.name", calendar.name);
            AddLikeCondition(conditions, parameters, "Calendar.description", calendar.description);
            AddLikeCondition(conditions, parameters, "Calendar.year", calendar.year);
            AddLikeCondition(conditions, parameters, "Calendar.collection", calendar.collection);
            AddLikeCondition(conditions, parameters, "Calendar.matrix", calendar.matrix);
            AddLikeCondition(conditions, parameters, "Archive.code", calendar.code);
            AddLikeCondition(conditions, parameters, "Archive.theme", calendar.theme);

            if (conditions.Count > 0)
                query += " AND " + string.Join(" AND ", conditions);

            query += " ORDER BY Calendar.id DESC";

            if (limit != null)
            {
                query += " LIMIT ?";
                parameters.Add(limit.Value);

                if (offset != null)
                {
                    query += " OFFSET ?";
                    parameters.Add(offset.Value);
                }
            }

            return query;
        }

        private sealed class CalendarSplitRow
        {
            public int id { get; set; }
            public string name { get; set; }
            public string theme { get; set; }
        }

        private sealed class CalendarNameRow
        {
            public string name { get; set; }
        }

        private sealed class SortableName
        {
            public string Raw { get; set; }
            public string Normalized { get; set; }
        }

        private static void ShowDbDialog(string message, string context)
        {
            var dialog = new ActionDialogWindow(
                Loc.T("Msg.GenericTitle"),
                context ?? string.Empty,
                message ?? string.Empty,
                Loc.T("Dlg.Close"));

            DialogHelper.ShowOwnedDialog(dialog, DialogHelper.GetActiveMainWindow());
        }
    }
}

