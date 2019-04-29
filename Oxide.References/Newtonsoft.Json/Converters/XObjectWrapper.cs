using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	internal class XObjectWrapper : IXmlNode
	{
		private readonly static List<IXmlNode> EmptyChildNodes;

		private readonly XObject _xmlObject;

		public virtual List<IXmlNode> Attributes
		{
			get
			{
				return null;
			}
		}

		public virtual List<IXmlNode> ChildNodes
		{
			get
			{
				return XObjectWrapper.EmptyChildNodes;
			}
		}

		public virtual string LocalName
		{
			get
			{
				return null;
			}
		}

		public virtual string NamespaceUri
		{
			get
			{
				return null;
			}
		}

		public virtual XmlNodeType NodeType
		{
			get
			{
				return this._xmlObject.NodeType;
			}
		}

		public virtual IXmlNode ParentNode
		{
			get
			{
				return null;
			}
		}

		public virtual string Value
		{
			get
			{
				return null;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public object WrappedNode
		{
			get
			{
				return this._xmlObject;
			}
		}

		static XObjectWrapper()
		{
			XObjectWrapper.EmptyChildNodes = new List<IXmlNode>();
		}

		public XObjectWrapper(XObject xmlObject)
		{
			this._xmlObject = xmlObject;
		}

		public virtual IXmlNode AppendChild(IXmlNode newChild)
		{
			throw new InvalidOperationException();
		}
	}
}