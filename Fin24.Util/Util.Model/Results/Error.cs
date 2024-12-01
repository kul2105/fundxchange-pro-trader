using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Util.Model.Results
{
   [DataContract]
   public class Error
   {
      [DataMember (Order=1)]
      public int Code { get; set;}
      [DataMember(Order = 2)]
      public string Message { get; set; }
   }
}