using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Facepunch
{
	public static class Pool
	{
		public static Dictionary<Type, Pool.ICollection> directory;

		static Pool()
		{
			Pool.directory = new Dictionary<Type, Pool.ICollection>();
		}

		public static void Clear()
		{
			foreach (KeyValuePair<Type, Pool.ICollection> keyValuePair in Pool.directory)
			{
				keyValuePair.Value.Reset();
			}
		}

		public static void FillBuffer<T>(int count = 2147483647)
		where T : class, new()
		{
			Pool.PoolCollection<T> poolCollection = Pool.FindCollection<T>();
			for (int i = 0; i < count && poolCollection.ItemsInStack < (long)((int)poolCollection.buffer.Length); i++)
			{
				poolCollection.buffer[checked((IntPtr)poolCollection.ItemsInStack)] = Activator.CreateInstance<T>();
				Pool.PoolCollection<T> itemsInStack = poolCollection;
				itemsInStack.ItemsInStack = itemsInStack.ItemsInStack + (long)1;
			}
		}

		public static Pool.PoolCollection<T> FindCollection<T>()
		where T : class
		{
			Pool.PoolCollection<T> collection = Pool<T>.Collection;
			if (collection == null)
			{
				collection = new Pool.PoolCollection<T>();
				Pool<T>.Collection = collection;
				Pool.directory.Add(typeof(T), Pool<T>.Collection);
			}
			return collection;
		}

		public static void Free<T>(ref T obj)
		where T : class
		{
			if (obj == null)
			{
				throw new ArgumentNullException();
			}
			Pool.FreeInternal<T>(ref obj);
		}

		private static void FreeInternal<T>(ref T obj)
		where T : class
		{
			Pool.PoolCollection<T> poolCollection = Pool.FindCollection<T>();
			if (poolCollection.ItemsInStack >= (long)((int)poolCollection.buffer.Length))
			{
				Pool.PoolCollection<T> itemsSpilled = poolCollection;
				itemsSpilled.ItemsSpilled = itemsSpilled.ItemsSpilled + (long)1;
				Pool.PoolCollection<T> itemsInUse = poolCollection;
				itemsInUse.ItemsInUse = itemsInUse.ItemsInUse - (long)1;
				obj = default(T);
				return;
			}
			poolCollection.buffer[checked((IntPtr)poolCollection.ItemsInStack)] = obj;
			Pool.PoolCollection<T> itemsInStack = poolCollection;
			itemsInStack.ItemsInStack = itemsInStack.ItemsInStack + (long)1;
			Pool.PoolCollection<T> itemsInUse1 = poolCollection;
			itemsInUse1.ItemsInUse = itemsInUse1.ItemsInUse - (long)1;
			Pool.IPooled pooled = (object)obj as Pool.IPooled;
			if (pooled != null)
			{
				pooled.EnterPool();
			}
			obj = default(T);
		}

		public static void FreeList<T>(ref List<T> obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException();
			}
			obj.Clear();
			Pool.FreeInternal<List<T>>(ref obj);
			if (obj != null)
			{
				throw new SystemException("Pool.Free is not setting object to NULL");
			}
		}

		public static void FreeMemoryStream(ref MemoryStream obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException();
			}
			obj.Position = (long)0;
			obj.SetLength((long)0);
			Pool.FreeInternal<MemoryStream>(ref obj);
			if (obj != null)
			{
				throw new SystemException("Pool.Free is not setting object to NULL");
			}
		}

		public static T Get<T>()
		where T : class, new()
		{
			Pool.PoolCollection<T> poolCollection = Pool.FindCollection<T>();
			if (poolCollection.ItemsInStack <= (long)0)
			{
				Pool.PoolCollection<T> itemsCreated = poolCollection;
				itemsCreated.ItemsCreated = itemsCreated.ItemsCreated + (long)1;
				Pool.PoolCollection<T> itemsInUse = poolCollection;
				itemsInUse.ItemsInUse = itemsInUse.ItemsInUse + (long)1;
				return Activator.CreateInstance<T>();
			}
			Pool.PoolCollection<T> itemsInStack = poolCollection;
			itemsInStack.ItemsInStack = itemsInStack.ItemsInStack - (long)1;
			Pool.PoolCollection<T> itemsInUse1 = poolCollection;
			itemsInUse1.ItemsInUse = itemsInUse1.ItemsInUse + (long)1;
			T t = poolCollection.buffer[checked((IntPtr)poolCollection.ItemsInStack)];
			poolCollection.buffer[checked((IntPtr)poolCollection.ItemsInStack)] = default(T);
			Pool.IPooled pooled = (object)t as Pool.IPooled;
			if (pooled != null)
			{
				pooled.LeavePool();
			}
			Pool.PoolCollection<T> itemsTaken = poolCollection;
			itemsTaken.ItemsTaken = itemsTaken.ItemsTaken + (long)1;
			return t;
		}

		public static List<T> GetList<T>()
		{
			return Pool.Get<List<T>>();
		}

		public static void ResizeBuffer<T>(int size)
		where T : class
		{
			Array.Resize<T>(ref Pool.FindCollection<T>().buffer, size);
		}

		public interface ICollection
		{
			long ItemsCreated
			{
				get;
			}

			long ItemsInStack
			{
				get;
			}

			long ItemsInUse
			{
				get;
			}

			long ItemsSpilled
			{
				get;
			}

			long ItemsTaken
			{
				get;
			}

			void Reset();
		}

		public interface IPooled
		{
			void EnterPool();

			void LeavePool();
		}

		public class PoolCollection<T> : Pool.ICollection
		{
			public T[] buffer;

			public long ItemsCreated
			{
				get
				{
					return get_ItemsCreated();
				}
				set
				{
					set_ItemsCreated(value);
				}
			}

			private long <ItemsCreated>k__BackingField;

			public long get_ItemsCreated()
			{
				return this.<ItemsCreated>k__BackingField;
			}

			public void set_ItemsCreated(long value)
			{
				this.<ItemsCreated>k__BackingField = value;
			}

			public long ItemsInStack
			{
				get
				{
					return get_ItemsInStack();
				}
				set
				{
					set_ItemsInStack(value);
				}
			}

			private long <ItemsInStack>k__BackingField;

			public long get_ItemsInStack()
			{
				return this.<ItemsInStack>k__BackingField;
			}

			public void set_ItemsInStack(long value)
			{
				this.<ItemsInStack>k__BackingField = value;
			}

			public long ItemsInUse
			{
				get
				{
					return get_ItemsInUse();
				}
				set
				{
					set_ItemsInUse(value);
				}
			}

			private long <ItemsInUse>k__BackingField;

			public long get_ItemsInUse()
			{
				return this.<ItemsInUse>k__BackingField;
			}

			public void set_ItemsInUse(long value)
			{
				this.<ItemsInUse>k__BackingField = value;
			}

			public long ItemsSpilled
			{
				get
				{
					return get_ItemsSpilled();
				}
				set
				{
					set_ItemsSpilled(value);
				}
			}

			private long <ItemsSpilled>k__BackingField;

			public long get_ItemsSpilled()
			{
				return this.<ItemsSpilled>k__BackingField;
			}

			public void set_ItemsSpilled(long value)
			{
				this.<ItemsSpilled>k__BackingField = value;
			}

			public long ItemsTaken
			{
				get
				{
					return get_ItemsTaken();
				}
				set
				{
					set_ItemsTaken(value);
				}
			}

			private long <ItemsTaken>k__BackingField;

			public long get_ItemsTaken()
			{
				return this.<ItemsTaken>k__BackingField;
			}

			public void set_ItemsTaken(long value)
			{
				this.<ItemsTaken>k__BackingField = value;
			}

			public PoolCollection()
			{
				this.Reset();
			}

			public void Reset()
			{
				this.buffer = new T[512];
				this.ItemsInStack = (long)0;
				this.ItemsInUse = (long)0;
				this.ItemsCreated = (long)0;
				this.ItemsTaken = (long)0;
				this.ItemsSpilled = (long)0;
			}
		}
	}
}