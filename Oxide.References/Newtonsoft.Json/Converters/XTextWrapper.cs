using System;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	internal class XTextWrapper : XObjectWrapper
	{
		public override IXmlNode ParentNode
		{
			get
			{
				if (this.Text.Parent == null)
				{
					return null;
				}
				return XContainerWrapper.WrapNode(this.Text.Parent);
			}
		}

		private XText Text
		{
			get
			{
				return (XText)base.WrappedNode;
			}
		}

		public override string Value
		{
			get
			{
				return this.Text.Value;
			}
			set
			{
				this.Text.Value = value;
			}
		}

		public XTextWrapper(XText text) : base(text)
		{
		}
	}
}