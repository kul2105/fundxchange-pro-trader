using System;

namespace Util.Model.Results
{
   public partial class ErrorResultList
   {
      //---------------------------------------------------------------------------------*---------/
      public bool HaveErrors
      {
         get
         {
            return ErrorList.Count > 0;
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public Error FirstError
      {
         get
         {
            if (!HaveErrors)
            {
               return null;
            }

            return ErrorList[0];
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public void Add (int errorCode, string msgFormat, params string []msgParams)
      {
         Add( new Error{Code=errorCode, Message = string.Format( msgFormat, msgParams)});
      }

      //---------------------------------------------------------------------------------*---------/
      public void Add (Error error)
      {
         ErrorList.Add( error);
      }

      //---------------------------------------------------------------------------------*---------/
      public void Add (ErrorResultList errors)
      {
         foreach (Error error in errors.ErrorList)
         {
            ErrorList.Add( error);
         }
      }
   }
}