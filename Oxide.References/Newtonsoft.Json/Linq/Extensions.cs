using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Newtonsoft.Json.Linq
{
	[Preserve]
	public static class Extensions
	{
		public static IJEnumerable<JToken> Ancestors<T>(this IEnumerable<T> source)
		where T : JToken
		{
			ValidationUtils.ArgumentNotNull(source, "source");
			return source.SelectMany<T, JToken>((T j) => j.Ancestors()).AsJEnumerable();
		}

		public static IJEnumerable<JToken> AncestorsAndSelf<T>(this IEnumerable<T> source)
		where T : JToken
		{
			ValidationUtils.ArgumentNotNull(source, "source");
			return source.SelectMany<T, JToken>((T j) => j.AncestorsAndSelf()).AsJEnumerable();
		}

		public static IJEnumerable<JToken> AsJEnumerable(this IEnumerable<JToken> source)
		{
			return source.AsJEnumerable<JToken>();
		}

		public static IJEnumerable<T> AsJEnumerable<T>(this IEnumerable<T> source)
		where T : JToken
		{
			if (source == null)
			{
				return null;
			}
			if (source is IJEnumerable<T>)
			{
				return (IJEnumerable<T>)source;
			}
			return new JEnumerable<T>(source);
		}

		public static IJEnumerable<JToken> Children<T>(this IEnumerable<T> source)
		where T : JToken
		{
			return source.Children<T, JToken>().AsJEnumerable();
		}

		public static IEnumerable<U> Children<T, U>(this IEnumerable<T> source)
		where T : JToken
		{
			ValidationUtils.ArgumentNotNull(source, "source");
			return source.SelectMany<T, JToken>((T c) => c.Children()).Convert<JToken, U>();
		}

		internal static IEnumerable<U> Convert<T, U>(this IEnumerable<T> source)
		where T : JToken
		{
			ValidationUtils.ArgumentNotNull(source, "source");
			foreach (T t in source)
			{
				yield return t.Convert<JToken, U>();
			}
		}

		internal static U Convert<T, U>(this T token)
		where T : JToken
		{
			U u;
			if (token == null)
			{
				u = default(U);
				return u;
			}
			if ((object)token is U && typeof(U) != typeof(IComparable) && typeof(U) != typeof(IFormattable))
			{
				return (U)(object)token;
			}
			JValue jValue = (object)token as JValue;
			if (jValue == null)
			{
				throw new InvalidCastException("Cannot cast {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, token.GetType(), typeof(T)));
			}
			if (jValue.Value is U)
			{
				return (U)jValue.Value;
			}
			Type underlyingType = typeof(U);
			if (ReflectionUtils.IsNullableType(underlyingType))
			{
				if (jValue.Value == null)
				{
					u = default(U);
					return u;
				}
				underlyingType = Nullable.GetUnderlyingType(underlyingType);
			}
			return (U)Convert.ChangeType(jValue.Value, underlyingType, CultureInfo.InvariantCulture);
		}

		public static IJEnumerable<JToken> Descendants<T>(this IEnumerable<T> source)
		where T : JContainer
		{
			ValidationUtils.ArgumentNotNull(source, "source");
			return source.SelectMany<T, JToken>((T j) => j.Descendants()).AsJEnumerable();
		}

		public static IJEnumerable<JToken> DescendantsAndSelf<T>(this IEnumerable<T> source)
		where T : JContainer
		{
			ValidationUtils.ArgumentNotNull(source, "source");
			return source.SelectMany<T, JToken>((T j) => j.DescendantsAndSelf()).AsJEnumerable();
		}

		public static IJEnumerable<JProperty> Properties(this IEnumerable<JObject> source)
		{
			ValidationUtils.ArgumentNotNull(source, "source");
			return source.SelectMany<JObject, JProperty>((JObject d) => d.Properties()).AsJEnumerable<JProperty>();
		}

		public static U Value<U>(this IEnumerable<JToken> value)
		{
			return value.Value<JToken, U>();
		}

		public static U Value<T, U>(this IEnumerable<T> value)
		where T : JToken
		{
			ValidationUtils.ArgumentNotNull(value, "value");
			JToken jTokens = value as JToken;
			if (jTokens == null)
			{
				throw new ArgumentException("Source value must be a JToken.");
			}
			return jTokens.Convert<JToken, U>();
		}

		public static IJEnumerable<JToken> Values(this IEnumerable<JToken> source, object key)
		{
			return source.Values<JToken, JToken>(key).AsJEnumerable();
		}

		public static IJEnumerable<JToken> Values(this IEnumerable<JToken> source)
		{
			return source.Values(null);
		}

		public static IEnumerable<U> Values<U>(this IEnumerable<JToken> source, object key)
		{
			return source.Values<JToken, U>(key);
		}

		public static IEnumerable<U> Values<U>(this IEnumerable<JToken> source)
		{
			return source.Values<JToken, U>(null);
		}

		internal static IEnumerable<U> Values<T, U>(this IEnumerable<T> source, object key)
		where T : JToken
		{
			ValidationUtils.ArgumentNotNull(source, "source");
			foreach (T t in source)
			{
				JToken jTokens = t;
				if (key != null)
				{
					JToken item = jTokens[key];
					if (item != null)
					{
						yield return item.Convert<JToken, U>();
					}
				}
				else if (!(jTokens is JValue))
				{
					foreach (JToken jTokens1 in jTokens.Children())
					{
						yield return jTokens1.Convert<JToken, U>();
					}
				}
				else
				{
					yield return ((JValue)jTokens).Convert<JValue, U>();
				}
				jTokens = null;
			}
		}
	}
}