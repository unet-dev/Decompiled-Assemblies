using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Newtonsoft.Json.Converters
{
	internal class XmlNodeWrapper : IXmlNode
	{
		private readonly XmlNode _node;

		private List<IXmlNode> _childNodes;

		private List<IXmlNode> _attributes;

		public List<IXmlNode> Attributes
		{
			get
			{
				if (this._node.Attributes == null)
				{
					return null;
				}
				if (this._attributes == null)
				{
					this._attributes = new List<IXmlNode>(this._node.Attributes.Count);
					foreach (XmlAttribute attribute in this._node.Attributes)
					{
						this._attributes.Add(XmlNodeWrapper.WrapNode(attribute));
					}
				}
				return this._attributes;
			}
		}

		public List<IXmlNode> ChildNodes
		{
			get
			{
				if (this._childNodes == null)
				{
					this._childNodes = new List<IXmlNode>(this._node.ChildNodes.Count);
					foreach (XmlNode childNode in this._node.ChildNodes)
					{
						this._childNodes.Add(XmlNodeWrapper.WrapNode(childNode));
					}
				}
				return this._childNodes;
			}
		}

		public virtual string LocalName
		{
			get
			{
				return this._node.LocalName;
			}
		}

		public string NamespaceUri
		{
			get
			{
				return this._node.NamespaceURI;
			}
		}

		public XmlNodeType NodeType
		{
			get
			{
				return this._node.NodeType;
			}
		}

		public IXmlNode ParentNode
		{
			get
			{
				XmlNode ownerElement;
				if (this._node is XmlAttribute)
				{
					ownerElement = ((XmlAttribute)this._node).OwnerElement;
				}
				else
				{
					ownerElement = this._node.ParentNode;
				}
				XmlNode xmlNodes = ownerElement;
				if (xmlNodes == null)
				{
					return null;
				}
				return XmlNodeWrapper.WrapNode(xmlNodes);
			}
		}

		public string Value
		{
			get
			{
				return this._node.Value;
			}
			set
			{
				this._node.Value = value;
			}
		}

		public object WrappedNode
		{
			get
			{
				return this._node;
			}
		}

		public XmlNodeWrapper(XmlNode node)
		{
			this._node = node;
		}

		public IXmlNode AppendChild(IXmlNode newChild)
		{
			XmlNodeWrapper xmlNodeWrapper = (XmlNodeWrapper)newChild;
			this._node.AppendChild(xmlNodeWrapper._node);
			this._childNodes = null;
			this._attributes = null;
			return newChild;
		}

		internal static IXmlNode WrapNode(XmlNode node)
		{
			XmlNodeType nodeType = node.NodeType;
			if (nodeType == XmlNodeType.Element)
			{
				return new XmlElementWrapper((XmlElement)node);
			}
			if (nodeType == XmlNodeType.DocumentType)
			{
				return new XmlDocumentTypeWrapper((XmlDocumentType)node);
			}
			if (nodeType != XmlNodeType.XmlDeclaration)
			{
				return new XmlNodeWrapper(node);
			}
			return new XmlDeclarationWrapper((XmlDeclaration)node);
		}
	}
}