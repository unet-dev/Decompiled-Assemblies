using System;

namespace Newtonsoft.Json.Converters
{
	internal interface IXmlDocumentType : IXmlNode
	{
		string InternalSubset
		{
			get;
		}

		string Name
		{
			get;
		}

		string Public
		{
			get;
		}

		string System
		{
			get;
		}
	}
}