using System;
using System.Collections.Generic;

namespace Fin24.Util.General.Misc
{
   public class StringUtil
   {
      //---------------------------------------------------------------------------------*---------/
      public static IDictionary<string,string> AsDictionary (string val)
      {
          if (String.IsNullOrEmpty(val))
              return new Dictionary<string, string>();

         string[] paramList = val.Split(';');

         IDictionary<string,string> result= new Dictionary<string, string>();

         foreach (string paramEntry in paramList)
         {
            string[] keyValue = paramEntry.Split('=');
            result[keyValue[0]] = keyValue[1];
         }

         return result;
      }
   }
}
