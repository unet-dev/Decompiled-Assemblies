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
	internal class ArraySliceFilter : PathFilter
	{
		public int? End
		{
			get;
			set;
		}

		public int? Start
		{
			get;
			set;
		}

		public int? Step
		{
			get;
			set;
		}

		public ArraySliceFilter()
		{
		}

		public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			int valueOrDefault;
			bool flag;
			int num;
			int valueOrDefault1;
			int num1;
			bool flag1;
			bool flag2;
			int num2;
			int num3;
			CultureInfo invariantCulture;
			object str;
			object obj;
			JToken jTokens;
			int? step = this.Step;
			flag = (step.GetValueOrDefault() == 0 ? step.HasValue : false);
			if (flag)
			{
				throw new JsonException("Step cannot be zero.");
			}
			using (IEnumerator<JToken> enumerator = current.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					jTokens = enumerator.Current;
					JArray jArrays = jTokens as JArray;
					if (jArrays != null)
					{
						step = this.Step;
						num = (step.HasValue ? step.GetValueOrDefault() : 1);
						int num4 = num;
						step = this.Start;
						if (step.HasValue)
						{
							valueOrDefault1 = step.GetValueOrDefault();
						}
						else
						{
							valueOrDefault1 = (num4 > 0 ? 0 : jArrays.Count - 1);
						}
						int count = valueOrDefault1;
						step = this.End;
						if (step.HasValue)
						{
							num1 = step.GetValueOrDefault();
						}
						else
						{
							num1 = (num4 > 0 ? jArrays.Count : -1);
						}
						int count1 = num1;
						step = this.Start;
						flag1 = (step.GetValueOrDefault() < 0 ? step.HasValue : false);
						if (flag1)
						{
							count = jArrays.Count + count;
						}
						step = this.End;
						flag2 = (step.GetValueOrDefault() < 0 ? step.HasValue : false);
						if (flag2)
						{
							count1 = jArrays.Count + count1;
						}
						int num5 = count;
						num2 = (num4 > 0 ? 0 : -2147483648);
						count = Math.Max(num5, num2);
						int num6 = count;
						num3 = (num4 > 0 ? jArrays.Count : jArrays.Count - 1);
						count = Math.Min(num6, num3);
						count1 = Math.Max(count1, -1);
						count1 = Math.Min(count1, jArrays.Count);
						bool flag3 = num4 > 0;
						if (this.IsValid(count, count1, flag3))
						{
							for (int i = count; this.IsValid(i, count1, flag3); i += num4)
							{
								yield return jArrays[i];
							}
						}
						else if (errorWhenNoMatch)
						{
							invariantCulture = CultureInfo.InvariantCulture;
							step = this.Start;
							if (step.HasValue)
							{
								step = this.Start;
								valueOrDefault = step.GetValueOrDefault();
								str = valueOrDefault.ToString(CultureInfo.InvariantCulture);
							}
							else
							{
								str = "*";
							}
							step = this.End;
							if (step.HasValue)
							{
								step = this.End;
								valueOrDefault = step.GetValueOrDefault();
								obj = valueOrDefault.ToString(CultureInfo.InvariantCulture);
							}
							else
							{
								obj = "*";
							}
							throw new JsonException("Array slice of {0} to {1} returned no results.".FormatWith(invariantCulture, str, obj));
						}
					}
					else if (errorWhenNoMatch)
					{
						throw new JsonException("Array slice is not valid on {0}.".FormatWith(CultureInfo.InvariantCulture, jTokens.GetType().Name));
					}
					jArrays = null;
					jTokens = null;
				}
				goto Label2;
				throw new JsonException("Array slice is not valid on {0}.".FormatWith(CultureInfo.InvariantCulture, jTokens.GetType().Name));
			}
		Label2:
			enumerator = null;
			yield break;
			invariantCulture = CultureInfo.InvariantCulture;
			step = this.Start;
			if (step.HasValue)
			{
				step = this.Start;
				valueOrDefault = step.GetValueOrDefault();
				str = valueOrDefault.ToString(CultureInfo.InvariantCulture);
			}
			else
			{
				str = "*";
			}
			step = this.End;
			if (step.HasValue)
			{
				step = this.End;
				valueOrDefault = step.GetValueOrDefault();
				obj = valueOrDefault.ToString(CultureInfo.InvariantCulture);
			}
			else
			{
				obj = "*";
			}
			throw new JsonException("Array slice of {0} to {1} returned no results.".FormatWith(invariantCulture, str, obj));
		}

		private bool IsValid(int index, int stopIndex, bool positiveStep)
		{
			if (positiveStep)
			{
				return index < stopIndex;
			}
			return index > stopIndex;
		}
	}
}