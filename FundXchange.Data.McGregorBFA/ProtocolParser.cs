using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Fin24.LiveData.Common.MarketModel.MarketMessages;
using Fin24.Util.General.Container;
using log4net;
using ProtoBuf;

namespace FundXchange.Data.McGregorBFA
{
    public class ProtocolParser
    {
        private const PrefixStyle MSG_PREFIX_STYLE = PrefixStyle.Base128;
        private const char LENGTH_HEADER_SEP = '|';
        private readonly IList<byte> _dataLengthAsBytes = new List<byte>();
        private byte[] _dataBody;
        private int _bodyIdx = 0;
        private int _dataLength = -1;
        private readonly object _syncLock = new object();
        private bool _haveLengthData;

        private static readonly ILog LOG = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //---------------------------------------------------------------------------------*---------/
        public static byte[] SerializeWithWrapper(object data)
        {
            MemoryStream dataAsStream = new MemoryStream();
            Serializer.NonGeneric.Serialize(dataAsStream, data);
            byte[] dataAsBytes = dataAsStream.ToArray();

            byte[] serializedMessage = WrapMessage(ProtocolTypeMapper.GetCodeForType(data.GetType()), dataAsBytes);

            return serializedMessage;
        }

        //---------------------------------------------------------------------------------*---------/
        private static byte[] WrapMessage(int messageType, byte[] dataBytes)
        {
            MessageWrapper msg = new MessageWrapper();
            msg.MessageType = messageType;
            msg.Payload = dataBytes;

            MemoryStream messageStream = new MemoryStream();
            Serializer.NonGeneric.Serialize(messageStream, msg);

            MemoryStream wrappedStream = new MemoryStream();

            byte[] messageLength = GetMessageLength(messageStream);
            wrappedStream.Write(messageLength, 0, messageLength.Length);
            wrappedStream.WriteByte((byte)LENGTH_HEADER_SEP);
            messageStream.WriteTo(wrappedStream);

            return wrappedStream.ToArray();
        }

        //---------------------------------------------------------------------------------*---------/
        private static byte[] GetMessageLength(Stream messageStream)
        {
            return Encoding.UTF8.GetBytes(messageStream.Length.ToString());
        }

        //---------------------------------------------------------------------------------*---------/
        public static object DeserializePacketFromWrapper(byte[] wrappedData)
        {
            int idx = 0;

            while (wrappedData[idx] != LENGTH_HEADER_SEP)
            {
                idx++;
            }

            MemoryStream msgSource = new MemoryStream(wrappedData, idx + 1, wrappedData.Length - idx - 1);
            MessageWrapper msg = (MessageWrapper)Serializer.NonGeneric.Deserialize(typeof(MessageWrapper), msgSource);

            MemoryStream dataSource = new MemoryStream(msg.Payload);
            object data = Serializer.NonGeneric.Deserialize(ProtocolTypeMapper.GetTypeForCode(msg.MessageType), dataSource);

            return data;
        }

        //---------------------------------------------------------------------------------*---------/
        public IList<object> DeserializeFromWrapper(byte[] wrapperData)
        {
            lock (_syncLock)
            {
                return DeserializeFromWrapper(new BetterArraySegment<byte>(wrapperData, 0, wrapperData.Length));
            }
        }

        //---------------------------------------------------------------------------------*---------/
        public IList<object> DeserializeFromWrapper(BetterArraySegment<byte> wrapperData)
        {
            return Deserialize(wrapperData);
        }

        //---------------------------------------------------------------------------------*---------/
        private IList<object> Deserialize(BetterArraySegment<byte> sourceData)
        {
            if (sourceData.Length == 0)
            {
                return null;
            }
            IList<object> deserializedList = new List<object>();

            BetterArraySegment<byte> source = sourceData;

            lock (_syncLock)
            {
                int srcIdx;
                int lastIdx = 0;
                while ((srcIdx = LoadData(source)) != -1)
                {
                    MemoryStream msgSource = new MemoryStream(_dataBody);
                    object data;

                    try
                    {
                        MessageWrapper msg = (MessageWrapper)Serializer.NonGeneric.Deserialize(typeof(MessageWrapper), msgSource);

                        MemoryStream dataSource = new MemoryStream(msg.Payload);
                        data = Serializer.NonGeneric.Deserialize(ProtocolTypeMapper.GetTypeForCode(msg.MessageType),
                                                                 dataSource);

                    }
                    catch (Exception err)
                    {
                        LOG.Error(String.Format("Could not deserialize message"), err);
                        ResetState();
                        continue;
                    }

                    if (data != null)
                    {
                        deserializedList.Add(data);
                    }

                    ResetState();
                    lastIdx += srcIdx;
                    if (sourceData.Length == lastIdx)
                    {
                        break;
                    }

                    source = new BetterArraySegment<byte>(source.Buffer, lastIdx, source.Buffer.Length - lastIdx);
                }

                return deserializedList;
            }
        }

        //---------------------------------------------------------------------------------*---------/
        private int LoadData(BetterArraySegment<byte> sourceData)
        {
            int sourceDataIdx = 0;
            if (!_haveLengthData)
            {
                while ((sourceDataIdx < sourceData.Length))
                {
                    if ((char)sourceData[sourceDataIdx] == LENGTH_HEADER_SEP)
                    {
                        _haveLengthData = true;
                        sourceDataIdx++;
                        break;
                    }
                    _dataLengthAsBytes.Add(sourceData[sourceDataIdx++]);
                }
            }
            // If _lengthIdx < 5 it means we have not yet received the "whole" length
            if (!_haveLengthData)
            {
                return -1;
            }

            if (_dataLength == -1)
            {
                _dataLength = int.Parse(Encoding.UTF8.GetString(_dataLengthAsBytes.ToArray()));
            }

            if (_dataBody == null)
            {
                _dataBody = new byte[_dataLength];
            }

            while ((_bodyIdx < _dataLength) && (sourceDataIdx < sourceData.Length))
            {
                _dataBody[_bodyIdx++] = sourceData[sourceDataIdx++];
            }

            if (_bodyIdx == _dataLength)
            {
                return sourceDataIdx;
            }

            return -1;
        }

        //---------------------------------------------------------------------------------*---------/
        private void ResetState()
        {
            _dataBody = null;
            _bodyIdx = 0;
            _dataLength = -1;
            _dataLengthAsBytes.Clear();
            _haveLengthData = false;
        }

    }
}