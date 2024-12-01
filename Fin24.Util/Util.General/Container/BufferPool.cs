using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Fin24.Util.General.Container
{
   public class BufferPool
   {
      private readonly int _numBytes;                 // the total number of bytes controlled by the buffer pool
      private readonly byte[] _buffer;                // the underlying byte array maintained by the Buffer Manager
      private readonly Stack<int> _freeIndexPool;     // 
      private int _currentIndex;
      private readonly int _bufferSize;

      //---------------------------------------------------------------------------------*---------/
      public BufferPool(int numberOfBuffers, int bufferSize)
      {
         if (numberOfBuffers <= 0)
         {
            throw new ArgumentException("Total size of buffer must be specified");
         }

         if (bufferSize <= 0)
         {
            throw new ArgumentException("Size of buffer blocks must be specified");
         }

         _numBytes = numberOfBuffers * bufferSize;
         _currentIndex = 0;
         _bufferSize = bufferSize;
         _freeIndexPool = new Stack<int>();
         _buffer = new byte[_numBytes];
      }

      //---------------------------------------------------------------------------------*---------/
      public bool SetBuffer(SocketAsyncEventArgs args)
      {

         if (_freeIndexPool.Count > 0)
         {
            args.SetBuffer(_buffer, _freeIndexPool.Pop(), _bufferSize);
         }
         else
         {
            if ((_numBytes - _bufferSize) < _currentIndex)
            {
               return false;
            }
            args.SetBuffer(_buffer, _currentIndex, _bufferSize);
            _currentIndex += _bufferSize;
         }
         return true;
      }

      //---------------------------------------------------------------------------------*---------/
      public void FreeBuffer(SocketAsyncEventArgs args)
      {
         _freeIndexPool.Push(args.Offset);
         args.SetBuffer(null, 0, 0);
      }

   }
}