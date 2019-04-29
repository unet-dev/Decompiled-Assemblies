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
	internal class ArrayMultipleIndexFilter : PathFilter
	{
		public List<int> Indexes
		{
			get;
			set;
		}

		public ArrayMultipleIndexFilter()
		{
		}

		public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken jTokens in current)
			{
				foreach (int index in this.Indexes)
				{
					JToken tokenIndex = PathFilter.GetTokenIndex(jTokens, errorWhenNoMatch, index);
					if (tokenIndex == null)
					{
						continue;
					}
					yield return tokenIndex;
				}
			}
		}
	}
}