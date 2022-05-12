using SQLite;

public class Activity
{

        [PrimaryKey, AutoIncrement]
		public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
}
