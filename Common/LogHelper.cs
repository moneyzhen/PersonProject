using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ZHONGJIAN_API.Common
{
    /// <summary>
    ///     日志文件
    /// </summary>
    public class LogHelper
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        [Obsolete]
        static LogHelper()
        {
            var configFileName = AppDomain.CurrentDomain.BaseDirectory + "NLog.config";
            if (!File.Exists(configFileName))
            {
                configFileName = configFileName.Replace("\\bin", "");
                configFileName = configFileName.Replace("\\Debug", "");
                configFileName = configFileName.Replace("\\Release", "");
            }
            if (File.Exists(configFileName))
            {
                LogManager.Configuration = new XmlLoggingConfiguration(configFileName, true);
            }
            else
            {
                //Create configuration object
                var config = new LoggingConfiguration();
                //Create targets and add them to the configuration
                var fileTarget = new FileTarget();
                config.AddTarget("file", fileTarget);
                fileTarget.FileName = "${basedir}/logs/${shortdate}.log";
                fileTarget.Layout = "${longdate} ${uppercase:${level}} ${message}";
                config.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Debug, fileTarget));
                LogManager.Configuration = config;
            }
        }

        public static void Info(string msg)
        {
            Log.Info(msg);
        }

        public static void ErrorLog(string erro, string methodName = null)
        {
            try
            {
                string stackTrace;
                if (methodName == null)
                {
                    var stackFrame = new StackFrame(1);
                    methodName = stackFrame.GetMethod().Name;
                    var fileName = stackFrame.GetFileName();
                    var lineNumber = stackFrame.GetFileLineNumber();
                    stackTrace = $"{methodName} ({fileName} line {lineNumber})";
                }
                else
                {
                    stackTrace = methodName;
                }
                Log.Error($"{methodName} erro,ex.Message={erro},ex.StackTrace={stackTrace}");
            }
            catch
            {
                Log.Error($"has erro,ex.Message={erro}");
            }
        }

        public static void ExceptionLog(Exception ex, string methodName = null)
        {
            try
            {
                if (methodName == null)
                {
                    var stackFrame = new StackFrame(1);
                    methodName = stackFrame.GetMethod().Name;
                }
                Log.Error($"{methodName} exception,ex.Message={ex.Message},ex.StackTrace={ex.StackTrace}");
            }
            catch
            {
                Log.Error(ex);
            }
        }

        public static void EntryLog(string userId, string viewUrl)
        {
            Info($"User {userId} entry {viewUrl}");
        }
    }
}
