using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LM.Utilities.DBAccess
{
    /// <summary>
    /// provide operation help on mongodb based on offical mongodb driver 
    /// </summary>
    public class MongoDBHelper
    {
        IMongoDatabase db = null;
        /// <summary>
        /// Initial db object;
        /// </summary>
        /// <param name="connectionString">like mongodb://127.0.0.1:27017</param>
        /// <param name="dbName"></param>
        public MongoDBHelper(string connectionString,string dbName) {
            MongoUrl mongourl = MongoUrl.Create(connectionString);
            var client = new MongoClient(mongourl);
            db = client.GetDatabase(dbName);
        }
    }
}
