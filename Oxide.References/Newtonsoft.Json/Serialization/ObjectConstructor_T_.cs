using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public delegate object ObjectConstructor<T>(params object[] args);
}