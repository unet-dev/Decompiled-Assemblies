using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Newtonsoft.Json.Linq.JsonPath
{
	[Preserve]
	internal abstract class PathFilter
	{
		protected PathFilter()
		{
		}

		public abstract IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch);

		protected static JToken GetTokenIndex(JToken t, bool errorWhenNoMatch, int index)
		{
			JArray jArrays = t as JArray;
			JConstructor jConstructor = t as JConstructor;
			if (jArrays != null)
			{
				if (jArrays.Count > index)
				{
					return jArrays[index];
				}
				if (errorWhenNoMatch)
				{
					throw new JsonException("Index {0} outside the bounds of JArray.".FormatWith(CultureInfo.InvariantCulture, index));
				}
				return null;
			}
			if (jConstructor == null)
			{
				if (errorWhenNoMatch)
				{
					throw new JsonException("Index {0} not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, index, t.GetType().Name));
				}
				return null;
			}
			if (jConstructor.Count > index)
			{
				return jConstructor[index];
			}
			if (errorWhenNoMatch)
			{
				throw new JsonException("Index {0} outside the bounds of JConstructor.".FormatWith(CultureInfo.InvariantCulture, index));
			}
			return null;
		}
	}
}