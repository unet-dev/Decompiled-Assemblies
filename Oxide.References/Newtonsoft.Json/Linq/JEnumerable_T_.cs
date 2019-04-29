using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Newtonsoft.Json.Linq
{
	[Preserve]
	public struct JEnumerable<T> : IJEnumerable<T>, IEnumerable<T>, IEnumerable
	where T : JToken
	{
		public readonly static JEnumerable<T> Empty;

		private readonly IEnumerable<T> _enumerable;

		public IJEnumerable<JToken> this[object key]
		{
			get
			{
				if (this._enumerable == null)
				{
					return (JEnumerable<JToken>)JEnumerable<JToken>.Empty;
				}
				return new JEnumerable<JToken>(this._enumerable.Values<T, JToken>(key));
			}
		}

		static JEnumerable()
		{
			JEnumerable<T>.Empty = new JEnumerable<T>(Enumerable.Empty<T>());
		}

		public JEnumerable(IEnumerable<T> enumerable)
		{
			ValidationUtils.ArgumentNotNull(enumerable, "enumerable");
			this._enumerable = enumerable;
		}

		public bool Equals(JEnumerable<T> other)
		{
			return object.Equals(this._enumerable, other._enumerable);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is JEnumerable<T>))
			{
				return false;
			}
			return this.Equals((JEnumerable<T>)obj);
		}

		public IEnumerator<T> GetEnumerator()
		{
			if (this._enumerable != null)
			{
				return this._enumerable.GetEnumerator();
			}
			return JEnumerable<T>.Empty.GetEnumerator();
		}

		public override int GetHashCode()
		{
			if (this._enumerable == null)
			{
				return 0;
			}
			return this._enumerable.GetHashCode();
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}