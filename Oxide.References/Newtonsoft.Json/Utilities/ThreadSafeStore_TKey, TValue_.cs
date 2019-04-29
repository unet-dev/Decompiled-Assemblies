using Newtonsoft.Json.Shims;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal class ThreadSafeStore<TKey, TValue>
	{
		private readonly object _lock;

		private Dictionary<TKey, TValue> _store;

		private readonly Func<TKey, TValue> _creator;

		[Preserve]
		public ThreadSafeStore(Func<TKey, TValue> creator)
		{
			if (creator == null)
			{
				throw new ArgumentNullException("creator");
			}
			this._creator = creator;
			this._store = new Dictionary<TKey, TValue>();
		}

		[Preserve]
		private TValue AddValue(TKey key)
		{
			TValue tValue;
			TValue tValue1;
			TValue tValue2 = this._creator(key);
			object obj = this._lock;
			Monitor.Enter(obj);
			try
			{
				if (this._store == null)
				{
					this._store = new Dictionary<TKey, TValue>();
					this._store[key] = tValue2;
				}
				else if (!this._store.TryGetValue(key, out tValue))
				{
					Dictionary<TKey, TValue> tKeys = new Dictionary<TKey, TValue>(this._store);
					tKeys[key] = tValue2;
					this._store = tKeys;
				}
				else
				{
					tValue1 = tValue;
					return tValue1;
				}
				tValue1 = tValue2;
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return tValue1;
		}

		[Preserve]
		public TValue Get(TKey key)
		{
			TValue tValue;
			if (this._store.TryGetValue(key, out tValue))
			{
				return tValue;
			}
			return this.AddValue(key);
		}
	}
}