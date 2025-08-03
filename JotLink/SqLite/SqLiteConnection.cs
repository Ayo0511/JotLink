using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JotLink
{
   public class SqLiteConnection
    {
        public ISQLiteAsyncConnection CreateConnection()
        {
            return new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory,"notes-Demo.db3"),
                SQLiteOpenFlags.ReadWrite|SQLiteOpenFlags.Create|SQLiteOpenFlags.SharedCache);
        }
    }
}


 