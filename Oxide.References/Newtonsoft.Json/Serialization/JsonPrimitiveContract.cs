using Newtonsoft.Json;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public class JsonPrimitiveContract : JsonContract
	{
		private readonly static Dictionary<Type, ReadType> ReadTypeMap;

		internal PrimitiveTypeCode TypeCode
		{
			get;
			set;
		}

		static JsonPrimitiveContract()
		{
			Dictionary<Type, ReadType> types = new Dictionary<Type, ReadType>();
			types[typeof(byte[])] = ReadType.ReadAsBytes;
			types[typeof(byte)] = ReadType.ReadAsInt32;
			types[typeof(short)] = ReadType.ReadAsInt32;
			types[typeof(int)] = ReadType.ReadAsInt32;
			types[typeof(decimal)] = ReadType.ReadAsDecimal;
			types[typeof(bool)] = ReadType.ReadAsBoolean;
			types[typeof(string)] = ReadType.ReadAsString;
			types[typeof(DateTime)] = ReadType.ReadAsDateTime;
			types[typeof(DateTimeOffset)] = ReadType.ReadAsDateTimeOffset;
			types[typeof(float)] = ReadType.ReadAsDouble;
			types[typeof(double)] = ReadType.ReadAsDouble;
			JsonPrimitiveContract.ReadTypeMap = types;
		}

		public JsonPrimitiveContract(Type underlyingType) : base(underlyingType)
		{
			ReadType readType;
			this.ContractType = JsonContractType.Primitive;
			this.TypeCode = ConvertUtils.GetTypeCode(underlyingType);
			this.IsReadOnlyOrFixedSize = true;
			if (JsonPrimitiveContract.ReadTypeMap.TryGetValue(this.NonNullableUnderlyingType, out readType))
			{
				this.InternalReadType = readType;
			}
		}
	}
}