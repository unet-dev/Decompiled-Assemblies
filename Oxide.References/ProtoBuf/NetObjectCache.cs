using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ProtoBuf
{
	internal sealed class NetObjectCache
	{
		internal const int Root = 0;

		private MutableList underlyingList;

		private object rootObject;

		private int trapStartIndex;

		private Dictionary<string, int> stringKeys;

		private Dictionary<object, int> objectKeys;

		private MutableList List
		{
			get
			{
				if (this.underlyingList == null)
				{
					this.underlyingList = new MutableList();
				}
				return this.underlyingList;
			}
		}

		public NetObjectCache()
		{
		}

		internal int AddObjectKey(object value, out bool existing)
		{
			int num;
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value == this.rootObject)
			{
				existing = true;
				return 0;
			}
			string str = value as string;
			BasicList list = this.List;
			if (str == null)
			{
				if (this.objectKeys == null)
				{
					this.objectKeys = new Dictionary<object, int>(NetObjectCache.ReferenceComparer.Default);
					num = -1;
				}
				else if (!this.objectKeys.TryGetValue(value, out num))
				{
					num = -1;
				}
			}
			else if (this.stringKeys == null)
			{
				this.stringKeys = new Dictionary<string, int>();
				num = -1;
			}
			else if (!this.stringKeys.TryGetValue(str, out num))
			{
				num = -1;
			}
			bool flag = num >= 0;
			bool flag1 = flag;
			existing = flag;
			if (!flag1)
			{
				num = list.Add(value);
				if (str != null)
				{
					this.stringKeys.Add(str, num);
				}
				else
				{
					this.objectKeys.Add(value, num);
				}
			}
			return num + 1;
		}

		internal void Clear()
		{
			this.trapStartIndex = 0;
			this.rootObject = null;
			if (this.underlyingList != null)
			{
				this.underlyingList.Clear();
			}
			if (this.stringKeys != null)
			{
				this.stringKeys.Clear();
			}
			if (this.objectKeys != null)
			{
				this.objectKeys.Clear();
			}
		}

		internal object GetKeyedObject(int key)
		{
			int num = key;
			key = num - 1;
			if (num == 0)
			{
				if (this.rootObject == null)
				{
					throw new ProtoException("No root object assigned");
				}
				return this.rootObject;
			}
			BasicList list = this.List;
			if (key < 0 || key >= list.Count)
			{
				throw new ProtoException("Internal error; a missing key occurred");
			}
			object item = list[key];
			if (item == null)
			{
				throw new ProtoException("A deferred key does not have a value yet");
			}
			return item;
		}

		internal void RegisterTrappedObject(object value)
		{
			if (this.rootObject == null)
			{
				this.rootObject = value;
				return;
			}
			if (this.underlyingList != null)
			{
				for (int i = this.trapStartIndex; i < this.underlyingList.Count; i++)
				{
					this.trapStartIndex = i + 1;
					if (this.underlyingList[i] == null)
					{
						this.underlyingList[i] = value;
						return;
					}
				}
			}
		}

		internal void SetKeyedObject(int key, object value)
		{
			int num = key;
			key = num - 1;
			if (num == 0)
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (this.rootObject != null && this.rootObject != value)
				{
					throw new ProtoException("The root object cannot be reassigned");
				}
				this.rootObject = value;
				return;
			}
			MutableList list = this.List;
			if (key < list.Count)
			{
				object item = list[key];
				if (item == null)
				{
					list[key] = value;
					return;
				}
				if (!object.ReferenceEquals(item, value))
				{
					throw new ProtoException("Reference-tracked objects cannot change reference");
				}
			}
			else if (key != list.Add(value))
			{
				throw new ProtoException("Internal error; a key mismatch occurred");
			}
		}

		private sealed class ReferenceComparer : IEqualityComparer<object>
		{
			public readonly static NetObjectCache.ReferenceComparer Default;

			static ReferenceComparer()
			{
				NetObjectCache.ReferenceComparer.Default = new NetObjectCache.ReferenceComparer();
			}

			private ReferenceComparer()
			{
			}

			bool System.Collections.Generic.IEqualityComparer<System.Object>.Equals(object x, object y)
			{
				return x == y;
			}

			int System.Collections.Generic.IEqualityComparer<System.Object>.GetHashCode(object obj)
			{
				return RuntimeHelpers.GetHashCode(obj);
			}
		}
	}
}