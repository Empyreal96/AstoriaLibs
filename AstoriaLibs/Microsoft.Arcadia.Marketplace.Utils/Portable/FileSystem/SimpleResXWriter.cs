using System;
using System.IO;
using System.Xml;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable.FileSystem
{
	public class SimpleResXWriter : IDisposable
	{
		private const string RootNodeElement = "root";

		private const string EntryNodeElement = "data";

		private const string EntryNodeNameAttribute = "name";

		private const string EntryNodeValueElement = "value";

		private XmlWriter writer;

		private bool isClosed;

		private string xmlSchemaDefinition = "<xsd:schema id=\"root\" xmlns=\"\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" \r\nxmlns:msdata=\"urn:schemas-microsoft-com:xml-msdata\">\r\n<xsd:element name=\"data\">\r\n            <xsd:complexType>\r\n                <xsd:sequence>\r\n                    <xsd:element name=\"value\" type=\"xsd:string\" minOccurs=\"0\"\r\n                    msdata:Ordinal=\"2\" />\r\n                </xsd:sequence>\r\n                    <xsd:attribute name=\"name\" type=\"xsd:string\" />\r\n                    <xsd:attribute name=\"type\" type=\"xsd:string\" />\r\n            </xsd:complexType>\r\n        </xsd:element>\r\n        </xsd:schema>";

		public SimpleResXWriter(Stream outputStream)
		{
			if (outputStream == null)
			{
				throw new ArgumentNullException("outputStream");
			}
			writer = XmlWriter.Create(outputStream, new XmlWriterSettings
			{
				CloseOutput = true,
				Indent = true,
				NewLineOnAttributes = true
			});
			WriteDefinitions();
		}

		public void AddString(string keyName, string keyValue)
		{
			if (string.IsNullOrEmpty(keyName))
			{
				throw new ArgumentException("Must not be null or blank", "keyName");
			}
			if (string.IsNullOrEmpty(keyValue))
			{
				throw new ArgumentException("Must not be null or blank", "keyValue");
			}
			writer.WriteStartElement("data");
			writer.WriteAttributeString("name", keyName);
			writer.WriteStartElement("value");
			writer.WriteString(keyValue);
			writer.WriteEndElement();
			writer.WriteEndElement();
		}

		public void Close()
		{
			if (!isClosed)
			{
				WriteCloseDefinitions();
				writer.Flush();
				writer.Dispose();
				isClosed = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Close();
			}
		}

		private void WriteDefinitions()
		{
			writer.WriteStartElement("root");
			writer.WriteRaw(xmlSchemaDefinition);
		}

		private void WriteCloseDefinitions()
		{
			writer.WriteEndElement();
		}
	}
}