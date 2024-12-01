using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fin24.Util.General.Container
{
   public class BiDirectionalRegistry <L,R>
   {
      private readonly IDictionary<L, R> _leftToRight= new Dictionary<L, R>();
      private readonly IDictionary<R, L> _rightToLeft= new Dictionary<R, L>();
      private readonly object _syncLock = new object();

      //---------------------------------------------------------------------------------*---------/
      public void Add (L lhs, R rhs)
      {
         lock (_syncLock)
         {
            _leftToRight[lhs] = rhs;
            _rightToLeft[rhs] = lhs;
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public void Remove (L lhs)
      {
         R rhs;

         lock (_syncLock)
         {
            if (_leftToRight.TryGetValue( lhs, out rhs))
            {
               _leftToRight.Remove(lhs);

               if (_rightToLeft.ContainsKey( rhs))
               {
                  _rightToLeft.Remove(rhs);
               }
            }
            
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public void Remove(R rhs)
      {
         L lhs;

         lock (_syncLock)
         {
            if (_rightToLeft.TryGetValue(rhs, out lhs))
            {
               _rightToLeft.Remove(rhs);

               if (_leftToRight.ContainsKey(lhs))
               {
                  _leftToRight.Remove(lhs);
               }
            }

         }
      }

      //---------------------------------------------------------------------------------*---------/
      public L this[R rhs]
      {
         get
         {
            lock (_syncLock)
            {
               return _rightToLeft[rhs];

            }
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public R this [L lhs]
      {
         get
         {
            lock (_syncLock)
            {
               return _leftToRight[lhs];
            }
         }
      }

   }
}