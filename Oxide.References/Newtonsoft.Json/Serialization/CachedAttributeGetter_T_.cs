using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	internal static class CachedAttributeGetter<T>
	where T : Attribute
	{
		private readonly static ThreadSafeStore<object, T> TypeAttributeCache;

		static CachedAttributeGetter()
		{
			CachedAttributeGetter<T>.TypeAttributeCache = new ThreadSafeStore<object, T>(new Func<object, T>(JsonTypeReflector.GetAttribute<T>));
		}

		public static T GetAttribute(object type)
		{
			return CachedAttributeGetter<T>.TypeAttributeCache.Get(type);
		}
	}
}