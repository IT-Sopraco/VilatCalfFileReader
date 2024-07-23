using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using log4net.Config;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace VSM.RUMA.CORE
{
    public class unLogger : VSM.RUMA.CORE.DB.IunLogger
    {
        public delegate void LogMessageEventHandler(unLogger.LogLevel logLevel, string message);
        public static event LogMessageEventHandler OnLogMessage;

        public enum LogLevel
        {
            Trace = 0,
            Debug = 1,
            Info = 2,
            Warn = 3,
            Error = 4,
            Fatal = 5
        }

        private static readonly ILog log = LogManager.GetLogger(getLogName());

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Configure(String XmlConfigPath, String DataSource)
        {
            if (log4net.GlobalContext.Properties["AppName"] == null)
                log4net.GlobalContext.Properties["AppName"] = getLogName();
            string ConfigPath = System.AppDomain.CurrentDomain.BaseDirectory + XmlConfigPath;
            XmlConfigurator.Configure(new System.IO.FileInfo(ConfigPath));
            getLogDir();
            try
            {
                foreach (log4net.Appender.IAppender Appender in log.Logger.Repository.GetAppenders())
                {
                    if (Appender.GetType() == typeof(log4net.Appender.AdoNetAppender))
                    {
                        log4net.Appender.AdoNetAppender DB = (log4net.Appender.AdoNetAppender)Appender;
                        StringBuilder lConnectionString = new StringBuilder();
                        lConnectionString.Append("Pooling=False;");
                        lConnectionString.Append("Use Compression=True;");
                        lConnectionString.Append("Encrypt=True;");
                        lConnectionString.Append("Allow Zero Datetime=True;");
                        lConnectionString.AppendFormat(" Database = agrologs;");
                        if (DataSource != String.Empty)
                            lConnectionString.AppendFormat(" Data Source = {0};", DataSource);
                        else
                            lConnectionString.AppendFormat(" Data Source = {0};", "db.agrobase.nl");
                        lConnectionString.AppendFormat(" User Id = rumat4cII;"); //TODO veranderen in logger wachtwoord
                         //TODO append password and..  veranderen in logger wachtwoord
                        DB.ConnectionString = lConnectionString.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDebug("configure db log" + ex.Message, ex);
            }
            log4net.GlobalContext.Properties["Versie"] = typeof(unLogger).Assembly.GetName().Version.ToString();

        }

        public static void FlushBuffers()
        {
            try
            {
                foreach (log4net.Appender.IAppender Appender in log.Logger.Repository.GetAppenders())
                {
                    if (typeof(log4net.Appender.BufferingAppenderSkeleton).IsAssignableFrom(Appender.GetType()))
                    {
                        log4net.Appender.BufferingAppenderSkeleton BufferedAppender = (log4net.Appender.BufferingAppenderSkeleton)Appender;
                        BufferedAppender.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDebug("Error Flushing Buffers" + ex.Message, ex);
            }
        }

        public static string getLogName()
        {
            string name = null;
            Assembly Entry = Assembly.GetEntryAssembly();
            if (Entry != null) name = Entry.GetName().Name;
            if (name == null) name = (String)AppDomain.CurrentDomain.GetData("AppName");
            if (name == null) name = "RUMAONLINE";
            return name;
        }

        public static bool SetThreadContext(String Key, String Value)
        {
            try
            {
                log4net.ThreadContext.Properties[Key] = Value;
                return true;
            }
            catch (Exception exc)
            {
                log.Warn(String.Format("SetThreadContext({0},{1})", Key, Value), exc);
                return false;
            }
        }

        public static bool SetThreadContext(Dictionary<String, Object> Collection)
        {
            try
            {
                foreach (KeyValuePair<String, Object> LogProperty in Collection)
                {
                    log4net.ThreadContext.Properties[LogProperty.Key] = LogProperty.Value;
                }
                return true;
            }
            catch (Exception exc)
            {
                log.Warn(exc.Message, exc);
                return false;
            }

        }
        public static Dictionary<String, Object> GetThreadContext()
        {
            Dictionary<String, Object> Result = new Dictionary<String, Object>();
            Result.Add("ClientIP", log4net.ThreadContext.Properties["ClientIP"]);
            Result.Add("ServerIP", log4net.ThreadContext.Properties["ServerIP"]);
            Result.Add("Browser", log4net.ThreadContext.Properties["Browser"]);
            Result.Add("Platform", log4net.ThreadContext.Properties["Platform"]);
            Result.Add("UserAgent", log4net.ThreadContext.Properties["UserAgent"]);
            Result.Add("Url", log4net.ThreadContext.Properties["Url"]);
            Result.Add("RequestType", log4net.ThreadContext.Properties["RequestType"]);
            Result.Add("ModuleStack", log4net.ThreadContext.Properties["ModuleStack"]);
            Result.Add("LogCode", log4net.ThreadContext.Properties["LogCode"]);
            Result.Add("FarmId", log4net.ThreadContext.Properties["FarmId"]);
            Result.Add("UbnId", log4net.ThreadContext.Properties["UbnId"]);
            Result.Add("ThrId", log4net.ThreadContext.Properties["ThrId"]);
            Result.Add("LogCode", log4net.ThreadContext.Properties["LogCode"]);
            Result.Add("AgroUser", log4net.ThreadContext.Properties["AgroUser"]);
            Result.Add("Versie", log4net.ThreadContext.Properties["Versie"]);
            return Result;
        }


        public static bool SetGlobalContext(String Key, String Value)
        {
            try
            {
                log4net.GlobalContext.Properties[Key] = Value;
                return true;
            }
            catch (Exception exc)
            {
                log.Warn(String.Format("SetGlobalContext({0},{1})", Key, Value), exc);
                return false;
            }
        }

        public static IDisposable AddStackMessage(string Message)
        {
            return log4net.ThreadContext.Stacks["ModuleStack"].Push(Message);
        }

        public static string RemoveLastStackMessage()
        {
            return log4net.ThreadContext.Stacks["ModuleStack"].Pop();
        }

        public static IDisposable AddModule(Modules module)
        {
            return log4net.ThreadContext.Stacks["ModuleDescr"].Push(module.ToString());
        }

        public static string RemoveModule()
        {
            return log4net.ThreadContext.Stacks["ModuleDescr"].Pop();
        }

        public static string getLogDir()
        {
            foreach (log4net.Appender.IAppender Appender in log.Logger.Repository.GetAppenders())
            {
                if (Appender.Name == "DefaultFile" || Appender.Name == "File")
                {
                    return Path.GetDirectoryName(((log4net.Appender.FileAppender)Appender).File) + Path.DirectorySeparatorChar;
                }
                else if (Appender is log4net.Appender.FileAppender || Appender is log4net.Appender.RollingFileAppender)
                {
                    return Path.GetDirectoryName(((log4net.Appender.FileAppender)Appender).File) + Path.DirectorySeparatorChar;
                }
            }
            return Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "log" + Path.DirectorySeparatorChar;
        }

        public static string getLogDir(String LogCategory)
        {
            string LogDir = getLogDir();
            if (!Directory.Exists(LogDir + LogCategory + Path.DirectorySeparatorChar))
            {
                try
                {
                    Directory.CreateDirectory(LogDir + LogCategory + Path.DirectorySeparatorChar);
                }
                catch { WriteError($@"{nameof(unLogger)} {nameof(getLogDir)} Can't CreateDirectory:{LogCategory}"); }
            }
            return LogDir + LogCategory + Path.DirectorySeparatorChar;
        }

        private static string StackInfo()
        {
            StackInfo(0, true);
            return "";
        }

        public static string StackInfo(int FrameNumber, bool AddParameters)
        {
            StringBuilder sbInfo = new StringBuilder("");
            if (log.IsDebugEnabled)
            {
                StackTrace stackTrace = new StackTrace(2);
                if (FrameNumber > stackTrace.FrameCount) FrameNumber = stackTrace.FrameCount;

                SetThreadContext("Module", stackTrace.GetFrame(FrameNumber).GetMethod().ReflectedType.FullName);
                sbInfo.Append(stackTrace.GetFrame(FrameNumber).GetMethod().ReflectedType.FullName);
                sbInfo.Append(".");
                sbInfo.Append(stackTrace.GetFrame(FrameNumber).GetMethod().Name);

                if (AddParameters)
                {
                    StringBuilder sbParam = new StringBuilder("(");
                    foreach (ParameterInfo Param in stackTrace.GetFrame(FrameNumber).GetMethod().GetParameters())
                    {
                        if (sbParam.ToString() != "(") sbParam.Append(",");
                        sbParam.Append(Param.ParameterType.Name + " " + Param.Name);
                    }
                    sbParam.Append(")");

                    sbInfo.Append(sbParam);

                    SetThreadContext("loc", sbInfo.ToString());
                    SetThreadContext("LogCode", stackTrace.GetFrame(0).GetMethod().MetadataToken.ToString());
                    SetThreadContext("stacktrace", stackTrace.ToString());
                }

            }
            else
            {
                SetThreadContext("loc", "");
            }
            return sbInfo.ToString();
        }

        #region Default

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteObject(Object Message)
        {
            StringBuilder sbWarn = new StringBuilder();
            sbWarn.Append("Object: " + Message.GetType().FullName);
            sbWarn.Append(Message + StackInfo());
            PropertyInfo[] lDataProperties;
            lDataProperties = Message.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                sbWarn.Append(propertyInfo.Name + " :" + propertyInfo.GetValue(Message, null));
            }
            log.Warn(sbWarn.ToString());

            if (OnLogMessage != null)
                OnLogMessage.Invoke(LogLevel.Warn, sbWarn.ToString());
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteObject(String Message, Object Object)
        {
            StringBuilder sbWarn = new StringBuilder();
            sbWarn.Append("##" + Message + StackInfo() + "#");
            sbWarn.Append(Message);
            PropertyInfo[] lDataProperties;
            lDataProperties = Object.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                sbWarn.Append(propertyInfo.Name + " :" + propertyInfo.GetValue(Object, null));
            }
            sbWarn.Append("#" + Message + "##");
            log.Warn(sbWarn.ToString());
            if (OnLogMessage != null)
                OnLogMessage.Invoke(LogLevel.Warn, sbWarn.ToString());
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteInfo(String Message)
        {
            if (log.IsInfoEnabled)
            {
                string msg = Message + StackInfo();
                log.Info(msg);
                if (OnLogMessage != null)
                    OnLogMessage.Invoke(LogLevel.Info, msg);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteInfo(String Message, Exception ex)
        {
            if (log.IsInfoEnabled)
            {
                string msg = Message + StackInfo();
                log.Info(msg, ex);
                if (OnLogMessage != null)
                    OnLogMessage.Invoke(LogLevel.Info, msg);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteDebug(String Message)
        {
            if (log.IsDebugEnabled)
            {
                string msg = Message + StackInfo();
                log.Debug(msg);
                if (OnLogMessage != null)
                    OnLogMessage.Invoke(LogLevel.Debug, msg);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteDebug(String Message, Exception ex)
        {
            if (log.IsDebugEnabled)
            {
                string msg = Message + StackInfo();
                log.Debug(msg, ex);
                if (OnLogMessage != null)
                    OnLogMessage.Invoke(LogLevel.Debug, msg);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteDebugObject(String Message, object Object)
        {
            StringBuilder sbDebug = new StringBuilder();

            sbDebug.Append("##" + Message + StackInfo() + "#");
            PropertyInfo[] lDataProperties;
            lDataProperties = Object.GetType().GetProperties();
            sbDebug.Append(Object.ToString());
            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                if (propertyInfo.GetValue(Object, null) != null)
                    sbDebug.Append(propertyInfo.Name + " : " + propertyInfo.GetValue(Object, null).ToString());
            }
            sbDebug.Append("#" + Message + "##");
            log.Debug(sbDebug.ToString());

            if (OnLogMessage != null)
                OnLogMessage.Invoke(LogLevel.Debug, sbDebug.ToString());
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteWarn(String Message)
        {
            string msg = Message + StackInfo();
            log.Warn(msg);
            if (OnLogMessage != null)
                OnLogMessage.Invoke(LogLevel.Warn, msg);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteWarn(String Message, Exception ex)
        {
            string msg = Message + StackInfo();
            log.Warn(msg, ex);
            if (OnLogMessage != null)
                OnLogMessage.Invoke(LogLevel.Warn, msg);
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteError(String Message)
        {
            string msg = Message + StackInfo();
            log.Error(msg);
            if (OnLogMessage != null)
                OnLogMessage.Invoke(LogLevel.Error, msg);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteError(String Message, Exception ex)
        {
            string msg = Message + StackInfo();
            log.Error(msg, ex);
            if (OnLogMessage != null)
                OnLogMessage.Invoke(LogLevel.Error, msg);
        }



        [MethodImpl(MethodImplOptions.Synchronized)]
        public void baseWriteError(String Message)
        {
            string msg = Message + StackInfo();
            log.Error(msg);
            if (OnLogMessage != null)
                OnLogMessage.Invoke(LogLevel.Error, msg);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void baseWriteError(String Message, Exception ex)
        {
            string msg = Message + StackInfo();
            log.Error(msg, ex);
            if (OnLogMessage != null)
                OnLogMessage.Invoke(LogLevel.Error, msg);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void baseWriteInfo(String Message)
        {
            if (log.IsInfoEnabled)
            {
                string msg = Message + StackInfo();
                log.Info(msg);
                if (OnLogMessage != null)
                    OnLogMessage.Invoke(LogLevel.Info, msg);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void baseWriteWarn(String Message)
        {
            string msg = Message + StackInfo();
            log.Warn(msg);
            
            if (OnLogMessage != null)
                OnLogMessage.Invoke(LogLevel.Warn, msg);
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void baseWriteDebug(String Message)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(Message + StackInfo());

                string msg = Message + StackInfo();
                log.Debug(msg);
                
                if (OnLogMessage != null)
                    OnLogMessage.Invoke(LogLevel.Debug, msg);
            }
        }


        #endregion




        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteObject(String Logger, String Message, Object Object)
        {
            ILog lLog = LogManager.GetLogger(Logger);
            StringBuilder sbWarn = new StringBuilder();
            sbWarn.Append("##" + Message + StackInfo() + "#");
            sbWarn.Append(Message);
            sbWarn.Append("#" + Message + "##");
            lLog.Warn(sbWarn.ToString());

            if (OnLogMessage != null)
                OnLogMessage.Invoke(LogLevel.Warn, sbWarn.ToString());
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteInfo(String Logger, String Message)
        {
            ILog lLog = LogManager.GetLogger(Logger);
            if (lLog.IsInfoEnabled)
            {
                string msg = Message + StackInfo();
                lLog.Info(msg);
                if (OnLogMessage != null)
                    OnLogMessage.Invoke(LogLevel.Info, msg);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteInfo(String Logger, String Message, Exception ex)
        {
            ILog lLog = LogManager.GetLogger(Logger);
            if (lLog.IsInfoEnabled)
            {
                string msg = Message + StackInfo();
                lLog.Info(msg, ex);
                if (OnLogMessage != null)
                    OnLogMessage.Invoke(LogLevel.Info, msg);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteDebug(String Logger, String Message)
        {
            ILog lLog = LogManager.GetLogger(Logger);
            if (lLog.IsDebugEnabled)
            {
                string msg = Message + StackInfo();
                lLog.Debug(msg);
                if (OnLogMessage != null)
                    OnLogMessage.Invoke(LogLevel.Debug, msg);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteDebug(String Logger, String Message, Exception ex)
        {
            ILog lLog = LogManager.GetLogger(Logger);
            if (lLog.IsDebugEnabled)
            {
                string msg = Message + StackInfo();
                lLog.Debug(msg, ex);
                if (OnLogMessage != null)
                    OnLogMessage.Invoke(LogLevel.Debug, msg);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteDebugObject(String Logger, String Message, object Object)
        {
            ILog lLog = LogManager.GetLogger(Logger);
            if (lLog.IsDebugEnabled)
            {
                StringBuilder sbDebug = new StringBuilder();

                sbDebug.Append("##" + Message + StackInfo() + "#");
                PropertyInfo[] lDataProperties;
                lDataProperties = Object.GetType().GetProperties();
                sbDebug.Append(Object.ToString());
                foreach (PropertyInfo propertyInfo in lDataProperties)
                {
                    if (propertyInfo.GetValue(Object, null) != null)
                        sbDebug.Append(propertyInfo.Name + " : " + propertyInfo.GetValue(Object, null).ToString());
                }
                sbDebug.Append("#" + Message + "##");
                lLog.Debug(sbDebug.ToString());
                if (OnLogMessage != null)
                    OnLogMessage.Invoke(LogLevel.Debug, sbDebug.ToString());
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteTrace(String Message)
        {
            if (log.IsDebugEnabled)
            {
                string msg = Message + StackInfo();
                
                log.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, log4net.Core.Level.Trace, Message, null);

                if (OnLogMessage != null)
                    OnLogMessage.Invoke(LogLevel.Trace, msg);
            }
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteTrace(String Logger, string message, Exception exception)
        {
            ILog lLog = LogManager.GetLogger(Logger);
            if (lLog.IsDebugEnabled)
            {
                lLog.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                    log4net.Core.Level.Trace, message, exception);

                if (OnLogMessage != null)
                    OnLogMessage.Invoke(LogLevel.Trace, message);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteTrace(String Logger, string message)
        {
            WriteTrace(Logger, message, null);
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteTraceFormat(string Logger, string format, params object[] args)
        {
            ILog lLog = LogManager.GetLogger(Logger);
            StackInfo();
            WriteTrace(Logger,String.Format(format, args));
        }



        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteWarn(String Logger, String Message)
        {
            ILog lLog = LogManager.GetLogger(Logger);
            string msg = Message + StackInfo();
            lLog.Warn(msg);
            if (OnLogMessage != null)
                OnLogMessage.Invoke(LogLevel.Warn, msg);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteWarn(String Logger, String Message, Exception ex)
        {
            ILog lLog = LogManager.GetLogger(Logger);
            string msg = Message + StackInfo();
            lLog.Warn(msg, ex);

            if (OnLogMessage != null)
                OnLogMessage.Invoke(LogLevel.Warn, msg);
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteError(String Logger, String Message)
        {
            ILog lLog = LogManager.GetLogger(Logger);            
            string msg = Message + StackInfo();
            lLog.Error(msg);
            if (OnLogMessage != null)
                OnLogMessage.Invoke(LogLevel.Error, msg);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteError(String Logger, String Message, Exception ex)
        {
            ILog lLog = LogManager.GetLogger(Logger);
            string msg = Message + StackInfo();
            lLog.Error(msg, ex);
            if (OnLogMessage != null)
                OnLogMessage.Invoke(LogLevel.Error, msg);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteFatal(String Logger, String Message)
        {
            ILog lLog = LogManager.GetLogger(Logger);
            string msg = Message + StackInfo();
            lLog.Fatal(msg);
            if (OnLogMessage != null)
                OnLogMessage.Invoke(LogLevel.Fatal, msg);
        }



        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteFatal(String Logger, String Message, Exception ex)
        {
            ILog lLog = LogManager.GetLogger(Logger);
            string msg = Message + StackInfo();
            lLog.Fatal(msg, ex);
            if (OnLogMessage != null)
                OnLogMessage.Invoke(LogLevel.Fatal, msg);
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteDebugFormat(string Logger, string format, params object[] args)
        {
            ILog lLog = LogManager.GetLogger(Logger);
            StackInfo();
            lLog.DebugFormat(format, args);

            if (OnLogMessage != null)
                OnLogMessage.Invoke(LogLevel.Debug, string.Format(format, args));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteInfoFormat(string Logger, string format, params object[] args)
        {
            ILog lLog = LogManager.GetLogger(Logger);
            StackInfo();
            lLog.InfoFormat(format, args);

            if (OnLogMessage != null)
                OnLogMessage.Invoke(LogLevel.Info, string.Format(format, args));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteWarnFormat(string Logger, string format, params object[] args)
        {
            ILog lLog = LogManager.GetLogger(Logger);
            StackInfo();
            lLog.WarnFormat(format, args);
            if (OnLogMessage != null)
                OnLogMessage.Invoke(LogLevel.Warn, string.Format(format, args));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteErrorFormat(string Logger, string format, params object[] args)
        {
            ILog lLog = LogManager.GetLogger(Logger);
            StackInfo();
            lLog.ErrorFormat(format, args);
            if (OnLogMessage != null)
                OnLogMessage.Invoke(LogLevel.Error, string.Format(format, args));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteFatalFormat(string Logger, string format, params object[] args)
        {
            ILog lLog = LogManager.GetLogger(Logger);
            StackInfo();
            lLog.FatalFormat(format, args);
        
            if (OnLogMessage != null)
                OnLogMessage.Invoke(LogLevel.Fatal, string.Format(format, args));
        }



        public enum Modules
        {
            Default = 1,
            Main = 2,
            Dierkaart = 3,
            IenRMelden = 4,

        }

    }
}
