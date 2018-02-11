using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LM.Utilities
{
    public class Log4netUtility
    {
        public static void initialLog4Net() {
            //log4net.Config.XmlConfigurator.ConfigureAndWatch(new )            
            string path = AppDomain.CurrentDomain.BaseDirectory + "log4net/log4net.config";
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(path));
        }
        public static string Exception(string id, Exception ex, string attachMessage) {
            string result = string.Empty;
            //log4net的初始化有两种形式:1.如下 2.写在应用程序集中(见AssemeblyInfo.cs)
            //log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.config"));
            log4net.ILog log = log4net.LogManager.GetLogger("OneCardException");
            if (!log.IsErrorEnabled)
                initialLog4Net();                   
            if (log.IsErrorEnabled) {
                string strMessage = string.Format("RequestID:{0}\r\nMesssage:{1}\r\nStackTrace:{2}\r\nAttachMessage:{3}", id, ex.Message, ex.StackTrace, attachMessage);
                log.Error(strMessage);
                result = strMessage;
            }
            log = null;
            return result;
        }

        /// <summary>
        /// 追加一条一卡通请求日志信息
        /// </summary>
        /// <param name="id">请求ID</param>
        /// <param name="ip">请求的IP地址</param>
        /// <param name="datetime">时间</param>
        /// <param name="strParam">请求时传入的参数</param>
        public static void RequestLog(string id, string ip, string datetime, string strParam)
        {
            log4net.ILog log = log4net.LogManager.GetLogger("OneCardRequest");
            if (!log.IsInfoEnabled)
                initialLog4Net();
            if (log.IsInfoEnabled)
            {
                string strMessage = string.Format("RequestID:{0}\r\nIP:{1}\r\nDateTime:{2}\r\nParam:{3}", id, ip, datetime, strParam);
                log.Info(strMessage);
            }
            log = null;
        }
        /// <summary>
        /// 追加一条一卡通处理结果日志
        /// </summary>
        /// <param name="id">请求ID</param>
        /// <param name="datetime">时间</param>
        /// <param name="strResult">处理结果</param>
        public static void ResultLog(string id, string datetime, string strResult)
        {
            log4net.ILog log = log4net.LogManager.GetLogger("OneCardResult");
            if (!log.IsInfoEnabled)
                initialLog4Net();
            if (log.IsInfoEnabled)
            {
                string strMessage = string.Format("RequestID:{0}\r\nDateTime:{1}\r\nParam:{2}", id, datetime, strResult);
                log.Info(strMessage);
            }
            log = null;
        }
        /// <summary>
        /// 追加一条普通的日志信息
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(string message)
        {
            log4net.ILog log = log4net.LogManager.GetLogger("InfoLog");
            if (!log.IsInfoEnabled)
                initialLog4Net();
            if (log.IsInfoEnabled)
            {
                log.Info(message);
            }
            log = null;
        }
    }
}
