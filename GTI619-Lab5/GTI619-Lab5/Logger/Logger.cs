using System;
using log4net;

namespace GTI619_Lab5.Logger
{
    internal class Logger : ILogger
    {
        private ILog Log { get; set; }

        public Logger(ILog log)
        {
            Log = log;
        }

        public bool IsDebugEnabled { get { return Log.IsDebugEnabled; } }
        public bool IsInfoEnabled { get { return Log.IsInfoEnabled; } }
        public bool IsWarnEnabled { get { return Log.IsWarnEnabled; } }
        public bool IsErrorEnabled { get { return Log.IsErrorEnabled; } }
        public bool IsFatalEnabled { get { return Log.IsFatalEnabled; } }

        public void Debug(string message, Exception exception = null)
        {
            if(IsDebugEnabled)
                Log.Debug(message, exception);
        }

        public void Info(string message, Exception exception = null)
        {
            if (IsInfoEnabled)
                Log.Info(message, exception);
        }

        public void Warn(string message, Exception exception = null)
        {
            if (IsWarnEnabled)
                Log.Warn(message, exception);
        }

        public void Error(string message, Exception exception = null)
        {
            if (IsErrorEnabled)
                Log.Error(message, exception);
        }

        public void Fatal(string message, Exception exception = null)
        {
            if (IsFatalEnabled)
                Log.Fatal(message, exception);
        }
    }
}