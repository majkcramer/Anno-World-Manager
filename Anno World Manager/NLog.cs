using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Targets;

namespace Anno_World_Manager
{


    internal static class Log
    {
        /// <summary>
        /// Logger Class to consume for Application wide Logging
        /// </summary>
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Log Filename where to log to
        /// </summary>
        private static String log_filename = "log.txt";

        
        //  Source: https://nlog-project.org/config/?tab=layout-renderers
#if DEBUG
        //  For DEBUGGING Purposes -> VERY Verbose Logging
        private static String log_layout = "${longdate}|${level:uppercase=true}|${callsite:className=true:fileName=false:includeSourcePath=false:methodName=true}|${callsite-linenumber:skipFrames=0}|${message:withexception=true}";
        private static LogLevel loglevel_console_min = LogLevel.Debug;
        private static LogLevel loglevel_file_min = LogLevel.Trace;
#else
        //  For PRODUCTION Purposes -> Brief Logging
        private static String log_layout = "${longdate}|${level:uppercase=true}|${message:withexception=true}";
        private static LogLevel loglevel_console_min = LogLevel.Info;
        private static LogLevel loglevel_file_min = LogLevel.Info;
#endif

        public static void Initialize()
        {
            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = log_filename };
            //var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            var logconsole = new NLog.Targets.DebuggerTarget();

            logfile.Layout = log_layout;
            logconsole.Layout = log_layout;

            // Rules for mapping loggers to targets            
            config.AddRule(loglevel_file_min, LogLevel.Fatal, logfile);
            config.AddRule(loglevel_console_min, LogLevel.Fatal, logconsole);

            // Apply config           
            NLog.LogManager.Configuration = config;
        }

        public static string GetFilename()
        {
            var filename = LogManager.Configuration?.AllTargets.OfType<FileTarget>()
            .Select(x => x.FileName.Render(LogEventInfo.CreateNullEvent()))
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
            return filename;
        }

    }

}
