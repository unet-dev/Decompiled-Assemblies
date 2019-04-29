using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Shims;
using System;
using System.Text.RegularExpressions;

namespace Newtonsoft.Json.Converters
{
	[Preserve]
	public class RegexConverter : JsonConverter
	{
		private const string PatternName = "Pattern";

		private const string OptionsName = "Options";

		public RegexConverter()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Regex);
		}

		private bool HasFlag(RegexOptions options, RegexOptions flag)
		{
			return (options & flag) == flag;
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.StartObject)
			{
				return this.ReadRegexObject(reader, serializer);
			}
			if (reader.TokenType != JsonToken.String)
			{
				throw JsonSerializationException.Create(reader, "Unexpected token when reading Regex.");
			}
			return this.ReadRegexString(reader);
		}

		private Regex ReadRegexObject(JsonReader reader, JsonSerializer serializer)
		{
			string value = null;
			RegexOptions? nullable = null;
			while (reader.Read())
			{
				JsonToken tokenType = reader.TokenType;
				if (tokenType == JsonToken.PropertyName)
				{
					string str = reader.Value.ToString();
					if (!reader.Read())
					{
						throw JsonSerializationException.Create(reader, "Unexpected end when reading Regex.");
					}
					if (string.Equals(str, "Pattern", StringComparison.OrdinalIgnoreCase))
					{
						value = (string)reader.Value;
					}
					else if (!string.Equals(str, "Options", StringComparison.OrdinalIgnoreCase))
					{
						reader.Skip();
					}
					else
					{
						nullable = new RegexOptions?(serializer.Deserialize<RegexOptions>(reader));
					}
				}
				else
				{
					if (tokenType == JsonToken.Comment)
					{
						continue;
					}
					if (tokenType == JsonToken.EndObject)
					{
						if (value == null)
						{
							throw JsonSerializationException.Create(reader, "Error deserializing Regex. No pattern found.");
						}
						string str1 = value;
						RegexOptions? nullable1 = nullable;
						return new Regex(str1, (nullable1.HasValue ? nullable1.GetValueOrDefault() : RegexOptions.None));
					}
				}
			}
			throw JsonSerializationException.Create(reader, "Unexpected end when reading Regex.");
		}

		private object ReadRegexString(JsonReader reader)
		{
			string value = (string)reader.Value;
			int num = value.LastIndexOf('/');
			string str = value.Substring(1, num - 1);
			RegexOptions regexOption = RegexOptions.None;
			string str1 = value.Substring(num + 1);
			for (int i = 0; i < str1.Length; i++)
			{
				char chr = str1[i];
				if (chr <= 'm')
				{
					if (chr == 'i')
					{
						regexOption |= RegexOptions.IgnoreCase;
					}
					else if (chr == 'm')
					{
						regexOption |= RegexOptions.Multiline;
					}
				}
				else if (chr == 's')
				{
					regexOption |= RegexOptions.Singleline;
				}
				else if (chr == 'x')
				{
					regexOption |= RegexOptions.ExplicitCapture;
				}
			}
			return new Regex(str, regexOption);
		}

		private void WriteBson(BsonWriter writer, Regex regex)
		{
			string str = null;
			if (this.HasFlag(regex.Options, RegexOptions.IgnoreCase))
			{
				str = string.Concat(str, "i");
			}
			if (this.HasFlag(regex.Options, RegexOptions.Multiline))
			{
				str = string.Concat(str, "m");
			}
			if (this.HasFlag(regex.Options, RegexOptions.Singleline))
			{
				str = string.Concat(str, "s");
			}
			str = string.Concat(str, "u");
			if (this.HasFlag(regex.Options, RegexOptions.ExplicitCapture))
			{
				str = string.Concat(str, "x");
			}
			writer.WriteRegex(regex.ToString(), str);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			Regex regex = (Regex)value;
			BsonWriter bsonWriter = writer as BsonWriter;
			if (bsonWriter != null)
			{
				this.WriteBson(bsonWriter, regex);
				return;
			}
			this.WriteJson(writer, regex, serializer);
		}

		private void WriteJson(JsonWriter writer, Regex regex, JsonSerializer serializer)
		{
			DefaultContractResolver contractResolver = serializer.ContractResolver as DefaultContractResolver;
			writer.WriteStartObject();
			writer.WritePropertyName((contractResolver != null ? contractResolver.GetResolvedPropertyName("Pattern") : "Pattern"));
			writer.WriteValue(regex.ToString());
			writer.WritePropertyName((contractResolver != null ? contractResolver.GetResolvedPropertyName("Options") : "Options"));
			serializer.Serialize(writer, regex.Options);
			writer.WriteEndObject();
		}
	}
}