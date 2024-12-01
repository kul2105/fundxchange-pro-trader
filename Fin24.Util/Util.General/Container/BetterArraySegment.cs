using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fin24.Util.General.Container
{
    public class BetterArraySegment<T>
    {
        private T[] _source;
        private int _offset;
        private int _count;

        //---------------------------------------------------------------------------------*---------/
        public BetterArraySegment(T[] source, int offset, int count)
        {
            _source = source;
            _offset = offset;
            _count = count;
        }

        //---------------------------------------------------------------------------------*---------/
        public T[] Buffer
        {
            get
            {
                return _source;
            }
        }

        //---------------------------------------------------------------------------------*---------/
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        //---------------------------------------------------------------------------------*---------/
        public int Length
        {
            get
            {
                return _count;
            }
        }

        //---------------------------------------------------------------------------------*---------/
        public bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        //---------------------------------------------------------------------------------*---------/
        public T this[int index]
        {
            get
            {
                if (index > _count)
                {
                    throw new IndexOutOfRangeException();
                }

                return _source[_offset + index];
            }
            set
            {
                _source[_offset + index] = value;
            }
        }
    }
}