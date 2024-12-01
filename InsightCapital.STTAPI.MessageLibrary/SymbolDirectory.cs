using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ProtoBuf;

namespace InsightCapital.STTAPI.MessageLibrary
{
    /// <summary>
    /// Author :- Rohit Kumar
    /// Symbol Directory Message
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1), ProtoContract, Serializable]
    public struct SymbolDirectory : IMessage
    {

        [ProtoMember(1, IsRequired = true)]
        public MessageHeader messageHeader;
        [ProtoMember(2, IsRequired = true)]
        public uint Nanosecond;
        [ProtoMember(3, IsRequired = true)]
        public uint InstrumentID;
        [ProtoMember(4, IsRequired = true)]
        public char Res1;
        [ProtoMember(5, IsRequired = true)]
        public char Res2;
        [ProtoMember(6, IsRequired = true)]
        public char SymbolStatus;
        [ProtoMember(7, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public char[] ISIN;
        [ProtoMember(8, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x19)]
        public char[] Symbol;
        [ProtoMember(9, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public char[] TIDM;
        [ProtoMember(10, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public char[] Segment;
        [ProtoMember(11, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] PreviousClosePrice;
        [ProtoMember(12, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public char[] ExpirationDate;
        [ProtoMember(13, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x19)]
        public char[] Underlying;
        [ProtoMember(14, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] StrikePrice;
        [ProtoMember(15, IsRequired = true)]
        public char OptionType;
        [ProtoMember(0x10, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public char[] Issuer;
        [ProtoMember(0x11, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public char[] IssueDate;
        [ProtoMember(0x12, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Coupon;
        [ProtoMember(0x13, IsRequired = true), MarshalAs(UnmanagedType.I1)]
        public byte Flags;
        [ProtoMember(20, IsRequired = true), MarshalAs(UnmanagedType.I1)]
        public byte SubBook;
        [ProtoMember(0x15, IsRequired = true), MarshalAs(UnmanagedType.ByValArray, SizeConst = 0xbd)]
        public char[] CorporateAction;
        public static int Length => Marshal.SizeOf(typeof(SymbolDirectory));

        public string GetMessageString()
        {
            return this.ToString();
        }

        public override string ToString()
        {
            return "Message Type :- SymbolDirectory" + " Nanosecond :-" + Nanosecond.ToString()
                     + " InstrumentID :-" + InstrumentID.ToString().Trim() + " Res1 :-" + Res1
                     + " Res2 :-" + Res2 + " SymbolStatus :-" + SymbolStatus
                     + " ISIN :-" + new string(ISIN) + " Symbol :-" + new string(Symbol) + " TIDM :-" + new string(TIDM).Trim()
                     + " Segment :-" + new string(Segment).Trim()
                     + " PreviousClosePrice :-" + JSEConverter.ConvertPriceToDouble(PreviousClosePrice)
                     + " ExpirationDate :-" + new string(ExpirationDate)
                     + " Underlying :-" + new string(Underlying).Trim()
                     + " StrikePrice :-" + JSEConverter.ConvertPriceToDouble(StrikePrice)
                     + " OptionType :-" + OptionType + " Issuer :-" + new string(Issuer) + " IssueDate :-" + new string(IssueDate)
                     + " Coupon :-" + JSEConverter.ConvertPriceToDouble(Coupon) + " Flags :-" + Flags
                     + " SubBook :-" + JSEConverter.GetStringValue(JSEConverter.GetSymbolDirectorySubBook(SubBook))
                     + " CorporateAction :-" + new string(CorporateAction).Trim();
        }
    }
}

