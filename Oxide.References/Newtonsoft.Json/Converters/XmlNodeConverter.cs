using Newtonsoft.Json;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	[Preserve]
	public class XmlNodeConverter : JsonConverter
	{
		private const string TextName = "#text";

		private const string CommentName = "#comment";

		private const string CDataName = "#cdata-section";

		private const string WhitespaceName = "#whitespace";

		private const string SignificantWhitespaceName = "#significant-whitespace";

		private const string DeclarationName = "?xml";

		private const string JsonNamespaceUri = "http://james.newtonking.com/projects/json";

		public string DeserializeRootElementName
		{
			get;
			set;
		}

		public bool OmitRootObject
		{
			get;
			set;
		}

		public bool WriteArrayAttribute
		{
			get;
			set;
		}

		public XmlNodeConverter()
		{
		}

		private static void AddAttribute(JsonReader reader, IXmlDocument document, IXmlNode currentNode, string attributeName, XmlNamespaceManager manager, string attributePrefix)
		{
			IXmlNode xmlNode;
			string str = XmlConvert.EncodeName(attributeName);
			string str1 = reader.Value.ToString();
			xmlNode = (!string.IsNullOrEmpty(attributePrefix) ? document.CreateAttribute(str, manager.LookupNamespace(attributePrefix), str1) : document.CreateAttribute(str, str1));
			((IXmlElement)currentNode).SetAttributeNode(xmlNode);
		}

		private void AddJsonArrayAttribute(IXmlElement element, IXmlDocument document)
		{
			element.SetAttributeNode(document.CreateAttribute("json:Array", "http://james.newtonking.com/projects/json", "true"));
			if (element is XElementWrapper && element.GetPrefixOfNamespace("http://james.newtonking.com/projects/json") == null)
			{
				element.SetAttributeNode(document.CreateAttribute("xmlns:json", "http://www.w3.org/2000/xmlns/", "http://james.newtonking.com/projects/json"));
			}
		}

		private static bool AllSameName(IXmlNode node)
		{
			bool flag;
			List<IXmlNode>.Enumerator enumerator = node.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.LocalName == node.LocalName)
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		public override bool CanConvert(Type valueType)
		{
			if (typeof(XObject).IsAssignableFrom(valueType))
			{
				return true;
			}
			if (typeof(XmlNode).IsAssignableFrom(valueType))
			{
				return true;
			}
			return false;
		}

		private string ConvertTokenToXmlValue(JsonReader reader)
		{
			if (reader.TokenType == JsonToken.String)
			{
				if (reader.Value == null)
				{
					return null;
				}
				return reader.Value.ToString();
			}
			if (reader.TokenType == JsonToken.Integer)
			{
				return XmlConvert.ToString(Convert.ToInt64(reader.Value, CultureInfo.InvariantCulture));
			}
			if (reader.TokenType == JsonToken.Float)
			{
				if (reader.Value is decimal)
				{
					return XmlConvert.ToString((decimal)reader.Value);
				}
				if (reader.Value is float)
				{
					return XmlConvert.ToString((float)reader.Value);
				}
				return XmlConvert.ToString(Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture));
			}
			if (reader.TokenType == JsonToken.Boolean)
			{
				return XmlConvert.ToString(Convert.ToBoolean(reader.Value, CultureInfo.InvariantCulture));
			}
			if (reader.TokenType != JsonToken.Date)
			{
				if (reader.TokenType != JsonToken.Null)
				{
					throw JsonSerializationException.Create(reader, "Cannot get an XML string value from token type '{0}'.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
				}
				return null;
			}
			if (reader.Value is DateTimeOffset)
			{
				return XmlConvert.ToString((DateTimeOffset)reader.Value);
			}
			DateTime dateTime = Convert.ToDateTime(reader.Value, CultureInfo.InvariantCulture);
			return XmlConvert.ToString(dateTime, DateTimeUtils.ToSerializationMode(dateTime.Kind));
		}

		private void CreateDocumentType(JsonReader reader, IXmlDocument document, IXmlNode currentNode)
		{
			string str = null;
			string str1 = null;
			string str2 = null;
			string str3 = null;
			while (reader.Read() && reader.TokenType != JsonToken.EndObject)
			{
				string str4 = reader.Value.ToString();
				if (str4 == "@name")
				{
					reader.Read();
					str = reader.Value.ToString();
				}
				else if (str4 == "@public")
				{
					reader.Read();
					str1 = reader.Value.ToString();
				}
				else if (str4 == "@system")
				{
					reader.Read();
					str2 = reader.Value.ToString();
				}
				else
				{
					if (str4 != "@internalSubset")
					{
						throw JsonSerializationException.Create(reader, string.Concat("Unexpected property name encountered while deserializing XmlDeclaration: ", reader.Value));
					}
					reader.Read();
					str3 = reader.Value.ToString();
				}
			}
			IXmlNode xmlNode = document.CreateXmlDocumentType(str, str1, str2, str3);
			currentNode.AppendChild(xmlNode);
		}

		private void CreateElement(JsonReader reader, IXmlDocument document, IXmlNode currentNode, string elementName, XmlNamespaceManager manager, string elementPrefix, Dictionary<string, string> attributeNameValues)
		{
			IXmlElement xmlElement = this.CreateElement(elementName, document, elementPrefix, manager);
			currentNode.AppendChild(xmlElement);
			foreach (KeyValuePair<string, string> attributeNameValue in attributeNameValues)
			{
				string str = XmlConvert.EncodeName(attributeNameValue.Key);
				string prefix = MiscellaneousUtils.GetPrefix(attributeNameValue.Key);
				xmlElement.SetAttributeNode((!string.IsNullOrEmpty(prefix) ? document.CreateAttribute(str, manager.LookupNamespace(prefix) ?? string.Empty, attributeNameValue.Value) : document.CreateAttribute(str, attributeNameValue.Value)));
			}
			if (reader.TokenType == JsonToken.String || reader.TokenType == JsonToken.Integer || reader.TokenType == JsonToken.Float || reader.TokenType == JsonToken.Boolean || reader.TokenType == JsonToken.Date)
			{
				string xmlValue = this.ConvertTokenToXmlValue(reader);
				if (xmlValue != null)
				{
					xmlElement.AppendChild(document.CreateTextNode(xmlValue));
					return;
				}
			}
			else if (reader.TokenType != JsonToken.Null)
			{
				if (reader.TokenType != JsonToken.EndObject)
				{
					manager.PushScope();
					this.DeserializeNode(reader, document, manager, xmlElement);
					manager.PopScope();
				}
				manager.RemoveNamespace(string.Empty, manager.DefaultNamespace);
			}
		}

		private IXmlElement CreateElement(string elementName, IXmlDocument document, string elementPrefix, XmlNamespaceManager manager)
		{
			string str = XmlConvert.EncodeName(elementName);
			string str1 = (string.IsNullOrEmpty(elementPrefix) ? manager.DefaultNamespace : manager.LookupNamespace(elementPrefix));
			if (string.IsNullOrEmpty(str1))
			{
				return document.CreateElement(str);
			}
			return document.CreateElement(str, str1);
		}

		private void CreateInstruction(JsonReader reader, IXmlDocument document, IXmlNode currentNode, string propertyName)
		{
			if (propertyName != "?xml")
			{
				IXmlNode xmlNode = document.CreateProcessingInstruction(propertyName.Substring(1), reader.Value.ToString());
				currentNode.AppendChild(xmlNode);
				return;
			}
			string str = null;
			string str1 = null;
			string str2 = null;
			while (reader.Read() && reader.TokenType != JsonToken.EndObject)
			{
				string str3 = reader.Value.ToString();
				if (str3 == "@version")
				{
					reader.Read();
					str = reader.Value.ToString();
				}
				else if (str3 == "@encoding")
				{
					reader.Read();
					str1 = reader.Value.ToString();
				}
				else
				{
					if (str3 != "@standalone")
					{
						throw JsonSerializationException.Create(reader, string.Concat("Unexpected property name encountered while deserializing XmlDeclaration: ", reader.Value));
					}
					reader.Read();
					str2 = reader.Value.ToString();
				}
			}
			IXmlNode xmlNode1 = document.CreateXmlDeclaration(str, str1, str2);
			currentNode.AppendChild(xmlNode1);
		}

		private void DeserializeNode(JsonReader reader, IXmlDocument document, XmlNamespaceManager manager, IXmlNode currentNode)
		{
			do
			{
				JsonToken tokenType = reader.TokenType;
				switch (tokenType)
				{
					case JsonToken.StartConstructor:
					{
						string str = reader.Value.ToString();
						while (reader.Read())
						{
							if (reader.TokenType != JsonToken.EndConstructor)
							{
								this.DeserializeValue(reader, document, manager, str, currentNode);
							}
							else
							{
								goto Label0;
							}
						}
						goto Label0;
					}
					case JsonToken.PropertyName:
					{
						if (currentNode.NodeType == XmlNodeType.Document && document.DocumentElement != null)
						{
							throw JsonSerializationException.Create(reader, "JSON root object has multiple properties. The root object must have a single property in order to create a valid XML document. Consider specifing a DeserializeRootElementName.");
						}
						string str1 = reader.Value.ToString();
						reader.Read();
						if (reader.TokenType != JsonToken.StartArray)
						{
							this.DeserializeValue(reader, document, manager, str1, currentNode);
							continue;
						}
						else
						{
							int num = 0;
							while (reader.Read() && reader.TokenType != JsonToken.EndArray)
							{
								this.DeserializeValue(reader, document, manager, str1, currentNode);
								num++;
							}
							if (num != 1 || !this.WriteArrayAttribute)
							{
								continue;
							}
							List<IXmlNode>.Enumerator enumerator = currentNode.ChildNodes.GetEnumerator();
							try
							{
								while (enumerator.MoveNext())
								{
									IXmlElement current = enumerator.Current as IXmlElement;
									if (current == null || !(current.LocalName == str1))
									{
										continue;
									}
									this.AddJsonArrayAttribute(current, document);
									goto Label0;
								}
								continue;
							}
							finally
							{
								((IDisposable)enumerator).Dispose();
							}
						}
						break;
					}
					case JsonToken.Comment:
					{
						currentNode.AppendChild(document.CreateComment((string)reader.Value));
						continue;
					}
					default:
					{
						if (tokenType == JsonToken.EndObject || tokenType == JsonToken.EndArray)
						{
							break;
						}
						else
						{
							throw JsonSerializationException.Create(reader, string.Concat("Unexpected JsonToken when deserializing node: ", reader.TokenType));
						}
					}
				}
				return;
			Label0:
			}
			while (reader.TokenType == JsonToken.PropertyName || reader.Read());
		}

		private void DeserializeValue(JsonReader reader, IXmlDocument document, XmlNamespaceManager manager, string propertyName, IXmlNode currentNode)
		{
			if (propertyName == "#text")
			{
				currentNode.AppendChild(document.CreateTextNode(reader.Value.ToString()));
				return;
			}
			if (propertyName == "#cdata-section")
			{
				currentNode.AppendChild(document.CreateCDataSection(reader.Value.ToString()));
				return;
			}
			if (propertyName == "#whitespace")
			{
				currentNode.AppendChild(document.CreateWhitespace(reader.Value.ToString()));
				return;
			}
			if (propertyName == "#significant-whitespace")
			{
				currentNode.AppendChild(document.CreateSignificantWhitespace(reader.Value.ToString()));
				return;
			}
			if (!string.IsNullOrEmpty(propertyName) && propertyName[0] == '?')
			{
				this.CreateInstruction(reader, document, currentNode, propertyName);
				return;
			}
			if (string.Equals(propertyName, "!DOCTYPE", StringComparison.OrdinalIgnoreCase))
			{
				this.CreateDocumentType(reader, document, currentNode);
				return;
			}
			if (reader.TokenType == JsonToken.StartArray)
			{
				this.ReadArrayElements(reader, document, propertyName, currentNode, manager);
				return;
			}
			this.ReadElement(reader, document, currentNode, propertyName, manager);
		}

		private string GetPropertyName(IXmlNode node, XmlNamespaceManager manager)
		{
			switch (node.NodeType)
			{
				case XmlNodeType.Element:
				{
					if (node.NamespaceUri == "http://james.newtonking.com/projects/json")
					{
						return string.Concat("$", node.LocalName);
					}
					return this.ResolveFullName(node, manager);
				}
				case XmlNodeType.Attribute:
				{
					if (node.NamespaceUri == "http://james.newtonking.com/projects/json")
					{
						return string.Concat("$", node.LocalName);
					}
					return string.Concat("@", this.ResolveFullName(node, manager));
				}
				case XmlNodeType.Text:
				{
					return "#text";
				}
				case XmlNodeType.CDATA:
				{
					return "#cdata-section";
				}
				case XmlNodeType.EntityReference:
				case XmlNodeType.Entity:
				case XmlNodeType.Document:
				case XmlNodeType.DocumentFragment:
				case XmlNodeType.Notation:
				case XmlNodeType.EndElement:
				case XmlNodeType.EndEntity:
				{
					throw new JsonSerializationException(string.Concat("Unexpected XmlNodeType when getting node name: ", node.NodeType));
				}
				case XmlNodeType.ProcessingInstruction:
				{
					return string.Concat("?", this.ResolveFullName(node, manager));
				}
				case XmlNodeType.Comment:
				{
					return "#comment";
				}
				case XmlNodeType.DocumentType:
				{
					return string.Concat("!", this.ResolveFullName(node, manager));
				}
				case XmlNodeType.Whitespace:
				{
					return "#whitespace";
				}
				case XmlNodeType.SignificantWhitespace:
				{
					return "#significant-whitespace";
				}
				case XmlNodeType.XmlDeclaration:
				{
					return "?xml";
				}
				default:
				{
					throw new JsonSerializationException(string.Concat("Unexpected XmlNodeType when getting node name: ", node.NodeType));
				}
			}
		}

		private bool IsArray(IXmlNode node)
		{
			bool flag;
			if (node.Attributes != null)
			{
				List<IXmlNode>.Enumerator enumerator = node.Attributes.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						IXmlNode current = enumerator.Current;
						if (!(current.LocalName == "Array") || !(current.NamespaceUri == "http://james.newtonking.com/projects/json"))
						{
							continue;
						}
						flag = XmlConvert.ToBoolean(current.Value);
						return flag;
					}
					return false;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return flag;
			}
			return false;
		}

		private bool IsNamespaceAttribute(string attributeName, out string prefix)
		{
			if (attributeName.StartsWith("xmlns", StringComparison.Ordinal))
			{
				if (attributeName.Length == 5)
				{
					prefix = string.Empty;
					return true;
				}
				if (attributeName[5] == ':')
				{
					prefix = attributeName.Substring(6, attributeName.Length - 6);
					return true;
				}
			}
			prefix = null;
			return false;
		}

		private void PushParentNamespaces(IXmlNode node, XmlNamespaceManager manager)
		{
			List<IXmlNode> xmlNodes = null;
			IXmlNode xmlNode = node;
			while (true)
			{
				IXmlNode parentNode = xmlNode.ParentNode;
				xmlNode = parentNode;
				if (parentNode == null)
				{
					break;
				}
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					if (xmlNodes == null)
					{
						xmlNodes = new List<IXmlNode>();
					}
					xmlNodes.Add(xmlNode);
				}
			}
			if (xmlNodes != null)
			{
				xmlNodes.Reverse();
				foreach (IXmlNode xmlNode1 in xmlNodes)
				{
					manager.PushScope();
					foreach (IXmlNode attribute in xmlNode1.Attributes)
					{
						if (!(attribute.NamespaceUri == "http://www.w3.org/2000/xmlns/") || !(attribute.LocalName != "xmlns"))
						{
							continue;
						}
						manager.AddNamespace(attribute.LocalName, attribute.Value);
					}
				}
			}
		}

		private void ReadArrayElements(JsonReader reader, IXmlDocument document, string propertyName, IXmlNode currentNode, XmlNamespaceManager manager)
		{
			string prefix = MiscellaneousUtils.GetPrefix(propertyName);
			IXmlElement xmlElement = this.CreateElement(propertyName, document, prefix, manager);
			currentNode.AppendChild(xmlElement);
			int num = 0;
			while (reader.Read() && reader.TokenType != JsonToken.EndArray)
			{
				this.DeserializeValue(reader, document, manager, propertyName, xmlElement);
				num++;
			}
			if (this.WriteArrayAttribute)
			{
				this.AddJsonArrayAttribute(xmlElement, document);
			}
			if (num == 1 && this.WriteArrayAttribute)
			{
				foreach (IXmlNode childNode in xmlElement.ChildNodes)
				{
					IXmlElement xmlElement1 = childNode as IXmlElement;
					if (xmlElement1 == null || !(xmlElement1.LocalName == propertyName))
					{
						continue;
					}
					this.AddJsonArrayAttribute(xmlElement1, document);
					return;
				}
			}
		}

		private Dictionary<string, string> ReadAttributeElements(JsonReader reader, XmlNamespaceManager manager)
		{
			string xmlValue;
			string str;
			string str1;
			Dictionary<string, string> strs = new Dictionary<string, string>();
			bool flag = false;
			bool flag1 = false;
			if (reader.TokenType != JsonToken.String && reader.TokenType != JsonToken.Null && reader.TokenType != JsonToken.Boolean && reader.TokenType != JsonToken.Integer && reader.TokenType != JsonToken.Float && reader.TokenType != JsonToken.Date && reader.TokenType != JsonToken.StartConstructor)
			{
				while (!flag && !flag1 && reader.Read())
				{
					JsonToken tokenType = reader.TokenType;
					if (tokenType == JsonToken.PropertyName)
					{
						string str2 = reader.Value.ToString();
						if (string.IsNullOrEmpty(str2))
						{
							flag = true;
						}
						else
						{
							char chr = str2[0];
							if (chr != '$')
							{
								if (chr != '@')
								{
									flag = true;
								}
								else
								{
									str2 = str2.Substring(1);
									reader.Read();
									xmlValue = this.ConvertTokenToXmlValue(reader);
									strs.Add(str2, xmlValue);
									if (!this.IsNamespaceAttribute(str2, out str))
									{
										continue;
									}
									manager.AddNamespace(str, xmlValue);
								}
							}
							else if (str2 == "$values" || str2 == "$id" || str2 == "$ref" || str2 == "$type" || str2 == "$value")
							{
								string str3 = manager.LookupPrefix("http://james.newtonking.com/projects/json");
								if (str3 == null)
								{
									int? nullable = null;
									while (manager.LookupNamespace(string.Concat("json", nullable)) != null)
									{
										nullable = new int?(nullable.GetValueOrDefault() + 1);
									}
									str3 = string.Concat("json", nullable);
									strs.Add(string.Concat("xmlns:", str3), "http://james.newtonking.com/projects/json");
									manager.AddNamespace(str3, "http://james.newtonking.com/projects/json");
								}
								if (str2 != "$values")
								{
									str2 = str2.Substring(1);
									reader.Read();
									if (!JsonTokenUtils.IsPrimitiveToken(reader.TokenType))
									{
										throw JsonSerializationException.Create(reader, string.Concat("Unexpected JsonToken: ", reader.TokenType));
									}
									if (reader.Value != null)
									{
										str1 = reader.Value.ToString();
									}
									else
									{
										str1 = null;
									}
									xmlValue = str1;
									strs.Add(string.Concat(str3, ":", str2), xmlValue);
								}
								else
								{
									flag = true;
								}
							}
							else
							{
								flag = true;
							}
						}
					}
					else if (tokenType == JsonToken.Comment)
					{
						flag1 = true;
					}
					else
					{
						if (tokenType != JsonToken.EndObject)
						{
							throw JsonSerializationException.Create(reader, string.Concat("Unexpected JsonToken: ", reader.TokenType));
						}
						flag1 = true;
					}
				}
			}
			return strs;
		}

		private void ReadElement(JsonReader reader, IXmlDocument document, IXmlNode currentNode, string propertyName, XmlNamespaceManager manager)
		{
			if (string.IsNullOrEmpty(propertyName))
			{
				throw JsonSerializationException.Create(reader, "XmlNodeConverter cannot convert JSON with an empty property name to XML.");
			}
			Dictionary<string, string> strs = this.ReadAttributeElements(reader, manager);
			string prefix = MiscellaneousUtils.GetPrefix(propertyName);
			if (propertyName.StartsWith('@'))
			{
				string str = propertyName.Substring(1);
				string prefix1 = MiscellaneousUtils.GetPrefix(str);
				XmlNodeConverter.AddAttribute(reader, document, currentNode, str, manager, prefix1);
				return;
			}
			if (propertyName.StartsWith('$'))
			{
				if (propertyName == "$values")
				{
					propertyName = propertyName.Substring(1);
					prefix = manager.LookupPrefix("http://james.newtonking.com/projects/json");
					this.CreateElement(reader, document, currentNode, propertyName, manager, prefix, strs);
					return;
				}
				if (propertyName == "$id" || propertyName == "$ref" || propertyName == "$type" || propertyName == "$value")
				{
					string str1 = propertyName.Substring(1);
					string str2 = manager.LookupPrefix("http://james.newtonking.com/projects/json");
					XmlNodeConverter.AddAttribute(reader, document, currentNode, str1, manager, str2);
					return;
				}
			}
			this.CreateElement(reader, document, currentNode, propertyName, manager, prefix, strs);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			XmlNamespaceManager xmlNamespaceManagers = new XmlNamespaceManager(new NameTable());
			IXmlDocument xDocumentWrapper = null;
			IXmlNode xmlNode = null;
			if (typeof(XObject).IsAssignableFrom(objectType))
			{
				if (objectType != typeof(XDocument) && objectType != typeof(XElement))
				{
					throw new JsonSerializationException("XmlNodeConverter only supports deserializing XDocument or XElement.");
				}
				xDocumentWrapper = new XDocumentWrapper(new XDocument());
				xmlNode = xDocumentWrapper;
			}
			if (typeof(XmlNode).IsAssignableFrom(objectType))
			{
				if (objectType != typeof(XmlDocument))
				{
					throw new JsonSerializationException("XmlNodeConverter only supports deserializing XmlDocuments");
				}
				xDocumentWrapper = new XmlDocumentWrapper(new XmlDocument()
				{
					XmlResolver = null
				});
				xmlNode = xDocumentWrapper;
			}
			if (xDocumentWrapper == null || xmlNode == null)
			{
				throw new JsonSerializationException(string.Concat("Unexpected type when converting XML: ", objectType));
			}
			if (reader.TokenType != JsonToken.StartObject)
			{
				throw new JsonSerializationException("XmlNodeConverter can only convert JSON that begins with an object.");
			}
			if (string.IsNullOrEmpty(this.DeserializeRootElementName))
			{
				reader.Read();
				this.DeserializeNode(reader, xDocumentWrapper, xmlNamespaceManagers, xmlNode);
			}
			else
			{
				this.ReadElement(reader, xDocumentWrapper, xmlNode, this.DeserializeRootElementName, xmlNamespaceManagers);
			}
			if (objectType != typeof(XElement))
			{
				return xDocumentWrapper.WrappedNode;
			}
			XElement wrappedNode = (XElement)xDocumentWrapper.DocumentElement.WrappedNode;
			wrappedNode.Remove();
			return wrappedNode;
		}

		private string ResolveFullName(IXmlNode node, XmlNamespaceManager manager)
		{
			string str;
			if (node.NamespaceUri == null || node.LocalName == "xmlns" && node.NamespaceUri == "http://www.w3.org/2000/xmlns/")
			{
				str = null;
			}
			else
			{
				str = manager.LookupPrefix(node.NamespaceUri);
			}
			string str1 = str;
			if (string.IsNullOrEmpty(str1))
			{
				return XmlConvert.DecodeName(node.LocalName);
			}
			return string.Concat(str1, ":", XmlConvert.DecodeName(node.LocalName));
		}

		private void SerializeGroupedNodes(JsonWriter writer, IXmlNode node, XmlNamespaceManager manager, bool writePropertyName)
		{
			List<IXmlNode> xmlNodes;
			bool flag;
			Dictionary<string, List<IXmlNode>> strs = new Dictionary<string, List<IXmlNode>>();
			for (int i = 0; i < node.ChildNodes.Count; i++)
			{
				IXmlNode item = node.ChildNodes[i];
				string propertyName = this.GetPropertyName(item, manager);
				if (!strs.TryGetValue(propertyName, out xmlNodes))
				{
					xmlNodes = new List<IXmlNode>();
					strs.Add(propertyName, xmlNodes);
				}
				xmlNodes.Add(item);
			}
			foreach (KeyValuePair<string, List<IXmlNode>> str in strs)
			{
				List<IXmlNode> value = str.Value;
				flag = (value.Count != 1 ? true : this.IsArray(value[0]));
				if (flag)
				{
					string key = str.Key;
					if (writePropertyName)
					{
						writer.WritePropertyName(key);
					}
					writer.WriteStartArray();
					for (int j = 0; j < value.Count; j++)
					{
						this.SerializeNode(writer, value[j], manager, false);
					}
					writer.WriteEndArray();
				}
				else
				{
					this.SerializeNode(writer, value[0], manager, writePropertyName);
				}
			}
		}

		private void SerializeNode(JsonWriter writer, IXmlNode node, XmlNamespaceManager manager, bool writePropertyName)
		{
			switch (node.NodeType)
			{
				case XmlNodeType.Element:
				{
					if (this.IsArray(node) && XmlNodeConverter.AllSameName(node) && node.ChildNodes.Count > 0)
					{
						this.SerializeGroupedNodes(writer, node, manager, false);
						return;
					}
					manager.PushScope();
					foreach (IXmlNode attribute in node.Attributes)
					{
						if (attribute.NamespaceUri != "http://www.w3.org/2000/xmlns/")
						{
							continue;
						}
						manager.AddNamespace((attribute.LocalName != "xmlns" ? XmlConvert.DecodeName(attribute.LocalName) : string.Empty), attribute.Value);
					}
					if (writePropertyName)
					{
						writer.WritePropertyName(this.GetPropertyName(node, manager));
					}
					if (!this.ValueAttributes(node.Attributes) && node.ChildNodes.Count == 1 && node.ChildNodes[0].NodeType == XmlNodeType.Text)
					{
						writer.WriteValue(node.ChildNodes[0].Value);
					}
					else if (node.ChildNodes.Count != 0 || !CollectionUtils.IsNullOrEmpty<IXmlNode>(node.Attributes))
					{
						writer.WriteStartObject();
						for (int i = 0; i < node.Attributes.Count; i++)
						{
							this.SerializeNode(writer, node.Attributes[i], manager, true);
						}
						this.SerializeGroupedNodes(writer, node, manager, true);
						writer.WriteEndObject();
					}
					else if (!((IXmlElement)node).IsEmpty)
					{
						writer.WriteValue(string.Empty);
					}
					else
					{
						writer.WriteNull();
					}
					manager.PopScope();
					return;
				}
				case XmlNodeType.Attribute:
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
				{
					if (node.NamespaceUri == "http://www.w3.org/2000/xmlns/" && node.Value == "http://james.newtonking.com/projects/json")
					{
						return;
					}
					if (node.NamespaceUri == "http://james.newtonking.com/projects/json" && node.LocalName == "Array")
					{
						return;
					}
					if (writePropertyName)
					{
						writer.WritePropertyName(this.GetPropertyName(node, manager));
					}
					writer.WriteValue(node.Value);
					return;
				}
				case XmlNodeType.EntityReference:
				case XmlNodeType.Entity:
				case XmlNodeType.Notation:
				case XmlNodeType.EndElement:
				case XmlNodeType.EndEntity:
				{
					throw new JsonSerializationException(string.Concat("Unexpected XmlNodeType when serializing nodes: ", node.NodeType));
				}
				case XmlNodeType.Comment:
				{
					if (!writePropertyName)
					{
						return;
					}
					writer.WriteComment(node.Value);
					return;
				}
				case XmlNodeType.Document:
				case XmlNodeType.DocumentFragment:
				{
					this.SerializeGroupedNodes(writer, node, manager, writePropertyName);
					return;
				}
				case XmlNodeType.DocumentType:
				{
					IXmlDocumentType xmlDocumentType = (IXmlDocumentType)node;
					writer.WritePropertyName(this.GetPropertyName(node, manager));
					writer.WriteStartObject();
					if (!string.IsNullOrEmpty(xmlDocumentType.Name))
					{
						writer.WritePropertyName("@name");
						writer.WriteValue(xmlDocumentType.Name);
					}
					if (!string.IsNullOrEmpty(xmlDocumentType.Public))
					{
						writer.WritePropertyName("@public");
						writer.WriteValue(xmlDocumentType.Public);
					}
					if (!string.IsNullOrEmpty(xmlDocumentType.System))
					{
						writer.WritePropertyName("@system");
						writer.WriteValue(xmlDocumentType.System);
					}
					if (!string.IsNullOrEmpty(xmlDocumentType.InternalSubset))
					{
						writer.WritePropertyName("@internalSubset");
						writer.WriteValue(xmlDocumentType.InternalSubset);
					}
					writer.WriteEndObject();
					return;
				}
				case XmlNodeType.XmlDeclaration:
				{
					IXmlDeclaration xmlDeclaration = (IXmlDeclaration)node;
					writer.WritePropertyName(this.GetPropertyName(node, manager));
					writer.WriteStartObject();
					if (!string.IsNullOrEmpty(xmlDeclaration.Version))
					{
						writer.WritePropertyName("@version");
						writer.WriteValue(xmlDeclaration.Version);
					}
					if (!string.IsNullOrEmpty(xmlDeclaration.Encoding))
					{
						writer.WritePropertyName("@encoding");
						writer.WriteValue(xmlDeclaration.Encoding);
					}
					if (!string.IsNullOrEmpty(xmlDeclaration.Standalone))
					{
						writer.WritePropertyName("@standalone");
						writer.WriteValue(xmlDeclaration.Standalone);
					}
					writer.WriteEndObject();
					return;
				}
				default:
				{
					throw new JsonSerializationException(string.Concat("Unexpected XmlNodeType when serializing nodes: ", node.NodeType));
				}
			}
		}

		private bool ValueAttributes(List<IXmlNode> c)
		{
			bool flag;
			List<IXmlNode>.Enumerator enumerator = c.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.NamespaceUri == "http://james.newtonking.com/projects/json")
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		private IXmlNode WrapXml(object value)
		{
			if (value is XObject)
			{
				return XContainerWrapper.WrapNode((XObject)value);
			}
			if (!(value is XmlNode))
			{
				throw new ArgumentException("Value must be an XML object.", "value");
			}
			return XmlNodeWrapper.WrapNode((XmlNode)value);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			IXmlNode xmlNode = this.WrapXml(value);
			XmlNamespaceManager xmlNamespaceManagers = new XmlNamespaceManager(new NameTable());
			this.PushParentNamespaces(xmlNode, xmlNamespaceManagers);
			if (!this.OmitRootObject)
			{
				writer.WriteStartObject();
			}
			this.SerializeNode(writer, xmlNode, xmlNamespaceManagers, !this.OmitRootObject);
			if (!this.OmitRootObject)
			{
				writer.WriteEndObject();
			}
		}
	}
}