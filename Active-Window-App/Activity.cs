using SQLite;
namespace ActiveModules
{
    public class Activity
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Time { get; set; }

    }
    public class LimitedActivity
    {
        public string Time { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }


    }
}

