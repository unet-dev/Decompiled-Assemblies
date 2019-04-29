using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Oxide.Game.Rust.Cui
{
	public class ComponentConverter : JsonConverter
	{
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public ComponentConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(ICuiComponent);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			Type type;
			JObject jObjects = JObject.Load(reader);
			string str = jObjects["type"].ToString();
			switch (str)
			{
				case "UnityEngine.UI.Text":
				{
					type = typeof(CuiTextComponent);
					break;
				}
				case "UnityEngine.UI.Image":
				{
					type = typeof(CuiImageComponent);
					break;
				}
				case "UnityEngine.UI.RawImage":
				{
					type = typeof(CuiRawImageComponent);
					break;
				}
				case "UnityEngine.UI.Button":
				{
					type = typeof(CuiButtonComponent);
					break;
				}
				case "UnityEngine.UI.Outline":
				{
					type = typeof(CuiOutlineComponent);
					break;
				}
				case "UnityEngine.UI.InputField":
				{
					type = typeof(CuiInputFieldComponent);
					break;
				}
				case "NeedsCursor":
				{
					type = typeof(CuiNeedsCursorComponent);
					break;
				}
				default:
				{
					if (str != "RectTransform")
					{
						return null;
					}
					type = typeof(CuiRectTransformComponent);
					break;
				}
			}
			object obj = Activator.CreateInstance(type);
			serializer.Populate(jObjects.CreateReader(), obj);
			return obj;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}