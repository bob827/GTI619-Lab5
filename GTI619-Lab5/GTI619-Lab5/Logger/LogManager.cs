using System;

namespace GTI619_Lab5.Logger
{
    public static class LogManager
    {
        public static ILogger GetLogger(string name)
        {
            return new Logger(log4net.LogManager.GetLogger(name));
        }

        public static ILogger GetLogger(Type type)
        {
            return new Logger(log4net.LogManager.GetLogger(type));
        }
    }
}