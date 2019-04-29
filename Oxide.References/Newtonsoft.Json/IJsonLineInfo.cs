using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json
{
	[Preserve]
	public interface IJsonLineInfo
	{
		int LineNumber
		{
			get;
		}

		int LinePosition
		{
			get;
		}

		bool HasLineInfo();
	}
}