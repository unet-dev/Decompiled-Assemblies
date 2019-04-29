using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Newtonsoft.Json.Linq.JsonPath
{
	[Preserve]
	internal class FieldMultipleFilter : PathFilter
	{
		public List<string> Names
		{
			get;
			set;
		}

		public FieldMultipleFilter()
		{
		}

		public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			List<string> names;
			CultureInfo invariantCulture;
			JToken jTokens;
			string str;
			using (IEnumerator<JToken> enumerator = current.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					jTokens = enumerator.Current;
					JObject jObjects = jTokens as JObject;
					if (jObjects != null)
					{
						List<string>.Enumerator enumerator1 = this.Names.GetEnumerator();
						try
						{
							while (enumerator1.MoveNext())
							{
								str = enumerator1.Current;
								JToken item = jObjects[str];
								if (item != null)
								{
									yield return item;
								}
								if (errorWhenNoMatch)
								{
									throw new JsonException("Property '{0}' does not exist on JObject.".FormatWith(CultureInfo.InvariantCulture, str));
								}
								str = null;
							}
							goto Label3;
							throw new JsonException("Property '{0}' does not exist on JObject.".FormatWith(CultureInfo.InvariantCulture, str));
						}
						finally
						{
							((IDisposable)enumerator1).Dispose();
						}
					Label3:
						enumerator1 = new List<string>.Enumerator();
					}
					else if (errorWhenNoMatch)
					{
						invariantCulture = CultureInfo.InvariantCulture;
						names = this.Names;
						throw new JsonException("Properties {0} not valid on {1}.".FormatWith(invariantCulture, string.Join(", ", (
							from n in names
							select string.Concat("'", n, "'")).ToArray<string>()), jTokens.GetType().Name));
					}
					jObjects = null;
					jTokens = null;
				}
				goto Label1;
				invariantCulture = CultureInfo.InvariantCulture;
				names = this.Names;
				throw new JsonException("Properties {0} not valid on {1}.".FormatWith(invariantCulture, string.Join(", ", (
					from n in names
					select string.Concat("'", n, "'")).ToArray<string>()), jTokens.GetType().Name));
			}
		Label1:
			enumerator = null;
		}
	}
}