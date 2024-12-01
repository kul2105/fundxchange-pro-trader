using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;

namespace FundXchange.Model.Serialization
{
    public static class ObjectSerializer<T>
    {
        public static string Serialize(T objectToSerialize)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (MemoryStream serializationStream = new MemoryStream())
            {
                serializer.Serialize(serializationStream, objectToSerialize);
                byte[] streamData = serializationStream.ToArray();
                return Encoding.UTF8.GetString(streamData);
            }
        }

        public static T Deserialize(string xmlDocumentPath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (var xmlReader = XmlReader.Create(xmlDocumentPath))
            {
                if (serializer.CanDeserialize(xmlReader))
                {
                    return (T)serializer.Deserialize(xmlReader);
                }
            }
            throw new SerializationException(string.Format("Unable to deserialize the given document to type {0}", typeof(T).ToString()));
        }
    }
}
