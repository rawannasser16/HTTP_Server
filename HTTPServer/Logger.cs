using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        //static StreamWriter sr = new StreamWriter("log.txt");
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            FileStream fs = new FileStream("log.txt", FileMode.OpenOrCreate);
            //Datetime:
            //message:
            // for each exception write its details associated with datetime
            StreamWriter sw = new StreamWriter(fs);

            sw.WriteLine("Date Time: " + DateTime.Now.ToString());
            sw.WriteLine("Message: " + ex.Message);

            fs.Close();
        }
    }
}
