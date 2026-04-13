using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Callen
{
    /// <summary>
    /// Represents a calendar record stored in the local SQLite database.
    /// </summary>
    public class Calendar
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string year { get; set; }
        public string matrix { get; set; }
        public string collection { get; set; }
        public string date_inserted { get; set; }
        public string date_modified { get; set; }
        public string date_viewed { get; set; }
        public int deleted { get; set; }
        public string pic_path { get; set; }
        [ForeignKey(typeof(Archive))]
        public int archive_id { get; set; }
        public string code { get; set; }
        public string theme { get; set; }

        public Calendar()
        {
        }
    }
}
