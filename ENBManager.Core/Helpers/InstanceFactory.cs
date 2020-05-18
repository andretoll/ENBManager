using System;

namespace ENBManager.Core.Helpers
{
    public static class InstanceFactory
    {
        public static object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}
