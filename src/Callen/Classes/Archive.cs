using SQLite;

namespace Callen
{
    /// <summary>
    /// Represents an archive entry used as the folder and theme source for calendars.
    /// </summary>
    public class Archive
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string code { get; set; }
        public string theme { get; set; }
        public int usage_count { get; set; }
    }
}
