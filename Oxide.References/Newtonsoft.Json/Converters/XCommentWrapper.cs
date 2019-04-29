using System;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	internal class XCommentWrapper : XObjectWrapper
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

		private XComment Text
		{
			get
			{
				return (XComment)base.WrappedNode;
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

		public XCommentWrapper(XComment text) : base(text)
		{
		}
	}
}