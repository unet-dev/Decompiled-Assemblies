using Newtonsoft.Json;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Newtonsoft.Json.Linq
{
	[Preserve]
	public class JArray : JContainer, IList<JToken>, ICollection<JToken>, IEnumerable<JToken>, IEnumerable
	{
		private readonly List<JToken> _values = new List<JToken>();

		protected override IList<JToken> ChildrenTokens
		{
			get
			{
				return this._values;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public override JToken this[object key]
		{
			get
			{
				ValidationUtils.ArgumentNotNull(key, "key");
				if (!(key is int))
				{
					throw new ArgumentException("Accessed JArray values with invalid key value: {0}. Int32 array index expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
				}
				return this.GetItem((int)key);
			}
			set
			{
				ValidationUtils.ArgumentNotNull(key, "key");
				if (!(key is int))
				{
					throw new ArgumentException("Set JArray values with invalid key value: {0}. Int32 array index expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
				}
				this.SetItem((int)key, value);
			}
		}

		public JToken this[int index]
		{
			get
			{
				return this.GetItem(index);
			}
			set
			{
				this.SetItem(index, value);
			}
		}

		public override JTokenType Type
		{
			get
			{
				return JTokenType.Array;
			}
		}

		public JArray()
		{
		}

		public JArray(JArray other) : base(other)
		{
		}

		public JArray(params object[] content) : this((object)content)
		{
		}

		public JArray(object content)
		{
			this.Add(content);
		}

		public void Add(JToken item)
		{
			this.Add(item);
		}

		public void Clear()
		{
			this.ClearItems();
		}

		internal override JToken CloneToken()
		{
			return new JArray(this);
		}

		public bool Contains(JToken item)
		{
			return this.ContainsItem(item);
		}

		public void CopyTo(JToken[] array, int arrayIndex)
		{
			this.CopyItemsTo(array, arrayIndex);
		}

		internal override bool DeepEquals(JToken node)
		{
			JArray jArrays = node as JArray;
			if (jArrays == null)
			{
				return false;
			}
			return base.ContentsEqual(jArrays);
		}

		public static new JArray FromObject(object o)
		{
			return JArray.FromObject(o, JsonSerializer.CreateDefault());
		}

		public static new JArray FromObject(object o, JsonSerializer jsonSerializer)
		{
			JToken jTokens = JToken.FromObjectInternal(o, jsonSerializer);
			if (jTokens.Type != JTokenType.Array)
			{
				throw new ArgumentException("Object serialized to {0}. JArray instance expected.".FormatWith(CultureInfo.InvariantCulture, jTokens.Type));
			}
			return (JArray)jTokens;
		}

		internal override int GetDeepHashCode()
		{
			return base.ContentsHashCode();
		}

		public IEnumerator<JToken> GetEnumerator()
		{
			return this.Children().GetEnumerator();
		}

		public int IndexOf(JToken item)
		{
			return this.IndexOfItem(item);
		}

		internal override int IndexOfItem(JToken item)
		{
			return this._values.IndexOfReference<JToken>(item);
		}

		public void Insert(int index, JToken item)
		{
			this.InsertItem(index, item, false);
		}

		public static new JArray Load(JsonReader reader)
		{
			return JArray.Load(reader, null);
		}

		public static new JArray Load(JsonReader reader, JsonLoadSettings settings)
		{
			if (reader.TokenType == JsonToken.None && !reader.Read())
			{
				throw JsonReaderException.Create(reader, "Error reading JArray from JsonReader.");
			}
			reader.MoveToContent();
			if (reader.TokenType != JsonToken.StartArray)
			{
				throw JsonReaderException.Create(reader, "Error reading JArray from JsonReader. Current JsonReader item is not an array: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
			}
			JArray jArrays = new JArray();
			jArrays.SetLineInfo(reader as IJsonLineInfo, settings);
			jArrays.ReadTokenFrom(reader, settings);
			return jArrays;
		}

		internal override void MergeItem(object content, JsonMergeSettings settings)
		{
			IEnumerable enumerable;
			if (base.IsMultiContent(content) || content is JArray)
			{
				enumerable = (IEnumerable)content;
			}
			else
			{
				enumerable = null;
			}
			IEnumerable enumerable1 = enumerable;
			if (enumerable1 == null)
			{
				return;
			}
			JContainer.MergeEnumerableContent(this, enumerable1, settings);
		}

		public static new JArray Parse(string json)
		{
			return JArray.Parse(json, null);
		}

		public static new JArray Parse(string json, JsonLoadSettings settings)
		{
			JArray jArrays;
			using (JsonReader jsonTextReader = new JsonTextReader(new StringReader(json)))
			{
				JArray jArrays1 = JArray.Load(jsonTextReader, settings);
				if (jsonTextReader.Read() && jsonTextReader.TokenType != JsonToken.Comment)
				{
					throw JsonReaderException.Create(jsonTextReader, "Additional text found in JSON string after parsing content.");
				}
				jArrays = jArrays1;
			}
			return jArrays;
		}

		public bool Remove(JToken item)
		{
			return this.RemoveItem(item);
		}

		public void RemoveAt(int index)
		{
			this.RemoveItemAt(index);
		}

		public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
		{
			writer.WriteStartArray();
			for (int i = 0; i < this._values.Count; i++)
			{
				this._values[i].WriteTo(writer, converters);
			}
			writer.WriteEndArray();
		}
	}
}