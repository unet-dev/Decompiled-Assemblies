using Newtonsoft.Json.Shims;
using System;
using System.Runtime.Serialization;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public delegate void SerializationCallback(object o, StreamingContext context);
}