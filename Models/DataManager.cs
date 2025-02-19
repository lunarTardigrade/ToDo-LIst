using SQLite;

namespace ToDo_LIst.Models
{
    public class DataManager<T> where T : class, IIdentifiable, new() 
    {
        public static DataManager<T> Create()
        {
            return new DataManager<T>();
        }

        public SQLiteConnection GetDbConnection()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var databasePath = Path.Combine(path, "tododatabase.db");
            var _db = new SQLiteConnection(databasePath);
            _db.CreateTable<T>();
            return _db;
        }
        public T GetRecord(string id)
        {
            using (var db = GetDbConnection())
            {
                return db.Find<T>(id);
            } 
        }
        public List<T> GetTable()
        {
            using (var db = GetDbConnection())
            {
                List<T> result = [.. db.Table<T>()];
                return result;
            }        
        }
        public List<T> GetRecordsBySecondaryId(string secondaryIdName, string secondaryIdValue)
        {
            using (var db = GetDbConnection())
            {
                string tableName = typeof(T).Name;
                string query = $"SELECT * FROM {tableName} WHERE {secondaryIdName} = ?";

                return db.Query<T>(query, secondaryIdValue);
            }
        } 


        public void InsertItem(T item)
        {
            using (var db = GetDbConnection())
            {
                db.Insert(item);
            }
        }

        public void DeleteItem(string identifier)
        {
            using (var db = GetDbConnection())
            {
                var itemToDelete = db.Table<T>().FirstOrDefault(itemTable => itemTable.Identifier == identifier);  
                
                if (itemToDelete != null)
                {
                    db.Delete(itemToDelete);
                }
            }
            // If the Deleted Item has children, remove the reference to the parent in each child:
            var itemsToUpdateParent = GetRecordsBySecondaryId("Parent", identifier);
            if (itemsToUpdateParent != null)
            {
                foreach (T item in itemsToUpdateParent)
                {
                    var parentProperty = item.GetType().GetProperty("Parent");
                    if (parentProperty != null) {
                        parentProperty.SetValue(item,null);
                        UpdateItem(item);
                    }
                }
            }
        }

        public void UpdateItem(T item)
        {
            using (var db = GetDbConnection())
            {
                db.Update(item);
            }
        }
        public void DropTable()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var databasePath = Path.Combine(path, "tododatabase.db");
            using (var connection = new SQLiteConnection(databasePath))
            {
                connection.DropTable<T>();
            }
        }
    }
}
