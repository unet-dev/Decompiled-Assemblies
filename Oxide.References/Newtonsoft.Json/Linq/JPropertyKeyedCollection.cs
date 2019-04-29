using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Newtonsoft.Json.Linq
{
	[Preserve]
	internal class JPropertyKeyedCollection : Collection<JToken>
	{
		private readonly static IEqualityComparer<string> Comparer;

		private Dictionary<string, JToken> _dictionary;

		public JToken this[string key]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				if (this._dictionary == null)
				{
					throw new KeyNotFoundException();
				}
				return this._dictionary[key];
			}
		}

		public ICollection<string> Keys
		{
			get
			{
				this.EnsureDictionary();
				return this._dictionary.Keys;
			}
		}

		public ICollection<JToken> Values
		{
			get
			{
				this.EnsureDictionary();
				return this._dictionary.Values;
			}
		}

		static JPropertyKeyedCollection()
		{
			JPropertyKeyedCollection.Comparer = StringComparer.Ordinal;
		}

		public JPropertyKeyedCollection() : base(new List<JToken>())
		{
		}

		private void AddKey(string key, JToken item)
		{
			this.EnsureDictionary();
			this._dictionary[key] = item;
		}

		protected void ChangeItemKey(JToken item, string newKey)
		{
			if (!this.ContainsItem(item))
			{
				throw new ArgumentException("The specified item does not exist in this KeyedCollection.");
			}
			string keyForItem = this.GetKeyForItem(item);
			if (!JPropertyKeyedCollection.Comparer.Equals(keyForItem, newKey))
			{
				if (newKey != null)
				{
					this.AddKey(newKey, item);
				}
				if (keyForItem != null)
				{
					this.RemoveKey(keyForItem);
				}
			}
		}

		protected override void ClearItems()
		{
			base.ClearItems();
			if (this._dictionary != null)
			{
				this._dictionary.Clear();
			}
		}

		public bool Compare(JPropertyKeyedCollection other)
		{
			JToken jTokens;
			bool value;
			if (this == other)
			{
				return true;
			}
			Dictionary<string, JToken> strs = this._dictionary;
			Dictionary<string, JToken> strs1 = other._dictionary;
			if (strs == null && strs1 == null)
			{
				return true;
			}
			if (strs == null)
			{
				return strs1.Count == 0;
			}
			if (strs1 == null)
			{
				return strs.Count == 0;
			}
			if (strs.Count != strs1.Count)
			{
				return false;
			}
			Dictionary<string, JToken>.Enumerator enumerator = strs.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, JToken> current = enumerator.Current;
					if (strs1.TryGetValue(current.Key, out jTokens))
					{
						JProperty jProperty = (JProperty)current.Value;
						JProperty jProperty1 = (JProperty)jTokens;
						if (jProperty.Value != null)
						{
							if (jProperty.Value.DeepEquals(jProperty1.Value))
							{
								continue;
							}
							value = false;
							return value;
						}
						else
						{
							value = jProperty1.Value == null;
							return value;
						}
					}
					else
					{
						value = false;
						return value;
					}
				}
				return true;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return value;
		}

		public bool Contains(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (this._dictionary == null)
			{
				return false;
			}
			return this._dictionary.ContainsKey(key);
		}

		private bool ContainsItem(JToken item)
		{
			JToken jTokens;
			if (this._dictionary == null)
			{
				return false;
			}
			string keyForItem = this.GetKeyForItem(item);
			return this._dictionary.TryGetValue(keyForItem, out jTokens);
		}

		private void EnsureDictionary()
		{
			if (this._dictionary == null)
			{
				this._dictionary = new Dictionary<string, JToken>(JPropertyKeyedCollection.Comparer);
			}
		}

		private string GetKeyForItem(JToken item)
		{
			return ((JProperty)item).Name;
		}

		public int IndexOfReference(JToken t)
		{
			return ((List<JToken>)base.Items).IndexOfReference<JToken>(t);
		}

		protected override void InsertItem(int index, JToken item)
		{
			this.AddKey(this.GetKeyForItem(item), item);
			base.InsertItem(index, item);
		}

		public bool Remove(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (this._dictionary == null)
			{
				return false;
			}
			if (!this._dictionary.ContainsKey(key))
			{
				return false;
			}
			return base.Remove(this._dictionary[key]);
		}

		protected override void RemoveItem(int index)
		{
			string keyForItem = this.GetKeyForItem(base.Items[index]);
			this.RemoveKey(keyForItem);
			base.RemoveItem(index);
		}

		private void RemoveKey(string key)
		{
			if (this._dictionary != null)
			{
				this._dictionary.Remove(key);
			}
		}

		protected override void SetItem(int index, JToken item)
		{
			string keyForItem = this.GetKeyForItem(item);
			string str = this.GetKeyForItem(base.Items[index]);
			if (!JPropertyKeyedCollection.Comparer.Equals(str, keyForItem))
			{
				this.AddKey(keyForItem, item);
				if (str != null)
				{
					this.RemoveKey(str);
				}
			}
			else if (this._dictionary != null)
			{
				this._dictionary[keyForItem] = item;
			}
			base.SetItem(index, item);
		}

		public bool TryGetValue(string key, out JToken value)
		{
			if (this._dictionary == null)
			{
				value = null;
				return false;
			}
			return this._dictionary.TryGetValue(key, out value);
		}
	}
}