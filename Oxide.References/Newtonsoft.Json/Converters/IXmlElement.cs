using System;

namespace Newtonsoft.Json.Converters
{
	internal interface IXmlElement : IXmlNode
	{
		bool IsEmpty
		{
			get;
		}

		string GetPrefixOfNamespace(string namespaceUri);

		void SetAttributeNode(IXmlNode attribute);
	}
}