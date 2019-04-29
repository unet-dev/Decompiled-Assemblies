using Newtonsoft.Json;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	internal class DefaultReferenceResolver : IReferenceResolver
	{
		private int _referenceCount;

		public DefaultReferenceResolver()
		{
		}

		public void AddReference(object context, string reference, object value)
		{
			this.GetMappings(context).Set(reference, value);
		}

		private BidirectionalDictionary<string, object> GetMappings(object context)
		{
			JsonSerializerInternalBase internalSerializer;
			if (!(context is JsonSerializerInternalBase))
			{
				if (!(context is JsonSerializerProxy))
				{
					throw new JsonException("The DefaultReferenceResolver can only be used internally.");
				}
				internalSerializer = ((JsonSerializerProxy)context).GetInternalSerializer();
			}
			else
			{
				internalSerializer = (JsonSerializerInternalBase)context;
			}
			return internalSerializer.DefaultReferenceMappings;
		}

		public string GetReference(object context, object value)
		{
			string str;
			BidirectionalDictionary<string, object> mappings = this.GetMappings(context);
			if (!mappings.TryGetBySecond(value, out str))
			{
				this._referenceCount++;
				str = this._referenceCount.ToString(CultureInfo.InvariantCulture);
				mappings.Set(str, value);
			}
			return str;
		}

		public bool IsReferenced(object context, object value)
		{
			string str;
			return this.GetMappings(context).TryGetBySecond(value, out str);
		}

		public object ResolveReference(object context, string reference)
		{
			object obj;
			this.GetMappings(context).TryGetByFirst(reference, out obj);
			return obj;
		}
	}
}