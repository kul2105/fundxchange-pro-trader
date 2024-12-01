using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using FundXchange.Morningstar.Enumerations;

namespace FundXchange.Morningstar.Messages
{
    /// <summary>
    /// This class represents a deserialized message received from MorningStar
    /// </summary>
    [Serializable]
    [XmlRoot("tapi_msg")]
    public class MorningstarMessage
    {
        #region Public members

        /// <summary>
        /// Represents the integral message type. This field is for internal use only.
        /// </summary>
        [XmlAttribute("msg")]
        public int _IntegralMessageType;
        
        /// <summary>
        /// Represents the integral security type. This field is for internal use only.
        /// </summary>
        [XmlAttribute("sec")]
        public int _IntegralSecurityType;

        /// <summary>
        /// The market identifier of the originating market.
        /// </summary>
        [XmlAttribute("mkt")]
        public int _MarketId;

        [XmlAttribute("fields")]
        public int _FieldCount;

        /// <summary>
        /// Fields contained in the message. This field is for internal use only. Please use the supplied methods to access specific fields.
        /// </summary>
        [XmlElement("fld")]
        public MessageField[] _Fields;

        #endregion

        #region Public properties

        [XmlIgnore]
        public MessageTypes MessageType
        {
            get { return (MessageTypes)Enum.ToObject(typeof(MessageTypes), _IntegralMessageType); }
        }

        [XmlIgnore]
        public SecurityTypes SecurityType
        {
            get { return (SecurityTypes)Enum.ToObject(typeof(SecurityTypes), _IntegralSecurityType); }
        }

        [XmlIgnore]
        public DateTime TimeStamp
        {
            get
            {
                if (HasATimeStamp())
                {
                    return new DateTime(
                        GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_TIME_YEAR),
                        GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_TIME_MONTH),
                        GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_TIME_DAY),
                        GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_TIME_HOURS),
                        GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_TIME_MINUTES),
                        GetFieldValueAs<int>(FieldIdentifiers.TF_FIELD_TIME_SECONDS));
                }
                return DateTime.MinValue;
            }
        }

        [XmlIgnore]
        public string ErrorMessage
        {
            get
            {
                if (HasField(FieldIdentifiers.TF_FIELD_ERROR_TEXT))
                    return GetFieldValueAs<string>(FieldIdentifiers.TF_FIELD_ERROR_TEXT);
                return string.Empty;
            }
        }

        [XmlIgnore]
        public bool IsAnErrorMessage
        {
            get { return !string.IsNullOrEmpty(ErrorMessage); }
        }

        #endregion

        #region Public methods

        public bool HasField(FieldIdentifiers fieldIdentifier)
        {
            var field = _Fields.SingleOrDefault(f => f.Identifier == fieldIdentifier);
            return field != null;
        }

        public T GetFieldValueAs<T>(FieldIdentifiers fieldIdentifier)
        {
            try
            {
                var field = this._Fields.FirstOrDefault(f => f.Identifier == fieldIdentifier);

                if (null != field)
                {
                    return (T)Convert.ChangeType(field._Value, typeof(T));
                }
            }
            catch { }

            return default(T);
        }

        public static MorningstarMessage DeserializeFrom(string xmlString)
        {
            var deserializer = new XmlSerializer(typeof(MorningstarMessage));            
            StringReader sb = new StringReader(xmlString);
            return (MorningstarMessage)deserializer.Deserialize(sb);
        }

        #endregion

        #region Private methods

        private bool HasATimeStamp()
        {
            return HasField(FieldIdentifiers.TF_FIELD_TIME_YEAR)
                && HasField(FieldIdentifiers.TF_FIELD_TIME_MONTH)
                && HasField(FieldIdentifiers.TF_FIELD_TIME_DAY);
        }

        #endregion

        #region Overridden members

        public override string ToString()
        {
            StringBuilder messageString = new StringBuilder();
            messageString.AppendLine(string.Format("Message type: {0}", this.MessageType));

            if (this.IsAnErrorMessage)
            {
                messageString.AppendLine(string.Format("Error: {0}", ErrorMessage));
            }
            else
            {
                foreach (var field in _Fields)
                {
                    messageString.AppendLine(string.Format("{0} : {1}", field.Identifier.ToString(), GetFieldValueAs<string>(field.Identifier)));
                }
            }
            return messageString.ToString();
        }

        #endregion
    }

    /// <summary>
    /// This class represents a single message field contained inside a MorningStarMessage
    /// </summary>
    public class MessageField
    {
        /// <summary>
        /// Represents the integral field identifier. This field is for internal use only.
        /// </summary>
        [XmlAttribute("id")]
        public string _IntegralIdentifier;

        /// <summary>
        /// Represents the raw field value. This field is for internal use only.
        /// </summary>
        [XmlText]
        public string _Value;

        public FieldIdentifiers Identifier
        {
            get
            {
                int i;
                if (int.TryParse(_IntegralIdentifier, out i))
                {
                    return (FieldIdentifiers)Enum.ToObject(typeof(FieldIdentifiers), i);
                }
                else
                {
                    // Determine the field type based on its' string value.
                    // Currently defaults to TF_FIELD_SYMBOL
                    return default(FieldIdentifiers);
                }
            }
        }
    }
}