using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace PressPlay.FFWD
{
    internal static class SendMessageMethodCache
    {
        private static Dictionary<Type, Dictionary<string, MethodInfo>> cache = new Dictionary<Type, Dictionary<string, MethodInfo>>();

        internal static MethodInfo GetCachedMethod(this Type tp, string methodName, BindingFlags flags)
        {
            if (!cache.ContainsKey(tp))
            {
                cache.Add(tp, new Dictionary<string, MethodInfo>());
            }
            if (cache[tp].ContainsKey(methodName))
            {
                return cache[tp][methodName];
            }
            MethodInfo info = tp.GetMethod(methodName, flags);
            cache[tp][methodName] = info;
            return info;
        }
    }
}
