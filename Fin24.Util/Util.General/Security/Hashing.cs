using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Fin24.Util.General.Security
{
   public class Hashing
   {
      //---------------------------------------------------------------------------------*---------/
      public static long CreateShaHash(string text, string salt)
      {
         salt = salt ?? "";
         SHA512Managed hashAlgo = new SHA512Managed();
         byte[] contentBytes = Encoding.UTF8.GetBytes(string.Concat(text, salt));
         byte[] hashBytes = hashAlgo.ComputeHash(contentBytes);
         hashAlgo.Clear();
         string hashString= Convert.ToBase64String(hashBytes);

         return long.Parse(hashString, NumberStyles.AllowHexSpecifier);
      }
   }
}
