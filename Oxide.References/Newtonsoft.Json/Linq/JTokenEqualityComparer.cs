using Newtonsoft.Json.Shims;
using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq
{
	[Preserve]
	public class JTokenEqualityComparer : IEqualityComparer<JToken>
	{
		public JTokenEqualityComparer()
		{
		}

		public bool Equals(JToken x, JToken y)
		{
			return JToken.DeepEquals(x, y);
		}

		public int GetHashCode(JToken obj)
		{
			if (obj == null)
			{
				return 0;
			}
			return obj.GetDeepHashCode();
		}
	}
}