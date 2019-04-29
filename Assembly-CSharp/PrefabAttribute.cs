using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class PrefabAttribute : MonoBehaviour, IPrefabPreProcess
{
	[NonSerialized]
	public Vector3 worldPosition;

	[NonSerialized]
	public Quaternion worldRotation;

	[NonSerialized]
	public Vector3 worldForward;

	[NonSerialized]
	public Vector3 localPosition;

	[NonSerialized]
	public Vector3 localScale;

	[NonSerialized]
	public Quaternion localRotation;

	[NonSerialized]
	public string fullName;

	[NonSerialized]
	public string hierachyName;

	[NonSerialized]
	public uint prefabID;

	[NonSerialized]
	public int instanceID;

	[NonSerialized]
	public PrefabAttribute.Library prefabAttribute;

	[NonSerialized]
	public GameManager gameManager;

	[NonSerialized]
	public bool isServer;

	public static PrefabAttribute.Library server;

	public bool isClient
	{
		get
		{
			return !this.isServer;
		}
	}

	static PrefabAttribute()
	{
		PrefabAttribute.server = new PrefabAttribute.Library(false, true);
	}

	protected PrefabAttribute()
	{
	}

	protected virtual void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
	}

	internal static bool ComparePrefabAttribute(PrefabAttribute x, PrefabAttribute y)
	{
		bool flag = x == null;
		bool flag1 = y == null;
		if (flag & flag1)
		{
			return true;
		}
		if (flag | flag1)
		{
			return false;
		}
		return x.instanceID == y.instanceID;
	}

	public override bool Equals(object o)
	{
		return PrefabAttribute.ComparePrefabAttribute(this, (PrefabAttribute)o);
	}

	public override int GetHashCode()
	{
		if (this.hierachyName == null)
		{
			return base.GetHashCode();
		}
		return this.hierachyName.GetHashCode();
	}

	protected abstract Type GetIndexedType();

	public static bool operator ==(PrefabAttribute x, PrefabAttribute y)
	{
		return PrefabAttribute.ComparePrefabAttribute(x, y);
	}

	public static implicit operator Boolean(PrefabAttribute exists)
	{
		return exists != null;
	}

	public static bool operator !=(PrefabAttribute x, PrefabAttribute y)
	{
		return !PrefabAttribute.ComparePrefabAttribute(x, y);
	}

	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (bundling)
		{
			return;
		}
		this.fullName = name;
		this.hierachyName = base.transform.GetRecursiveName("");
		this.prefabID = StringPool.Get(name);
		this.instanceID = base.GetInstanceID();
		this.worldPosition = base.transform.position;
		this.worldRotation = base.transform.rotation;
		this.worldForward = base.transform.forward;
		this.localPosition = base.transform.localPosition;
		this.localScale = base.transform.localScale;
		this.localRotation = base.transform.localRotation;
		if (serverside)
		{
			this.prefabAttribute = PrefabAttribute.server;
			this.gameManager = GameManager.server;
			this.isServer = true;
		}
		this.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		if (serverside)
		{
			PrefabAttribute.server.Add(this.prefabID, this);
		}
		preProcess.RemoveComponent(this);
		preProcess.NominateForDeletion(base.gameObject);
	}

	public override string ToString()
	{
		if (this == null)
		{
			return "null";
		}
		return this.hierachyName;
	}

	public class AttributeCollection
	{
		private Dictionary<Type, List<PrefabAttribute>> attributes;

		private Dictionary<Type, object> cache;

		public AttributeCollection()
		{
		}

		public void Add(PrefabAttribute attribute)
		{
			List<PrefabAttribute> prefabAttributes = this.Find(attribute.GetIndexedType());
			Assert.IsTrue(!prefabAttributes.Contains(attribute), "AttributeCollection.Add: Adding twice to list");
			prefabAttributes.Add(attribute);
			this.cache = null;
		}

		internal List<PrefabAttribute> Find(Type t)
		{
			List<PrefabAttribute> prefabAttributes;
			if (this.attributes.TryGetValue(t, out prefabAttributes))
			{
				return prefabAttributes;
			}
			prefabAttributes = new List<PrefabAttribute>();
			this.attributes.Add(t, prefabAttributes);
			return prefabAttributes;
		}

		public T[] Find<T>()
		{
			object array;
			if (this.cache == null)
			{
				this.cache = new Dictionary<Type, object>();
			}
			if (this.cache.TryGetValue(typeof(T), out array))
			{
				return (T[])array;
			}
			array = this.Find(typeof(T)).Cast<T>().ToArray<T>();
			this.cache.Add(typeof(T), array);
			return (T[])array;
		}
	}

	public class Library
	{
		public bool clientside;

		public bool serverside;

		public Dictionary<uint, PrefabAttribute.AttributeCollection> prefabs;

		public Library(bool clientside, bool serverside)
		{
			this.clientside = clientside;
			this.serverside = serverside;
		}

		public void Add(uint prefabID, PrefabAttribute attribute)
		{
			this.Find(prefabID, false).Add(attribute);
		}

		public PrefabAttribute.AttributeCollection Find(uint prefabID, bool warmup = true)
		{
			PrefabAttribute.AttributeCollection attributeCollection;
			if (this.prefabs.TryGetValue(prefabID, out attributeCollection))
			{
				return attributeCollection;
			}
			attributeCollection = new PrefabAttribute.AttributeCollection();
			this.prefabs.Add(prefabID, attributeCollection);
			if (warmup && (!this.clientside || this.serverside))
			{
				if (!this.clientside && this.serverside)
				{
					GameManager.server.FindPrefab(prefabID);
				}
				else if (this.clientside)
				{
					bool flag = this.serverside;
				}
			}
			return attributeCollection;
		}

		public T Find<T>(uint prefabID)
		where T : PrefabAttribute
		{
			T[] tArray = this.Find(prefabID, true).Find<T>();
			if (tArray.Length == 0)
			{
				return default(T);
			}
			return tArray[0];
		}

		public T[] FindAll<T>(uint prefabID)
		where T : PrefabAttribute
		{
			return this.Find(prefabID, true).Find<T>();
		}
	}
}