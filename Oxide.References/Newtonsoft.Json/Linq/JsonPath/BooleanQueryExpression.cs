using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Linq.JsonPath
{
	[Preserve]
	internal class BooleanQueryExpression : QueryExpression
	{
		public List<PathFilter> Path
		{
			get;
			set;
		}

		public JValue Value
		{
			get;
			set;
		}

		public BooleanQueryExpression()
		{
		}

		private bool EqualsWithStringCoercion(JValue value, JValue queryValue)
		{
			string str;
			if (value.Equals(queryValue))
			{
				return true;
			}
			if (queryValue.Type != JTokenType.String)
			{
				return false;
			}
			string str1 = (string)queryValue.Value;
			switch (value.Type)
			{
				case JTokenType.Date:
				{
					using (StringWriter stringWriter = StringUtils.CreateStringWriter(64))
					{
						if (!(value.Value is DateTimeOffset))
						{
							DateTimeUtils.WriteDateTimeString(stringWriter, (DateTime)value.Value, DateFormatHandling.IsoDateFormat, null, CultureInfo.InvariantCulture);
						}
						else
						{
							DateTimeUtils.WriteDateTimeOffsetString(stringWriter, (DateTimeOffset)value.Value, DateFormatHandling.IsoDateFormat, null, CultureInfo.InvariantCulture);
						}
						str = stringWriter.ToString();
						break;
					}
					break;
				}
				case JTokenType.Raw:
				{
					return false;
				}
				case JTokenType.Bytes:
				{
					str = Convert.ToBase64String((byte[])value.Value);
					break;
				}
				case JTokenType.Guid:
				case JTokenType.TimeSpan:
				{
					str = value.Value.ToString();
					break;
				}
				case JTokenType.Uri:
				{
					str = ((Uri)value.Value).OriginalString;
					break;
				}
				default:
				{
					return false;
				}
			}
			return string.Equals(str, str1, StringComparison.Ordinal);
		}

		public override bool IsMatch(JToken t)
		{
			QueryOperator @operator;
			bool flag;
			using (IEnumerator<JToken> enumerator = JPath.Evaluate(this.Path, t, false).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					JValue current = enumerator.Current as JValue;
					if (current == null)
					{
						@operator = base.Operator;
						if (@operator != QueryOperator.NotEquals && @operator != QueryOperator.Exists)
						{
							continue;
						}
						flag = true;
						return flag;
					}
					else
					{
						@operator = base.Operator;
						switch (@operator)
						{
							case QueryOperator.Equals:
							{
								if (!this.EqualsWithStringCoercion(current, this.Value))
								{
									continue;
								}
								flag = true;
								return flag;
							}
							case QueryOperator.NotEquals:
							{
								if (this.EqualsWithStringCoercion(current, this.Value))
								{
									continue;
								}
								flag = true;
								return flag;
							}
							case QueryOperator.Exists:
							{
								flag = true;
								return flag;
							}
							case QueryOperator.LessThan:
							{
								if (current.CompareTo(this.Value) >= 0)
								{
									continue;
								}
								flag = true;
								return flag;
							}
							case QueryOperator.LessThanOrEquals:
							{
								if (current.CompareTo(this.Value) > 0)
								{
									continue;
								}
								flag = true;
								return flag;
							}
							case QueryOperator.GreaterThan:
							{
								if (current.CompareTo(this.Value) <= 0)
								{
									continue;
								}
								flag = true;
								return flag;
							}
							case QueryOperator.GreaterThanOrEquals:
							{
								if (current.CompareTo(this.Value) < 0)
								{
									continue;
								}
								flag = true;
								return flag;
							}
							default:
							{
								continue;
							}
						}
					}
				}
				return false;
			}
			return flag;
		}
	}
}