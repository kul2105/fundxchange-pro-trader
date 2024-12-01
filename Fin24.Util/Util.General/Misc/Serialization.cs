using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Fin24.Util.General.Misc
{
   public class Serialization
   {
      //---------------------------------------------------------------------------------*---------/
      public static byte[] Serialize(object source)
      {
         using (MemoryStream stream = new MemoryStream())
         {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, source);

            return stream.ToArray();
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public static T Deserialize<T>(byte[] binData)
      {
         using (MemoryStream stream = new MemoryStream())
         {
            stream.Write(binData, 0, binData.Length);
            stream.Seek(0, SeekOrigin.Begin);

            BinaryFormatter formatter = new BinaryFormatter();
            return (T)formatter.Deserialize(stream);
         }
      }      
   }
}
