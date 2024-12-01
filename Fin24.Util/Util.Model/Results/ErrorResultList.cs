using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Util.Model.Results
{
   [DataContract]
   public partial class ErrorResultList
   {
      [DataMember]
      public IList<Error> ErrorList= new List<Error>();
   }
}