using NLog;
using System;

namespace ENBManager.Core.Helpers
{
    public static class InstanceFactory
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public static object CreateInstance(Type type)
        {
            _logger.Debug($"Creating instance of type {type.Name}.");
            return Activator.CreateInstance(type);
        }
    }
}
