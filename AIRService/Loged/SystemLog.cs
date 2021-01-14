using System;
using System.IO;
using System.Web;

namespace Helper
{
    public class SystemLogg
    {
        public static void WriteLog(string message)
        {
            try
            {
                DateTime dateTime = Helper.TimeData.TimeHelper.UtcDateTime;
                //$@"{AppDomain.CurrentDomain.BaseDirectory}Logged\" + fileName;
                //Save file to system
                string fileFolderPath = HttpContext.Current.Server.MapPath($"~/Logged/{dateTime.Year}/{dateTime.Month}/");
                if (!System.IO.Directory.Exists(fileFolderPath))
                    System.IO.Directory.CreateDirectory(fileFolderPath);
                //  
                string fileName = $"Log-{dateTime.Day}.txt";
                StreamWriter streamWriter = new StreamWriter(fileFolderPath + fileName, true);
                //
                streamWriter.WriteLine($"[{dateTime:yyyy-MM-dd HH:mm:ss}] " + message);
                streamWriter.Flush();
                streamWriter.Close();
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

    }
}
