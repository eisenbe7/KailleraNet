using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace KailleraNET.Util
{
    /// <summary>
    /// Manages all of the chat logs
    /// </summary>
    public class ChatLogger
    {
        private static List<ChatLogger> chatLoggers = new List<ChatLogger>();

        String LogStr;

        /// <summary>
        /// Gets chat logger, null for main chat
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static ChatLogger getLog(Game g)
        {
            //Main chat logger
            if (g == null)
            {
                var mainLog = chatLoggers.FirstOrDefault(cLog => cLog.id == 0);
                if (mainLog == null) return new ChatLogger(null);
                return mainLog;
            }
            var log = chatLoggers.FirstOrDefault(cLog => cLog.id == g.id);
            if (log == null) return new ChatLogger(g);
            return log;
        }


        int id;
        private ChatLogger(Game currGame)
        {
            if (currGame == null)
            {
                id = 0;
                chatLoggers.Add(this);
                LogStr = (DateTime.Now.ToShortDateString() + "- " + "Server Chat.txt").Replace("/", "-");

            }
            else
            {
                id = currGame.id;
                LogStr = (DateTime.Now.ToShortDateString() + "- " + currGame.name + ".txt").Replace("/", "-");
            }

        }

        public void logChat(string s)
        {
            if (!File.Exists(CleanInput(LogStr))) File.Create(CleanInput(LogStr));
            try
            {
                using (var log = File.AppendText(CleanInput(LogStr)))
                {
                    try
                    {
                        log.WriteLine(DateTime.Now.ToString() + ": " + s);
                    }
                    //If the file is in use or something, we just won't write it.  Whatever.
                    catch (Exception)
                    {

                    }
                    finally
                    {
                        log.Flush();
                    }
                }
            }
            catch (Exception e) { }
        }

        static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings. 
            return Regex.Replace(strIn, @"[^\w\.@-]", ""); 
        }


    }
}
