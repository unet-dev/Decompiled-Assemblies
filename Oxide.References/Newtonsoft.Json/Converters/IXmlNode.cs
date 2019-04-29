using System;
using System.Collections.Generic;
using System.Xml;

namespace Newtonsoft.Json.Converters
{
	internal interface IXmlNode
	{
		List<IXmlNode> Attributes
		{
			get;
		}

		List<IXmlNode> ChildNodes
		{
			get;
		}

		string LocalName
		{
			get;
		}

		string NamespaceUri
		{
			get;
		}

		XmlNodeType NodeType
		{
			get;
		}

		IXmlNode ParentNode
		{
			get;
		}

		string Value
		{
			get;
			set;
		}

		object WrappedNode
		{
			get;
		}

		IXmlNode AppendChild(IXmlNode newChild);
	}
}