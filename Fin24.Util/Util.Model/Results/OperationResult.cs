using System;
using System.Runtime.Serialization;
using Fin24.Util.General.Container;

namespace Util.Model.Results
{
   [DataContract]
   public partial class OperationResult<T>
   {
      [DataMember]
      public ResultStatus Status{ get; set; }
      [DataMember]
      public ErrorResultList ErrorResult { get; set; }
      [DataMember]
      public T Result { get; set; }
   }
}