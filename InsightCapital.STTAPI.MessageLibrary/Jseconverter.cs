using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace InsightCapital.STTAPI.MessageLibrary
{
    public class JSEConverter
    {
        private static string baseChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const long NormalConstant = 0x5f5e100L;
        private const long VolatilityConstant = 0xf4240L;
        private const long TurnoverConstant = 0x2710L;

        private static bool CheckFlagSet(byte bytes, int value) =>
            ((bytes & value) == value);

        private static unsafe long CheckNegative(byte[] bytes)
        {
            bool flag = false;
            if ((bytes[bytes.Length - 1] & 0x80) == 0x80)
            {
                // byte* numPtr1 = &(bytes[bytes.Length - 1]);
                //numPtr1[0] = (byte)(numPtr1[0] ^ 0x80);

                fixed (byte* numPtr1 = &(bytes[bytes.Length - 1]))
                {
                    numPtr1[0] = (byte)(numPtr1[0] ^ 0x80);
                }

                flag = true;
            }
            long num = BitConverter.ToInt64(bytes, 0);
            if (flag)
            {
                num *= -1L;
            }
            return num;
        }

        public static long ConvertOrderID(string input)
        {
            input = input.Substring(1, input.Length - 1);
            int length = baseChars.Length;
            long num2 = 0L;
            char[] array = input.ToCharArray();
            Array.Reverse(array);
            string str = new string(array);
            for (int i = 0; i < str.Length; i++)
            {
                int index = baseChars.IndexOf(str[i]);
                num2 += index * ((long)Math.Pow((double)length, (double)i));
            }
            return num2;
        }

        public static string ConvertOrderID(ulong orderid)
        {
            string str = string.Empty;
            uint num = 0x3e;
            while (true)
            {
                str = $"{baseChars[(int)(orderid % ((ulong)num))]}{str}";
                orderid /= (ulong)num;
                if (orderid <= 0L)
                {
                    return str.PadLeft(11, '0');
                }
            }
        }

        public static decimal ConvertPriceToDecimal(byte[] OriginalBytes)
        {
            byte[] bytes = CreateNewByteArray(OriginalBytes);
            decimal num = 0M;
            if ((bytes != null) && (bytes.Length != 0))
            {
                num = CheckNegative(bytes) / 100000000M;
            }
            return num;
        }

        public static unsafe byte[] ConvertPriceToDecimal(decimal price)
        {
            byte[] bytes = new byte[8];
            if (price >= 0M)
            {
                bytes = BitConverter.GetBytes((double)price);
            }
            else
            {
                decimal decimal1 = Math.Abs(price);
                price = decimal1;
                bytes = BitConverter.GetBytes((double)price);
                //byte* numPtr1 = &(bytes[bytes.Length - 1]);
                //numPtr1[0] = (byte)(numPtr1[0] ^ 0x80);
                fixed (byte* numPtr1 = &(bytes[bytes.Length - 1]))
                {
                    numPtr1[0] = (byte)(numPtr1[0] ^ 0x80);
                }
            }
            return bytes;
        }

        public static double ConvertPriceToDouble(byte[] OriginalBytes)
        {
            byte[] bytes = CreateNewByteArray(OriginalBytes);
            double num = 0.0;
            if ((bytes != null) && (bytes.Length != 0))
            {
                num = ((double)CheckNegative(bytes)) / 100000000.0;
            }
            return num;
        }

        public static unsafe byte[] ConvertPriceToDouble(double price)
        {
            byte[] bytes = new byte[8];
            if (price >= 0.0)
            {
                bytes = BitConverter.GetBytes(price);
            }
            else
            {

                double num1 = Math.Abs(price);
                price = num1;
                bytes = BitConverter.GetBytes(price);
                // byte* numPtr1 = &(bytes[bytes.Length - 1]);
                // numPtr1[0] = (byte)(numPtr1[0] ^ 0x80);
                fixed (byte* numPtr1 = &(bytes[bytes.Length - 1]))
                {
                    numPtr1[0] = (byte)(numPtr1[0] ^ 0x80);
                }
            }
            return bytes;
        }

        public static long ConvertPriceToLong(byte[] OriginalBytes)
        {
            byte[] bytes = CreateNewByteArray(OriginalBytes);
            long num = 0L;
            if ((bytes != null) && (bytes.Length != 0))
            {
                num = CheckNegative(bytes);
            }
            return num;
        }

        public static unsafe byte[] ConvertPriceToLong(long price)
        {
            byte[] bytes = new byte[8];
            if (price >= 0L)
            {
                bytes = BitConverter.GetBytes(price);
            }
            else
            {
                long num1 = Math.Abs(price);
                price = num1;
                bytes = BitConverter.GetBytes(price);

                fixed (byte* numPtr1 = &(bytes[bytes.Length - 1]))
                {
                    numPtr1[0] = (byte)(numPtr1[0] ^ 0x80);
                }

                //byte* numPtr1 = &(bytes[bytes.Length - 1]);

                //numPtr1[0] = (byte) (numPtr1[0] ^ 0x80);
            }
            return bytes;
        }

        public static long ConvertTradeID(string input)
        {
            input = input.Substring(1, input.Length - 1);
            int length = baseChars.Length;
            long num2 = 0L;
            char[] array = input.ToCharArray();
            Array.Reverse(array);
            string str = new string(array);
            for (int i = 0; i < str.Length; i++)
            {
                int index = baseChars.IndexOf(str[i]);
                num2 += index * ((long)Math.Pow((double)length, (double)i));
            }
            return num2;
        }

        public static string ConvertTradeID(ulong tradeid)
        {
            string str = string.Empty;
            uint num = 0x3e;
            while (true)
            {
                str = $"{baseChars[(int)(tradeid % ((ulong)num))]}{str}";
                tradeid /= (ulong)num;
                if (tradeid <= 0L)
                {
                    return str.PadLeft(10, 'T');
                }
            }
        }

        public static decimal ConvertTurnoverToDecimal(byte[] OriginalBytes)
        {
            byte[] bytes = CreateNewByteArray(OriginalBytes);
            decimal num = 0M;
            if ((bytes != null) && (bytes.Length != 0))
            {
                num = CheckNegative(bytes) / 10000M;
            }
            return num;
        }

        public static double ConvertTurnoverToDouble(byte[] OriginalBytes)
        {
            byte[] bytes = CreateNewByteArray(OriginalBytes);
            double num = 0.0;
            if ((bytes != null) && (bytes.Length != 0))
            {
                num = ((double)CheckNegative(bytes)) / 10000.0;
            }
            return num;
        }

        public static decimal ConvertVolatilityToDecimal(byte[] OriginalBytes)
        {
            byte[] bytes = CreateNewByteArray(OriginalBytes);
            decimal num = 0M;
            if ((bytes != null) && (bytes.Length != 0))
            {
                num = CheckNegative(bytes) / 1000000M;
            }
            return num;
        }

        public static double ConvertVolatilityToDouble(byte[] OriginalBytes)
        {
            byte[] bytes = CreateNewByteArray(OriginalBytes);
            double num = 0.0;
            if ((bytes != null) && (bytes.Length != 0))
            {
                num = ((double)CheckNegative(bytes)) / 1000000.0;
            }
            return num;
        }

        private static byte[] CreateNewByteArray(byte[] bytes)
        {
            byte[] destinationArray = new byte[bytes.Length];
            Array.Copy(bytes, 0, destinationArray, 0, bytes.Length);
            return destinationArray;
        }

        public static List<AddOrderFlags> GetAddOrderFlags(byte bytes)
        {
            List<AddOrderFlags> list = new List<AddOrderFlags>();
            foreach (AddOrderFlags flags in System.Enum.GetValues(typeof(AddOrderFlags)).Cast<AddOrderFlags>())
            {
                if (flags == AddOrderFlags.Normal)
                {
                    continue;
                }
                if (CheckFlagSet(bytes, (int)flags))
                {
                    list.Add(flags);
                }
            }
            if (list.Count == 0)
            {
                list.Add(AddOrderFlags.Normal);
            }
            return list;
        }

        public static DateTime GetDateTimeFromArray(char[] array)
        {
            string source = new string(array);
            return (!source.Contains<char>('-') ? DateTime.ParseExact(source, "dd/MM/yyyy HH:mm:ss.ff", CultureInfo.InvariantCulture) : DateTime.ParseExact(source, "dd-MM-yyyy HH:mm:ss.ff", CultureInfo.InvariantCulture));
        }

        public static char[] GetDateTimeStringFromArray(char[] array)
        {
            TimeSpan span = TimeSpan.Parse(new string(array));
            DateTime time = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, span.Hours, span.Minutes, span.Seconds);
            return time.ToString("dd/MM/yyyy HH:mm:ss.ff").ToCharArray();
        }

        public static DateTime GetJSEDateTime(uint Nanoseconds, TimeSpan timeSpan)
        {
            TimeSpan span = new TimeSpan();
            span = timeSpan;
            span.Add(TimeSpan.FromTicks((long)(((ulong)Nanoseconds) / ((long)100))));
            return DateTime.Today.Add(span);
        }

        public static List<OrderModifiedFlag> GetOrderModifiedFlags(byte bytes)
        {
            List<OrderModifiedFlag> list = new List<OrderModifiedFlag>();
            foreach (OrderModifiedFlag flag in System.Enum.GetValues(typeof(OrderModifiedFlag)).Cast<OrderModifiedFlag>())
            {
                if (flag == OrderModifiedFlag.PriorityLost)
                {
                    continue;
                }
                if (CheckFlagSet(bytes, (int)flag))
                {
                    list.Add(flag);
                }
            }
            if (list.Count == 0)
            {
                list.Add(OrderModifiedFlag.PriorityLost);
            }
            return list;
        }

        public static List<SnapshotCompleteSubBook> GetSnapshotCompleteSubBook(byte bytes)
        {
            List<SnapshotCompleteSubBook> list = new List<SnapshotCompleteSubBook>();
            foreach (SnapshotCompleteSubBook book in System.Enum.GetValues(typeof(SnapshotCompleteSubBook)).Cast<SnapshotCompleteSubBook>())
            {
                if (book == SnapshotCompleteSubBook.Normal)
                {
                    continue;
                }
                if (CheckFlagSet(bytes, (int)book))
                {
                    list.Add(book);
                }
            }
            if (list.Count == 0)
            {
                list.Add(SnapshotCompleteSubBook.Normal);
            }
            return list;
        }

        public static string GetStringValue<T>(List<T> list)
        {
            string str = "";
            for (int i = 0; i < list.Count; i++)
            {
                str = str + list[i].ToString();
                if (i != (list.Count - 1))
                {
                    str = str + ",";
                }
            }
            return str;
        }

        public static List<SymbolDirectoryFlags> GetSymbolDirectoryFlags(byte bytes)
        {
            List<SymbolDirectoryFlags> list = new List<SymbolDirectoryFlags>();
            foreach (SymbolDirectoryFlags flags in System.Enum.GetValues(typeof(SymbolDirectoryFlags)).Cast<SymbolDirectoryFlags>())
            {
                if (flags == SymbolDirectoryFlags.NormalOrderBook)
                {
                    continue;
                }
                if (CheckFlagSet(bytes, (int)flags))
                {
                    list.Add(flags);
                }
            }
            if (list.Count == 0)
            {
                list.Add(SymbolDirectoryFlags.NormalOrderBook);
            }
            return list;
        }

        public static List<SymbolDirectorySubBook> GetSymbolDirectorySubBook(byte bytes)
        {
            List<SymbolDirectorySubBook> list = new List<SymbolDirectorySubBook>();
            foreach (SymbolDirectorySubBook book in System.Enum.GetValues(typeof(SymbolDirectorySubBook)).Cast<SymbolDirectorySubBook>())
            {
                if (book == SymbolDirectorySubBook.None)
                {
                    continue;
                }
                if (CheckFlagSet(bytes, (int)book))
                {
                    list.Add(book);
                }
            }
            if (list.Count == 0)
            {
                list.Add(SymbolDirectorySubBook.None);
            }
            return list;
        }

        public static List<TopOfBookFlags> GetTopOfBookFlags(byte bytes)
        {
            List<TopOfBookFlags> list = new List<TopOfBookFlags>();
            foreach (TopOfBookFlags flags in System.Enum.GetValues(typeof(TopOfBookFlags)).Cast<TopOfBookFlags>())
            {
                if (flags == TopOfBookFlags.Normal)
                {
                    continue;
                }
                if (CheckFlagSet(bytes, (int)flags))
                {
                    list.Add(flags);
                }
            }
            if (list.Count == 0)
            {
                list.Add(TopOfBookFlags.Normal);
            }
            return list;
        }

        public static List<TradeFlags> GetTradeFlags(byte bytes)
        {
            List<TradeFlags> list = new List<TradeFlags>();
            foreach (TradeFlags flags in System.Enum.GetValues(typeof(TradeFlags)).Cast<TradeFlags>())
            {
                if (flags == TradeFlags.Normal)
                {
                    continue;
                }
                if (CheckFlagSet(bytes, (int)flags))
                {
                    list.Add(flags);
                }
            }
            if (list.Count == 0)
            {
                list.Add(TradeFlags.Normal);
            }
            return list;
        }

        public static string RetrieveIPAddress()
        {
            foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return address.ToString();
                }
            }
            return string.Empty;
        }
    }
}
