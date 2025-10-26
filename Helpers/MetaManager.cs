using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InfernumMultiplayer.Helpers
{
    public class MetaManager
    {
        private static ClassSpreader _spreader = new ClassSpreader();

        public static Type? getType(String path)
        {
            if (!_spreader.GettedTypes.ContainsKey(path))
            {
                Type? type = _spreader.InfernumMod.Code?.GetType(path);
                if (type == null)
                {
                    return null;
                }
                _spreader.GettedTypes.Add(path, type);
            }

            return _spreader.GettedTypes[path];
        }

        public static MethodInfo? GetMethodInfo(String typePath, String methodName, 
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (!_spreader.GettedTypes.ContainsKey(typePath))
            {
                Type? type = _spreader.InfernumMod.Code?.GetType(typePath);
                if (type == null)
                {
                    return null;
                }
                _spreader.GettedTypes.Add(typePath, type);
            }

            return _spreader.GettedTypes[typePath].GetMethod(methodName, bindingFlags);
        }

    }
}
