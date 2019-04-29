using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Shims;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Newtonsoft.Json.Converters
{
	[Preserve]
	public class VectorConverter : JsonConverter
	{
		private readonly static Type V2;

		private readonly static Type V3;

		private readonly static Type V4;

		public bool EnableVector2
		{
			get;
			set;
		}

		public bool EnableVector3
		{
			get;
			set;
		}

		public bool EnableVector4
		{
			get;
			set;
		}

		static VectorConverter()
		{
			VectorConverter.V2 = typeof(Vector2);
			VectorConverter.V3 = typeof(Vector3);
			VectorConverter.V4 = typeof(Vector4);
		}

		public VectorConverter()
		{
			this.EnableVector2 = true;
			this.EnableVector3 = true;
			this.EnableVector4 = true;
		}

		public VectorConverter(bool enableVector2, bool enableVector3, bool enableVector4) : this()
		{
			this.EnableVector2 = enableVector2;
			this.EnableVector3 = enableVector3;
			this.EnableVector4 = enableVector4;
		}

		public override bool CanConvert(Type objectType)
		{
			if (this.EnableVector2 && objectType == VectorConverter.V2 || this.EnableVector3 && objectType == VectorConverter.V3)
			{
				return true;
			}
			if (!this.EnableVector4)
			{
				return false;
			}
			return objectType == VectorConverter.V4;
		}

		private static Vector2 PopulateVector2(JsonReader reader)
		{
			Vector2 vector2 = new Vector2();
			if (reader.TokenType != JsonToken.Null)
			{
				JObject jObjects = JObject.Load(reader);
				vector2.x = jObjects["x"].Value<float>();
				vector2.y = jObjects["y"].Value<float>();
			}
			return vector2;
		}

		private static Vector3 PopulateVector3(JsonReader reader)
		{
			Vector3 vector3 = new Vector3();
			if (reader.TokenType != JsonToken.Null)
			{
				JObject jObjects = JObject.Load(reader);
				vector3.x = jObjects["x"].Value<float>();
				vector3.y = jObjects["y"].Value<float>();
				vector3.z = jObjects["z"].Value<float>();
			}
			return vector3;
		}

		private static Vector4 PopulateVector4(JsonReader reader)
		{
			Vector4 vector4 = new Vector4();
			if (reader.TokenType != JsonToken.Null)
			{
				JObject jObjects = JObject.Load(reader);
				vector4.x = jObjects["x"].Value<float>();
				vector4.y = jObjects["y"].Value<float>();
				vector4.z = jObjects["z"].Value<float>();
				vector4.w = jObjects["w"].Value<float>();
			}
			return vector4;
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (objectType == VectorConverter.V2)
			{
				return VectorConverter.PopulateVector2(reader);
			}
			if (objectType == VectorConverter.V3)
			{
				return VectorConverter.PopulateVector3(reader);
			}
			return VectorConverter.PopulateVector4(reader);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			float? nullable;
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			Type type = value.GetType();
			if (type == VectorConverter.V2)
			{
				Vector2 vector2 = (Vector2)value;
				nullable = null;
				float? nullable1 = nullable;
				nullable = null;
				VectorConverter.WriteVector(writer, vector2.x, vector2.y, nullable1, nullable);
				return;
			}
			if (type == VectorConverter.V3)
			{
				Vector3 vector3 = (Vector3)value;
				nullable = null;
				VectorConverter.WriteVector(writer, vector3.x, vector3.y, new float?(vector3.z), nullable);
				return;
			}
			if (type != VectorConverter.V4)
			{
				writer.WriteNull();
				return;
			}
			Vector4 vector4 = (Vector4)value;
			VectorConverter.WriteVector(writer, vector4.x, vector4.y, new float?(vector4.z), new float?(vector4.w));
		}

		private static void WriteVector(JsonWriter writer, float x, float y, float? z, float? w)
		{
			writer.WriteStartObject();
			writer.WritePropertyName("x");
			writer.WriteValue(x);
			writer.WritePropertyName("y");
			writer.WriteValue(y);
			if (z.HasValue)
			{
				writer.WritePropertyName("z");
				writer.WriteValue(z.Value);
				if (w.HasValue)
				{
					writer.WritePropertyName("w");
					writer.WriteValue(w.Value);
				}
			}
			writer.WriteEndObject();
		}
	}
}