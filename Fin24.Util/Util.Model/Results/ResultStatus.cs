using System;
using System.Runtime.Serialization;

namespace Fin24.Util.General.Container
{
   [DataContract]
   public enum ResultStatus
   {
      [EnumMember]
      Success,
      [EnumMember]
      Warning,
      [EnumMember]
      Failure
   }
}
