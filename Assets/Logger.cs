using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
class Logger {
        int sessionCount = 1;
        public static string LOCAL_PATH = Directory.GetCurrentDirectory() + "\\CLOWN_LOGS";
        public static string LOGS_PATH = Directory.GetCurrentDirectory() + "\\CLOWN_LOGS";
        public static string EVENT_LOGS = "\\EventLogs";
        StreamWriter eventsLogs;
        public void initLoggingFile () {
            System.IO.Directory.CreateDirectory(LOGS_PATH);
            string writePath = LOGS_PATH;
            while (File.Exists(writePath + EVENT_LOGS + Convert.ToString(sessionCount) + ".csv"))
            {
                ++sessionCount;
            }
            writePath = LOGS_PATH + EVENT_LOGS + Convert.ToString(sessionCount) + ".csv";
            eventsLogs = new StreamWriter(writePath, true);
            using (eventsLogs)
                eventsLogs.WriteLine(DateTime.Now.ToString("MM.dd.yyyy hh:mm:ss.fff") + "," +
                "Time From start," +
                "Action author," +
                "Action target," +
                "Action number," +
                "Message," +
                "First clown Appraisals Valence," +
                "First clown Appraisals Arousal," +
                "First clown Appraisals Dominance," +
                "First clown Feelings Valence," +
                "First clown Feelings Arousal," +
                "First clown Feelings Dominance,"
                );
        }

        public void updateLogs (string author, string target, int actionNumber, string message,
        double [] aprraisalsFirstClown, double[] feelingsFirstClown, 
        string moralSchemaFirstToSecond) {

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            string writePath = LOGS_PATH + EVENT_LOGS + Convert.ToString(sessionCount) + ".csv";
            eventsLogs = new StreamWriter(writePath, true);
            using (eventsLogs)
                eventsLogs.WriteLine(DateTime.Now.ToString("MM.dd.yyyy hh:mm:ss.fff") + "," +
                Time.realtimeSinceStartup + "," +
                author + "," +
                target + "," +
                actionNumber + "," +
                message + "," +
                aprraisalsFirstClown[0] + "," +
                aprraisalsFirstClown[1] + "," +
                aprraisalsFirstClown[2] + "," +
                feelingsFirstClown[0] + "," +
                feelingsFirstClown[1] + "," +
                feelingsFirstClown[2] + "," +
                moralSchemaFirstToSecond + ","
                );
        }
    }

