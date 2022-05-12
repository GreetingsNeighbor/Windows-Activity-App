using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;



namespace Active_Window_App
{
    public class SQLiteManager
    {
        static string dbName = "activities.db";
        static string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string dbPath = System.IO.Path.Combine(path, dbName);
    }
}
