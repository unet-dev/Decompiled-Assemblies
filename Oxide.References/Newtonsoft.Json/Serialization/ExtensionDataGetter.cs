using Newtonsoft.Json.Shims;
using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public delegate IEnumerable<KeyValuePair<object, object>> ExtensionDataGetter(object o);
}