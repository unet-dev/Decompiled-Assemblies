using Newtonsoft.Json.Shims;
using System;
using System.Collections;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal interface IWrappedDictionary : IDictionary, ICollection, IEnumerable
	{
		object UnderlyingDictionary
		{
			get;
		}
	}
}