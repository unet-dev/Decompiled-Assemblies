using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Newtonsoft.Json.Linq.JsonPath
{
	[Preserve]
	internal class ArrayIndexFilter : PathFilter
	{
		public int? Index
		{
			get;
			set;
		}

		public ArrayIndexFilter()
		{
		}

		public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			JToken jTokens;
			using (IEnumerator<JToken> enumerator = current.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					jTokens = enumerator.Current;
					if (this.Index.HasValue)
					{
						JToken jTokens1 = jTokens;
						bool flag = errorWhenNoMatch;
						int? index = this.Index;
						JToken tokenIndex = PathFilter.GetTokenIndex(jTokens1, flag, index.GetValueOrDefault());
						if (tokenIndex != null)
						{
							yield return tokenIndex;
						}
					}
					else if (jTokens is JArray || jTokens is JConstructor)
					{
						using (IEnumerator<JToken> enumerator1 = ((IEnumerable<JToken>)jTokens).GetEnumerator())
						{
							while (enumerator1.MoveNext())
							{
								yield return enumerator1.Current;
							}
						}
						enumerator1 = null;
					}
					else if (errorWhenNoMatch)
					{
						throw new JsonException("Index * not valid on {0}.".FormatWith(CultureInfo.InvariantCulture, jTokens.GetType().Name));
					}
					jTokens = null;
				}
				goto Label1;
				throw new JsonException("Index * not valid on {0}.".FormatWith(CultureInfo.InvariantCulture, jTokens.GetType().Name));
			}
		Label1:
			enumerator = null;
		}
	}
}