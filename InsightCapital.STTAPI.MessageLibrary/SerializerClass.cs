using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace InsightCapital.STTAPI.MessageLibrary
{
    public class SerializerClass
    {
        public static T ConvertFromBufferToStruct<T>(byte[] pBuffer, int pOffset, int pSize) =>
            ((T)ConvertFromBufferToStruct(pBuffer, pOffset, pSize, typeof(T)));

        public static object ConvertFromBufferToStruct(byte[] pBuffer, int pOffset, int pSize, Type pType)
        {
            object obj2;
            IntPtr zero = IntPtr.Zero;
            try
            {
                zero = Marshal.AllocHGlobal(Marshal.SizeOf(pType));
                Marshal.Copy(pBuffer, pOffset, zero, Math.Min(pSize, pBuffer.Length - pOffset));
                obj2 = Marshal.PtrToStructure(zero, pType);
            }
            finally
            {
                if (zero != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(zero);
                }
            }
            return obj2;
        }

        public static void ConvertToUnmanagedMemory<T>(int pSize, T pRecord, byte[] pBuffer)
        {
            int cb = pSize;
            IntPtr ptr = Marshal.AllocHGlobal(cb);
            Marshal.StructureToPtr<T>(pRecord, ptr, true);
            Marshal.Copy(ptr, pBuffer, 0, Math.Min(cb, pBuffer.Length));
            Marshal.FreeHGlobal(ptr);
        }

        public static T DeserializeMe<T>(byte[] bytes)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    return Serializer.Deserialize<T>(stream);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }

        public static byte[] SerializeMe<T>(T obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize<T>(stream, obj);
                return stream.ToArray();
            }
        }
    }
}
