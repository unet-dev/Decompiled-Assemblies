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
	internal class FieldFilter : PathFilter
	{
		public string Name
		{
			get;
			set;
		}

		public FieldFilter()
		{
		}

		public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			string name;
			CultureInfo invariantCulture;
			string str;
			JToken jTokens;
			using (IEnumerator<JToken> enumerator = current.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					jTokens = enumerator.Current;
					JObject jObjects = jTokens as JObject;
					if (jObjects != null)
					{
						if (this.Name == null)
						{
							using (IEnumerator<KeyValuePair<string, JToken>> enumerator1 = jObjects.GetEnumerator())
							{
								while (enumerator1.MoveNext())
								{
									yield return enumerator1.Current.Value;
								}
							}
							enumerator1 = null;
						}
						else
						{
							JToken item = jObjects[this.Name];
							if (item != null)
							{
								yield return item;
							}
							else if (errorWhenNoMatch)
							{
								throw new JsonException("Property '{0}' does not exist on JObject.".FormatWith(CultureInfo.InvariantCulture, this.Name));
							}
						}
					}
					else if (errorWhenNoMatch)
					{
						invariantCulture = CultureInfo.InvariantCulture;
						name = this.Name;
						if (name == null)
						{
							str = name;
							name = "*";
						}
						throw new JsonException("Property '{0}' not valid on {1}.".FormatWith(invariantCulture, name, jTokens.GetType().Name));
					}
					jObjects = null;
					jTokens = null;
				}
				enumerator = null;
				yield break;
				throw new JsonException("Property '{0}' does not exist on JObject.".FormatWith(CultureInfo.InvariantCulture, this.Name));
				invariantCulture = CultureInfo.InvariantCulture;
				name = this.Name;
				if (name == null)
				{
					str = name;
					name = "*";
				}
				throw new JsonException("Property '{0}' not valid on {1}.".FormatWith(invariantCulture, name, jTokens.GetType().Name));
			}
			enumerator = null;
			yield break;
			throw new JsonException("Property '{0}' does not exist on JObject.".FormatWith(CultureInfo.InvariantCulture, this.Name));
		}
	}
}