using Newtonsoft.Json.Shims;
using System;
using System.Collections.ObjectModel;

namespace Newtonsoft.Json
{
	[Preserve]
	public class JsonConverterCollection : Collection<JsonConverter>
	{
		public JsonConverterCollection()
		{
		}
	}
}