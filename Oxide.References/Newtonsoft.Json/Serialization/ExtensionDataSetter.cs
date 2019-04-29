using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public delegate void ExtensionDataSetter(object o, string key, object value);
}