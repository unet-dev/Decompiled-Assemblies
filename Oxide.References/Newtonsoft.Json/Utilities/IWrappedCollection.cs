using Newtonsoft.Json.Shims;
using System;
using System.Collections;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal interface IWrappedCollection : IList, ICollection, IEnumerable
	{
		object UnderlyingCollection
		{
			get;
		}
	}
}