using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Oxide.Plugins
{
	public class Hash<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
	{
		private readonly IDictionary<TKey, TValue> dictionary;

		public int Count
		{
			get
			{
				return this.dictionary.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.dictionary.IsReadOnly;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				TValue tValue;
				if (this.TryGetValue(key, out tValue))
				{
					return tValue;
				}
				if (!typeof(TValue).IsValueType)
				{
					return default(TValue);
				}
				return (TValue)Activator.CreateInstance(typeof(TValue));
			}
			set
			{
				if (value == null)
				{
					this.dictionary.Remove(key);
					return;
				}
				this.dictionary[key] = value;
			}
		}

		public ICollection<TKey> Keys
		{
			get
			{
				return this.dictionary.Keys;
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				return this.dictionary.Values;
			}
		}

		public Hash()
		{
			this.dictionary = new Dictionary<TKey, TValue>();
		}

		public void Add(TKey key, TValue value)
		{
			this.dictionary.Add(key, value);
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			this.dictionary.Add(item);
		}

		public void Clear()
		{
			this.dictionary.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return this.dictionary.Contains(item);
		}

		public bool ContainsKey(TKey key)
		{
			return this.dictionary.ContainsKey(key);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
		{
			this.dictionary.CopyTo(array, index);
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return this.dictionary.GetEnumerator();
		}

		public bool Remove(TKey key)
		{
			return this.dictionary.Remove(key);
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			return this.dictionary.Remove(item);
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return this.dictionary.TryGetValue(key, out value);
		}
	}
}