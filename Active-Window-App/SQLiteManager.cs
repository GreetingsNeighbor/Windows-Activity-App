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

        public string _dbPath ;
        private SQLiteConnection _connection ;

        public SQLiteManager()
        {
            string dbName = "activities.db";
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _dbPath = System.IO.Path.Combine(path, dbName);
            CreateActivityTable();
        }

        public void CreateActivityTable()
        {
            using (SQLiteConnection conn = new SQLiteConnection(_dbPath))
                conn.CreateTable<Activity>();

        }
        public bool IsExistingTable(string tablename)
        {
            using (SQLiteConnection conn = new SQLiteConnection(_dbPath))
                conn.Query<Object>("SELECT name FROM sqlite_master WHERE type = 'table' AND name='"+ tablename+"'");
            return true;
        }
        public void SaveActivity(string dbPath)
        {
            Activity activity = new Activity();
            using (SQLiteConnection conn = new SQLiteConnection(_dbPath))
                conn.Insert(activity);
            
       
        }
    }

}
