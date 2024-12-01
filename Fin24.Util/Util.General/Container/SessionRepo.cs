using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fin24.Util.General.Container;

namespace Fin24.Util.Container
{
   //TODO Delete this class
   public class SessionRepo<T>
   {
      private readonly IDictionary<T, ISession> m_sessionList= new Dictionary<T, ISession>();

      //---------------------------------------------------------------------------------*---------/
      public ISession Get (T key)
      {
         ISession session;
         if (!m_sessionList.TryGetValue(key, out session))
         {
            session = CreateSession();
            m_sessionList.Add( key, session);
         }

         return session;
      }

      //---------------------------------------------------------------------------------*---------/
      public void Remove (T key)
      {
         if (m_sessionList.ContainsKey( key))
         {
            m_sessionList.Remove(key);
         }
      }
      //---------------------------------------------------------------------------------*---------/
      protected virtual ISession CreateSession()
      {
         return new DefaultSession();
      }

      //---------------------------------------------------------------------------------*---------/

   }
}
