using Newtonsoft.Json;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public class JsonContainerContract : JsonContract
	{
		private JsonContract _itemContract;

		private JsonContract _finalItemContract;

		internal JsonContract FinalItemContract
		{
			get
			{
				return this._finalItemContract;
			}
		}

		internal JsonContract ItemContract
		{
			get
			{
				return this._itemContract;
			}
			set
			{
				JsonContract jsonContract;
				this._itemContract = value;
				if (this._itemContract == null)
				{
					this._finalItemContract = null;
					return;
				}
				if (this._itemContract.UnderlyingType.IsSealed())
				{
					jsonContract = this._itemContract;
				}
				else
				{
					jsonContract = null;
				}
				this._finalItemContract = jsonContract;
			}
		}

		public JsonConverter ItemConverter
		{
			get;
			set;
		}

		public bool? ItemIsReference
		{
			get;
			set;
		}

		public ReferenceLoopHandling? ItemReferenceLoopHandling
		{
			get;
			set;
		}

		public TypeNameHandling? ItemTypeNameHandling
		{
			get;
			set;
		}

		internal JsonContainerContract(Type underlyingType) : base(underlyingType)
		{
			JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(underlyingType);
			if (cachedAttribute != null)
			{
				if (cachedAttribute.ItemConverterType != null)
				{
					this.ItemConverter = JsonTypeReflector.CreateJsonConverterInstance(cachedAttribute.ItemConverterType, cachedAttribute.ItemConverterParameters);
				}
				this.ItemIsReference = cachedAttribute._itemIsReference;
				this.ItemReferenceLoopHandling = cachedAttribute._itemReferenceLoopHandling;
				this.ItemTypeNameHandling = cachedAttribute._itemTypeNameHandling;
			}
		}
	}
}