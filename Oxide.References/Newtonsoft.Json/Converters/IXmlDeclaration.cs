using System;

namespace Newtonsoft.Json.Converters
{
	internal interface IXmlDeclaration : IXmlNode
	{
		string Encoding
		{
			get;
			set;
		}

		string Standalone
		{
			get;
			set;
		}

		string Version
		{
			get;
		}
	}
}