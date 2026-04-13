using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SQLite;

namespace Callen
{
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
                    conn.Execute("INSERT INTO Archive (code, theme) VALUES (?, ?)", folderName, theme);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating folder: " + ex.Message);
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
                MessageBox.Show("Error in GetPicItems: " + ex.Message);
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
                return conn.Query<Archive>("SELECT code, MIN(theme) AS theme FROM Archive GROUP BY code");
            }
        }

        public static List<Archive> GetFoldersThemes(string code)
        {
            using (var conn = new SQLiteConnection(App.databasePath))
            {
                return conn.Query<Archive>("SELECT * FROM Archive WHERE code = ?", code);
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

        private static void AddLikeCondition(
            ICollection<string> conditions,
            ICollection<object> parameters,
            string columnName,
            string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            conditions.Add(columnName + " LIKE ?");
            parameters.Add("%" + value + "%");
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
    }
}
