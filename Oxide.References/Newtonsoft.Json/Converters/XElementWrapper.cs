using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	internal class XElementWrapper : XContainerWrapper, IXmlElement, IXmlNode
	{
		private List<IXmlNode> _attributes;

		public override List<IXmlNode> Attributes
		{
			get
			{
				string namespaceUri;
				if (this._attributes == null)
				{
					this._attributes = new List<IXmlNode>();
					foreach (XAttribute xAttribute in this.Element.Attributes())
					{
						this._attributes.Add(new XAttributeWrapper(xAttribute));
					}
					string str = this.NamespaceUri;
					if (!string.IsNullOrEmpty(str))
					{
						string str1 = str;
						IXmlNode parentNode = this.ParentNode;
						if (parentNode != null)
						{
							namespaceUri = parentNode.NamespaceUri;
						}
						else
						{
							namespaceUri = null;
						}
						if (str1 != namespaceUri && string.IsNullOrEmpty(this.GetPrefixOfNamespace(str)))
						{
							bool flag = false;
							foreach (IXmlNode _attribute in this._attributes)
							{
								if (!(_attribute.LocalName == "xmlns") || !string.IsNullOrEmpty(_attribute.NamespaceUri) || !(_attribute.Value == str))
								{
									continue;
								}
								flag = true;
							}
							if (!flag)
							{
								this._attributes.Insert(0, new XAttributeWrapper(new XAttribute("xmlns", str)));
							}
						}
					}
				}
				return this._attributes;
			}
		}

		private XElement Element
		{
			get
			{
				return (XElement)base.WrappedNode;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.Element.IsEmpty;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.Element.Name.LocalName;
			}
		}

		public override string NamespaceUri
		{
			get
			{
				return this.Element.Name.NamespaceName;
			}
		}

		public override string Value
		{
			get
			{
				return this.Element.Value;
			}
			set
			{
				this.Element.Value = value;
			}
		}

		public XElementWrapper(XElement element) : base(element)
		{
		}

		public override IXmlNode AppendChild(IXmlNode newChild)
		{
			this._attributes = null;
			return base.AppendChild(newChild);
		}

		public string GetPrefixOfNamespace(string namespaceUri)
		{
			return this.Element.GetPrefixOfNamespace(namespaceUri);
		}

		public void SetAttributeNode(IXmlNode attribute)
		{
			XObjectWrapper xObjectWrapper = (XObjectWrapper)attribute;
			this.Element.Add(xObjectWrapper.WrappedNode);
			this._attributes = null;
		}
	}
}