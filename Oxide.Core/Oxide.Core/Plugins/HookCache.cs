using System;
using System.Collections.Generic;

namespace Oxide.Core.Plugins
{
	public class HookCache
	{
		private string NullKey = "null";

		public Dictionary<string, HookCache> _cache = new Dictionary<string, HookCache>();

		public List<HookMethod> _methods;

		public HookCache()
		{
		}

		public List<HookMethod> GetHookMethod(string hookName, object[] args, out HookCache cache)
		{
			HookCache hookCache;
			if (!this._cache.TryGetValue(hookName, out hookCache))
			{
				hookCache = new HookCache();
				this._cache.Add(hookName, hookCache);
			}
			return hookCache.GetHookMethod(args, 0, out cache);
		}

		public List<HookMethod> GetHookMethod(object[] args, int index, out HookCache cache)
		{
			HookCache hookCache;
			if (args == null || index >= (int)args.Length)
			{
				cache = this;
				return this._methods;
			}
			if (args[index] == null)
			{
				if (!this._cache.TryGetValue(this.NullKey, out hookCache))
				{
					hookCache = new HookCache();
					this._cache.Add(this.NullKey, hookCache);
				}
			}
			else if (!this._cache.TryGetValue(args[index].GetType().FullName, out hookCache))
			{
				hookCache = new HookCache();
				this._cache.Add(args[index].GetType().FullName, hookCache);
			}
			return hookCache.GetHookMethod(args, index + 1, out cache);
		}

		public void SetupMethods(List<HookMethod> methods)
		{
			this._methods = methods;
		}
	}
}