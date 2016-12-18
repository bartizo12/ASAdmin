using AS.Domain.Interfaces;
using System.IO;
using System.Xml.Serialization;

namespace AS.Infrastructure
{
    /// <summary>
    /// Simple XMLSerializer class
    /// </summary>
    public class ASXmlSerializer : IXmlSerializer
    {
        /// <summary>
        /// Serialize object of generic type  to XML
        /// </summary>
        /// <typeparam name="T">Type of value to be serialized</typeparam>
        /// <param name="value">Value to be serialized</param>
        /// <returns>XML string output of the value</returns>
        public string SerializeToXML<T>(T value)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using (StringWriter writer = new StringWriter())
            {
                xmlSerializer.Serialize(writer, value);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Serializes object to XML.
        /// </summary>
        /// <param name="value">object to be serialized</param>
        /// <returns>XML string output of the value</returns>
        public string SerializeToXML(object value)
        {
            if (value == null)
                return string.Empty;

            XmlSerializer xmlSerializer = new XmlSerializer(value.GetType());
            using (StringWriter writer = new StringWriter())
            {
                xmlSerializer.Serialize(writer, value);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Deserializes from XML to generic object
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="xmlValue">XML representation of the object</param>
        /// <returns>Deserialzed object</returns>
        public T DeserializeFromXML<T>(string xmlValue)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using (StringReader reader = new StringReader(xmlValue))
            {
                return (T)xmlSerializer.Deserialize(reader);
            }
        }
    }
}