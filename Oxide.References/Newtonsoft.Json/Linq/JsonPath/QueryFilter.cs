using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Shims;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Newtonsoft.Json.Linq.JsonPath
{
	[Preserve]
	internal class QueryFilter : PathFilter
	{
		public QueryExpression Expression
		{
			get;
			set;
		}

		public QueryFilter()
		{
		}

		public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken jTokens in current)
			{
				foreach (JToken jTokens1 in (IEnumerable<JToken>)jTokens)
				{
					if (!this.Expression.IsMatch(jTokens1))
					{
						continue;
					}
					yield return jTokens1;
				}
			}
		}
	}
}