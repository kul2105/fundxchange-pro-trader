using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightCapital.STTAPI.MessageLibrary
{
    public static class ExtensionMethods
    {
        public static T GetEnum<T>(this byte source)
        {
            return (T)Enum.ToObject(typeof(T), source);
        }

        public static T GetEnum<T>(this char source)
        {
            return (T)Enum.ToObject(typeof(T), source);
        }
    }
}
