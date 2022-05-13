using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using ActiveModules;


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
        public void SaveActivity(Activity activity)
        {
 
            using (SQLiteConnection conn = new SQLiteConnection(_dbPath))
                conn.Insert(activity);
            
       
        }
        public List<LimitedActivity> SelectNumActivity(int num)
        {

            using (SQLiteConnection conn = new SQLiteConnection(_dbPath))
            {
                string q = "Select time, title, url from activity ORDER BY id desc LIMIT " + num;
                return conn.Query<LimitedActivity>(q);
            }
        }

        internal void CloseConn()
        {
            using (SQLiteConnection conn = new SQLiteConnection(_dbPath))
            {
                conn.Close();
            }
        }
    }

}
