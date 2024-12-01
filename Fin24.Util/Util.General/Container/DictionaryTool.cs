using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fin24.Util.General.Container
{
   public class DictionaryTool
   {
      public static IList<T> Get<K,T> (K key, IDictionary<K, IList<T>> container)
      {
         IList<T> result;

         if (!container.TryGetValue( key, out result))
         {
            result= new List<T>();
            container.Add( key, result);
         }

         return result;
      }
   }
}