using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LM.Utilities.DBAccess
{
    /// <summary>
    /// provide operation help on mongodb based on offical mongodb driver 
    /// </summary>
    public class MongoDBHelper
    {
        static IMongoDatabase db = null;
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

        public static void FindBizMsgFromDB(IMongoDatabase db)
        {
            var collection = db.GetCollection<BsonDocument>("BizMsg");
            var bsonDoc = new BsonDocument();
            bsonDoc.Add("BizMsgID", "440184170911418816");
            var t = collection.Find(bsonDoc).SingleOrDefault();
            byte[] bytes = (byte[])t.GetValue("Data");
            MemoryStream ms = new MemoryStream(bytes);
            XmlDocument doc = new XmlDocument();
            doc.Load(ms);
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="fileDir"></param>
        /// <param name="db"></param>
        public static void InsertBizMsg2DB(string fileDir, IMongoDatabase db)
        {
            var collection = db.GetCollection<BsonDocument>("BizMsg");

            List<string> files = Directory.EnumerateFiles(fileDir, "*.xml", SearchOption.AllDirectories).ToList();
            for (int i = 0; i < files.Count; i++)
            {
                if (File.Exists(files[i]))
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(files[i]);
                        string bizMsgId = doc.SelectSingleNode("//BizMsgID") == null ? "" : doc.SelectSingleNode("//BizMsgID").InnerText;
                        string ywlx = doc.SelectSingleNode("//RecType") == null ? "" : doc.SelectSingleNode("//RecType").InnerText;
                        string ywh = doc.SelectSingleNode("//RecFlowID") == null ? "" : doc.SelectSingleNode("//RecFlowID").InnerText;
                        string bdcdyh = doc.SelectSingleNode("//EstateNum") == null ? "" : doc.SelectSingleNode("//EstateNum").InnerText;
                        string createTime = doc.SelectSingleNode("//CreateDate") == null ? "" : doc.SelectSingleNode("//CreateDate").InnerText;
                        byte[] fileBytes = File.ReadAllBytes(files[i]);

                        var bsonDoc = new BsonDocument();
                        bsonDoc.Add("BizMsgID", bizMsgId);
                        bsonDoc.Add("RecType", ywlx);
                        bsonDoc.Add("RecFlowID", ywh);
                        bsonDoc.Add("EstateNum", bdcdyh);
                        bsonDoc.Add("CreateDate", createTime);
                        bsonDoc.Add("Data", fileBytes);

                        collection.InsertOne(bsonDoc);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// 使用GridFS向mongodb中添加文件
        /// </summary>
        /// <param name="db"></param>
        /// <param name="filePath"></param>
        /// <param name="fileCode"></param>
        public static void UploadFile(IMongoDatabase db, string filePath, string fileCode)
        {
            GridFSBucket gfsb = new GridFSBucket(db);
            //文件上传
            string sourceFileName = filePath;
            FileInfo fileInfo = new FileInfo(filePath);
            string fileName = fileInfo.Name;

            byte[] fileBytes = File.ReadAllBytes(sourceFileName);
            GridFSUploadOptions gfsuo = new GridFSUploadOptions();
            gfsuo.Metadata = new BsonDocument();
            gfsuo.Metadata.Add("fileCode", fileCode);
            ObjectId id = gfsb.UploadFromBytes(fileName, fileBytes, gfsuo);
            //Debug.WriteLine(id.ToString());
        }

        public static void UploadBizMsg2MongoDB(IMongoDatabase db, string filePath)
        {
            GridFSBucket gfsb = new GridFSBucket(db);
            //文件上传
            string sourceFileName = filePath;
            FileInfo fileInfo = new FileInfo(filePath);
            string fileName = fileInfo.Name;
            byte[] fileBytes = File.ReadAllBytes(sourceFileName);
            GridFSUploadOptions gfsuo = new GridFSUploadOptions();
            gfsuo.Metadata = new BsonDocument();
            //gfsuo.Metadata.Add("fileCode", fileCode);
            ObjectId id = gfsb.UploadFromBytes(fileName, fileBytes, gfsuo);
            //Debug.WriteLine(id.ToString());
        }
        /// <summary>
        /// 使用GridFS从Monogodb中获取数据
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id"></param>
        /// <param name="fileCode"></param>
        public static void DownloadFileFromDB(IMongoDatabase db, BsonValue id, string fileCode)
        {
            GridFSBucket gfsb = new GridFSBucket(db);
            byte[] bytes = gfsb.DownloadAsBytes(id);
            //string outputPath = tempImgPath + "\\" + fileCode + fileExtName;
            //File.WriteAllBytes(outputPath, bytes);
            //currentSavedImg = outputPath;
            //OuputMessage("结果", "成功从数据库中读取文件,并保存");
            //filesOutputPath.Add(outputPath);
            //CutImage2Tiles(outputPath, fileCode);
        }
        public static async void QueryAndDownloadFromDB(IMongoDatabase db, string fileCode)
        {
            //文件读取与下载
            var collection = db.GetCollection<BsonDocument>("fs.files");
            var filter = new BsonDocument();
            //filter.Add("md5", "fd181c5331929ad3b0e95be3f0016587");                       
            filter.Add("fileCode", fileCode);
            var count = 0;
            using (var cursor = await collection.FindAsync(filter))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (var document in batch)
                    {
                        BsonValue bv;
                        document.TryGetValue("_id", out bv);
                        //document.TryGetValue("fileCode", out tFileCode);
                        string fileStr = document.ToString();
                        //OuputMessage("查询文档内容", bv.ToString());        
                        //OuputMessage("查询文档内容",fileStr);                 
                        DownloadFileFromDB(db, bv, fileCode);
                        count++;
                    }
                }
                //for (int i = 0; i < filesOutputPath.Count; i++)
                //{
                //    Console.WriteLine(filesOutputPath[i]);
                //}
            }
        }
    }
}
