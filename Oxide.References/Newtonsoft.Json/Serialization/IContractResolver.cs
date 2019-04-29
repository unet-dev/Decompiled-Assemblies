using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public interface IContractResolver
	{
		JsonContract ResolveContract(Type type);
	}
}