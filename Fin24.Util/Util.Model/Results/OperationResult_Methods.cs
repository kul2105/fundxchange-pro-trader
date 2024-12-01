using System;
using Fin24.Util.General.Container;

namespace Util.Model.Results
{
   public partial class OperationResult<T>
   {
      //---------------------------------------------------------------------------------*---------/
      public OperationResult ()
      {
         ErrorResult = new ErrorResultList();
      }

      //---------------------------------------------------------------------------------*---------/
      public OperationResult (T result) : this ()
      {
         Result = result;
         Status= ResultStatus.Success;
      }

      //---------------------------------------------------------------------------------*---------/
      public static implicit operator T (OperationResult<T> realObj)
      {
         return realObj.Result;
      }
   }
}