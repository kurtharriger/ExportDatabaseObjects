using System;
using System.Diagnostics;

namespace ExportDatabaseObjects
{
    public class Program
    {
        static int Main(string[] args)
        {
            ScriptGenerator scriptGenerator = new ScriptGenerator();
            scriptGenerator.LoadConfigurationOptions();

            if (args != null && args.Length > 0) scriptGenerator.ScriptModifiedObjectsOnly = false;
            try
            {
                scriptGenerator.ValidateOptions();
            }
            catch (ApplicationException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }

            try
            {
                scriptGenerator.ProgressNotification += new ScriptGenerator.ProgressNotificationHandler(WriteProgressMessage);
                scriptGenerator.Script(args);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -2;
            }
        }
        static void WriteProgressMessage(string message)
        {
            string msg = String.Format("{0} - {1}", DateTime.Now.ToString("HH:mm:ss"), message);
            Console.WriteLine(msg);
            Trace.WriteLine(msg);
        }
    }
}