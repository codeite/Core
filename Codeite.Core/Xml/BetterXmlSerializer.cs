using System;
using System.IO;
using System.Security.Policy;
using System.Xml;
using System.Xml.Serialization;

namespace Codeite.Core.Xml
{
    public class BetterXmlSerializer<T>
    {
        private readonly XmlSerializer _xmlSerializer;

        public BetterXmlSerializer(XmlAttributeOverrides overrides = null, Type[] extraTypes = null, XmlRootAttribute root = null, string defaultNamespace = null, string location = null)
        {
            _xmlSerializer = new XmlSerializer(typeof(T), overrides, extraTypes ?? new Type[0], root, defaultNamespace, location);
        }

        public event XmlAttributeEventHandler UnknownAttribute
        {
            add { _xmlSerializer.UnknownAttribute += value; }
            remove { _xmlSerializer.UnknownAttribute -= value; }
        }

        public event XmlElementEventHandler UnknownElement
        {
            add { _xmlSerializer.UnknownElement += value; }
            remove { _xmlSerializer.UnknownElement -= value; }
        }

        public event XmlNodeEventHandler UnknownNode
        {
            add { _xmlSerializer.UnknownNode += value; }
            remove { _xmlSerializer.UnknownNode -= value; }
        }

        public event UnreferencedObjectEventHandler UnreferencedObject
        {
            add { _xmlSerializer.UnreferencedObject += value; }
            remove { _xmlSerializer.UnreferencedObject -= value; }
        }

        public bool CanDeserialize(XmlReader xmlReader)
        {
            return _xmlSerializer.CanDeserialize(xmlReader);
        }

        public T Deserialize(Stream stream)
        {
            return (T)_xmlSerializer.Deserialize(stream);
        }

        public T Deserialize(TextReader reader)
        {
            return (T)_xmlSerializer.Deserialize(reader);
        }

        public T Deserialize(XmlReader reader, string encodingStyle = null, XmlDeserializationEvents? events = null)
        {
            if (events == null)
            {
                return (T)_xmlSerializer.Deserialize(reader, encodingStyle);
            }

            return (T)_xmlSerializer.Deserialize(reader, encodingStyle, events.Value);
        }

        public T Deserialize(string xml, string encodingStyle = null, XmlDeserializationEvents? events = null)
        {
            using (var stringReader = new StringReader(xml))
            using (var xmlReader = new XmlTextReader(stringReader))
            {
                return Deserialize(xmlReader, encodingStyle, events);
            }
        }


        public void Serialize(Stream stream, T target, XmlSerializerNamespaces namespaces = null)
        {
            _xmlSerializer.Serialize(stream, target, namespaces);
        }

        public void Serialize(TextWriter textWriter, T target, XmlSerializerNamespaces namespaces = null)
        {
            _xmlSerializer.Serialize(textWriter, target, namespaces);
        }

        public void Serialize(XmlWriter xmlWriter, T target, XmlSerializerNamespaces namespaces = null, string encodingStyle = null, string id = null)
        {
            _xmlSerializer.Serialize(xmlWriter, target, namespaces, encodingStyle, id);
        }

        public string Serialize(T target, XmlSerializerNamespaces namespaces = null, string encodingStyle = null,
                                string id = null)
        {
            using (var stringWriter = new StringWriter())
            using (var xmlWriter = new XmlTextWriter(stringWriter))
            {
                _xmlSerializer.Serialize(xmlWriter, target, namespaces, encodingStyle, id);
                return stringWriter.GetStringBuilder().ToString();
            }
        }
    }
}
