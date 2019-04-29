using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace Newtonsoft.Json.Converters
{
	public class Matrix4x4Converter : JsonConverter
	{
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public Matrix4x4Converter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Matrix4x4);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			Matrix4x4 matrix4x4;
			if (reader.TokenType == JsonToken.Null)
			{
				matrix4x4 = new Matrix4x4();
				return matrix4x4;
			}
			JObject jObjects = JObject.Load(reader);
			matrix4x4 = new Matrix4x4()
			{
				m00 = (float)((float)jObjects["m00"]),
				m01 = (float)((float)jObjects["m01"]),
				m02 = (float)((float)jObjects["m02"]),
				m03 = (float)((float)jObjects["m03"]),
				m20 = (float)((float)jObjects["m20"]),
				m21 = (float)((float)jObjects["m21"]),
				m22 = (float)((float)jObjects["m22"]),
				m23 = (float)((float)jObjects["m23"]),
				m30 = (float)((float)jObjects["m30"]),
				m31 = (float)((float)jObjects["m31"]),
				m32 = (float)((float)jObjects["m32"]),
				m33 = (float)((float)jObjects["m33"])
			};
			return matrix4x4;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			Matrix4x4 matrix4x4 = (Matrix4x4)value;
			writer.WriteStartObject();
			writer.WritePropertyName("m00");
			writer.WriteValue(matrix4x4.m00);
			writer.WritePropertyName("m01");
			writer.WriteValue(matrix4x4.m01);
			writer.WritePropertyName("m02");
			writer.WriteValue(matrix4x4.m02);
			writer.WritePropertyName("m03");
			writer.WriteValue(matrix4x4.m03);
			writer.WritePropertyName("m10");
			writer.WriteValue(matrix4x4.m10);
			writer.WritePropertyName("m11");
			writer.WriteValue(matrix4x4.m11);
			writer.WritePropertyName("m12");
			writer.WriteValue(matrix4x4.m12);
			writer.WritePropertyName("m13");
			writer.WriteValue(matrix4x4.m13);
			writer.WritePropertyName("m20");
			writer.WriteValue(matrix4x4.m20);
			writer.WritePropertyName("m21");
			writer.WriteValue(matrix4x4.m21);
			writer.WritePropertyName("m22");
			writer.WriteValue(matrix4x4.m22);
			writer.WritePropertyName("m23");
			writer.WriteValue(matrix4x4.m23);
			writer.WritePropertyName("m30");
			writer.WriteValue(matrix4x4.m30);
			writer.WritePropertyName("m31");
			writer.WriteValue(matrix4x4.m31);
			writer.WritePropertyName("m32");
			writer.WriteValue(matrix4x4.m32);
			writer.WritePropertyName("m33");
			writer.WriteValue(matrix4x4.m33);
			writer.WriteEnd();
		}
	}
}