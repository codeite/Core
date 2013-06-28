using System;

namespace Codeite.Core.Activation
{
    public static class ActivatorHelper
    {
        public static T CreateInstance<T>(params object[] args) where T : class
        {
            return Activator.CreateInstance(typeof (T), args) as T;
        }
    }
}