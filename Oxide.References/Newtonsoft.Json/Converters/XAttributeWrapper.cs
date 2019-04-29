using System;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	internal class XAttributeWrapper : XObjectWrapper
	{
		private XAttribute Attribute
		{
			get
			{
				return (XAttribute)base.WrappedNode;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.Attribute.Name.LocalName;
			}
		}

		public override string NamespaceUri
		{
			get
			{
				return this.Attribute.Name.NamespaceName;
			}
		}

		public override IXmlNode ParentNode
		{
			get
			{
				if (this.Attribute.Parent == null)
				{
					return null;
				}
				return XContainerWrapper.WrapNode(this.Attribute.Parent);
			}
		}

		public override string Value
		{
			get
			{
				return this.Attribute.Value;
			}
			set
			{
				this.Attribute.Value = value;
			}
		}

		public XAttributeWrapper(XAttribute attribute) : base(attribute)
		{
		}
	}
}